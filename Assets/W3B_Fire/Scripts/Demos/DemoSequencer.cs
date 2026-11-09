using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// =====================================================================
// DemoSequencer: Rich's presenter. One step at a time, with big on-screen
// sliders and a live oscilloscope for the projector.
// Right arrow = next step, Left arrow = previous.
// =====================================================================
public class DemoSequencer : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public string title;
        [TextArea] public string hint;
        public GameObject root;
        public bool showExtraControls;
    }

    public Step[] steps;
    public int current = 0;
    [Range(1f, 3f)] public float uiScale = 2f;

    Texture2D scopeTex;
    Color32[] pixels;
    const int ScopeW = 600, ScopeH = 160;

    void Start() => Show(current);

    void Update()
    {
        bool next = false, previous = false;
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k != null) { next = k.rightArrowKey.wasPressedThisFrame; previous = k.leftArrowKey.wasPressedThisFrame; }
#else
        next = Input.GetKeyDown(KeyCode.RightArrow);
        previous = Input.GetKeyDown(KeyCode.LeftArrow);
#endif
        if (next) Show(Mathf.Min(current + 1, steps.Length - 1));
        if (previous) Show(Mathf.Max(current - 1, 0));
    }

    public void Show(int index)
    {
        current = index;
        for (int i = 0; i < steps.Length; i++)
            if (steps[i].root != null) steps[i].root.SetActive(i == current);
    }

    void OnGUI()
    {
        if (steps == null || steps.Length == 0) return;
        GUI.matrix = Matrix4x4.Scale(new Vector3(uiScale, uiScale, 1f));
        var step = steps[current];
        float w = Screen.width / uiScale - 20f;

        GUILayout.BeginArea(new Rect(10, 10, w, Screen.height / uiScale - 20f));
        GUILayout.Label($"<size=22><b>{current + 1}/{steps.Length}  {step.title}</b></size>", Rich());
        if (!string.IsNullOrEmpty(step.hint)) GUILayout.Label(step.hint, Rich());
        GUILayout.Space(6);

        if (step.root != null)
        {
            var gen = step.root.GetComponentInChildren<ProceduralSound>();
            if (gen != null)
            {
                DrawScope(gen);
                GUILayout.Label(scopeTex, GUILayout.Width(ScopeW / 1.5f), GUILayout.Height(ScopeH / 1.5f));
            }
            Controls(step);
        }
        GUILayout.FlexibleSpace();
        GUILayout.Label("←  previous     next  →", Rich());
        GUILayout.EndArea();
    }

    void Controls(Step step)
    {
        var root = step.root;
        foreach (var c in root.GetComponentsInChildren<IntensityController>()) c.intensity = Slider("Intensity", c.intensity, 0f, 1f);
        foreach (var s in root.GetComponentsInChildren<SineDemo>())
        {
            s.frequency = Slider("Frequency (Hz)", s.frequency, 50f, 2000f);
            if (step.showExtraControls)
            {
                s.bitDepth = Mathf.RoundToInt(Slider("Bit depth (steps per number)", s.bitDepth, 1f, 16f));
                s.holdSamples = Mathf.RoundToInt(Slider("Hold each number for N samples", s.holdSamples, 1f, 64f));
            }
        }
        foreach (var n in root.GetComponentsInChildren<NoiseDemo>())
        {
            n.noiseMix = Slider("Sine  ←→  Noise", n.noiseMix, 0f, 1f);
            n.cutoff = Slider("Filter cutoff (Hz)", n.cutoff, 50f, 20000f);
            n.highPass = GUILayout.Toggle(n.highPass, " High pass (keep the highs)");
        }
        foreach (var r in root.GetComponentsInChildren<PdRain>())
        {
            r.intensity = Slider("Intensity: drizzle  ←→  storm", r.intensity, 0f, 1f);
            if (step.showExtraControls)
            {
                r.droplets = Slider("Droplets", r.droplets, 0f, 2f);
                r.drips = Slider("Drips", r.drips, 0f, 2f);
                r.rumble = Slider("Rumble", r.rumble, 0f, 2f);
                r.ambience = Slider("Ambience", r.ambience, 0f, 2f);
            }
        }
        foreach (var f in root.GetComponentsInChildren<FireRecipe>())
        {
            f.intensity = Slider("Intensity: embers  ←→  blaze", f.intensity, 0f, 1f);
            GUILayout.BeginHorizontal();
            foreach (FireRecipe.Solo s in System.Enum.GetValues(typeof(FireRecipe.Solo)))
                if (GUILayout.Toggle(f.solo == s, " " + s + "   ", GUILayout.Width(110))) f.solo = s;
            GUILayout.EndHorizontal();
        }
        foreach (var n in root.GetComponentsInChildren<NoiseLayer>())
        {
            n.cutoff = Slider("Noise layer: filter cutoff (Hz)", n.cutoff, 20f, 15000f);
            n.wobbleDepth = Slider("Noise layer: wobble", n.wobbleDepth, 0f, 1f);
            n.level = Slider("Noise layer: level", n.level, 0f, 1f);
        }
        foreach (var e in root.GetComponentsInChildren<BurstLayer>())
        {
            e.burstsPerSecond = Slider("Burst layer: bursts per second", e.burstsPerSecond, 0.2f, 80f);
            e.burstLength = Slider("Burst layer: length (ms)", e.burstLength, 0.5f, 50f);
            e.brightness = Slider("Burst layer: brightness (Hz)", e.brightness, 100f, 12000f);
        }
    }

    // Draws the newest samples. For a sine, about two cycles, so you can see the steps.
    void DrawScope(ProceduralSound gen)
    {
        if (scopeTex == null)
        {
            scopeTex = new Texture2D(ScopeW, ScopeH, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            pixels = new Color32[ScopeW * ScopeH];
        }
        if (Event.current.type != EventType.Repaint) return;

        var bg = new Color32(20, 20, 24, 255);
        for (int i = 0; i < pixels.Length; i++) pixels[i] = bg;
        for (int x = 0; x < ScopeW; x++) pixels[(ScopeH / 2) * ScopeW + x] = new Color32(70, 70, 80, 255);

        var buf = gen.scope;
        int end = gen.scopeWrite;
        int count = 2048;
        var sine = gen as SineDemo;
        if (sine != null) count = Mathf.Clamp(Mathf.RoundToInt(2f * gen.SampleRate / sine.frequency), 24, 2048);

        int start = (end - count - 1024 + 4096) & 4095;
        if (sine != null)   // line up on an upward zero crossing so the wave stands still
            for (int i = 0; i < 1024; i++)
            {
                int a = (start + i) & 4095, b = (start + i + 1) & 4095;
                if (buf[a] < 0f && buf[b] >= 0f) { start = b; break; }
            }

        int prevY = -1;
        var line = new Color32(255, 170, 60, 255);
        for (int x = 0; x < ScopeW; x++)
        {
            int idx = (start + (int)((long)x * count / ScopeW)) & 4095;
            int y = Mathf.Clamp((int)((buf[idx] * 0.45f + 0.5f) * (ScopeH - 1)), 0, ScopeH - 1);
            int from = prevY < 0 ? y : Mathf.Min(prevY, y), to = prevY < 0 ? y : Mathf.Max(prevY, y);
            for (int yy = from; yy <= to; yy++) pixels[yy * ScopeW + x] = line;
            prevY = y;
        }
        scopeTex.SetPixels32(pixels);
        scopeTex.Apply(false);
    }

    static float Slider(string label, float value, float min, float max)
    {
        GUILayout.Label($"{label}: {value:0.##}", Rich());
        return GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(400));
    }

    static GUIStyle Rich() => new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true, fontSize = 14 };
}
