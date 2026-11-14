using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// =====================================================================
// DemoSequencer: the presenter for the W3B_Demo scene. One step at a time,
// with big on-screen sliders and a live oscilloscope for the projector.
//
// KEYS
//   Right arrow = next step, Left arrow = previous step.
//   After step 10 the right arrow does NOT open step 11 (the reveal): it shows
//   a "back to the slides" card instead, so the reveal is not spoiled.
//   R = open step 11, the reveal, when it is time (near the end of the lab).
//
// WHAT IS ON SCREEN FOR EACH STEP (written for students to READ, not only hear)
//   a badge: RECORDED (made from audio files) or CODE (made by maths, no files)
//   one or two plain sentences saying what is happening
//   "Try" : what to move, and what to listen for
//   "Next: back to the slides" when the next thing is a slide
// The words live in the arrays below, so they are easy to change.
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

    // ---- The on-screen words, one entry per step (index 0 = step 1). ----
    // true = made from recordings; false = made by code.
    static readonly bool[] Recorded = { false, false, false, true, true, false, false, false, false, false, false };

    static readonly string[] Plain =
    {
        "One pure tone, made by code. Every sound can be built from sine waves (Fourier).",
        "On a computer, sound is a list of numbers: tens of thousands every second (44,100 or 48,000). DSP (digital signal processing) means making or changing sound with maths, one number at a time.",
        "Noise is every frequency at once. A filter keeps only some of them. A filter is DSP.",
        "A recording plays back exactly the same every time. It is frozen.",
        "The same kind of recordings, played by rules: random choice, pitch, volume and timing. It never repeats exactly. This is Part A.",
        "From here on: no recordings. Every drop is made by code as it plays. Layer 1: random noise, squashed so only its rare high peaks get through. Each peak plays a tiny beep: a drop.",
        "Layer 2: a short pulse, and sometimes a small bubble ringing as it rises. The physics of a water drop (Farnell).",
        "Layers 3 and 4: low noise for drops hitting big surfaces, and quiet noise for distant rain. Now it is the full rain.",
        "One slider changes WHAT the rain is, not only how loud: more drops, more rumble. A recording cannot do this.",
        "Part B gives you two building blocks: Noise Layer (a constant sound) and Burst Layer (short random events). This is Part B.",
        "Fire has three parts (Farnell): hissing = gas escaping; crackling = sap bursting; lapping = the roar of the flames."
    };

    static readonly string[] Try =
    {
        "Try: drag Frequency. Bigger number = higher note. (440 Hz is the note A.)",
        "Try: lower Bit depth, then raise Hold. Up and down = bit depth: how finely each number is measured. Side to side = sample rate: how often. Fewer of either: the wave becomes stairs, and it sounds buzzy.",
        "Try: move Sine to Noise. Sweep the filter cutoff low (rumble) then high. Tick High pass: now only the highs (hiss).",
        "Try: just listen. Can you hear the moment it repeats?",
        "Try: move Intensity. Listen: does it ever repeat exactly?",
        "Try: move Intensity, drizzle to storm. Listen for single drops.",
        "Try: move Intensity. Listen for the little rising 'bloop' of a bubble.",
        "Try: move Intensity. Listen to the low rumble grow.",
        "Try: drizzle to storm. Is it only louder, or does it change what it is?",
        "Try: move each slider. Which one sounds like part of a fire?",
        "Try: Hissing, then Crackling, then Lapping, then All. Then move Intensity."
    };

    static bool BackToSlidesAfter(int index) => index == 2 || index == 4 || index == 9;

    bool holding;   // true = showing the "back to the slides" card instead of step 11

    Texture2D scopeTex;
    Color32[] pixels;
    const int ScopeW = 600, ScopeH = 160;

    void Start() => Show(current);

    void Update()
    {
        bool next = false, previous = false, reveal = false;
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k != null) { next = k.rightArrowKey.wasPressedThisFrame; previous = k.leftArrowKey.wasPressedThisFrame; reveal = k.rKey.wasPressedThisFrame; }
#else
        next = Input.GetKeyDown(KeyCode.RightArrow);
        previous = Input.GetKeyDown(KeyCode.LeftArrow);
        reveal = Input.GetKeyDown(KeyCode.R);
#endif
        int last = steps.Length - 1;                       // the reveal is the last step
        if (reveal) { holding = false; Show(last); return; }
        if (holding)
        {
            if (previous) { holding = false; Show(last - 1); }   // left arrow: back to step 10
            return;                                               // right arrow does nothing on the card
        }
        if (next)
        {
            if (current == last - 1) { holding = true; HideAll(); }   // after step 10: the card, not the reveal
            else Show(Mathf.Min(current + 1, last));
        }
        if (previous) Show(Mathf.Max(current - 1, 0));
    }

    // Switch every step off: silence, while the "back to the slides" card shows.
    void HideAll()
    {
        foreach (var st in steps) if (st.root != null) st.root.SetActive(false);
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

        if (holding)   // the card that stands between step 10 and the reveal
        {
            GUILayout.Label("<size=26><b>Back to the slides.</b></size>", Rich());
            GUILayout.Label("<size=18>The reveal comes later. (Lecturer: press R when it is time. Left arrow = back to step 10.)</size>", Rich());
            GUILayout.EndArea();
            return;
        }

        // Title line, with the badge: RECORDED (audio files) or CODE (no audio files).
        bool rec = current < Recorded.Length && Recorded[current];
        string badge = rec ? "<color=#FFB74D>RECORDED (audio files)</color>" : "<color=#4FC3F7>CODE (no audio files)</color>";
        GUILayout.Label($"<size=22><b>{current + 1}/{steps.Length}  {step.title}</b></size>   <size=18><b>{badge}</b></size>", Rich());

        // The plain words for this step (fall back to the scene's hint if a step has none).
        string plain = current < Plain.Length ? Plain[current] : step.hint;
        if (!string.IsNullOrEmpty(plain)) GUILayout.Label($"<size=16>{plain}</size>", Rich());
        if (current < Try.Length) GUILayout.Label($"<size=15><i>{Try[current]}</i></size>", Rich());
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
        if (BackToSlidesAfter(current))
            GUILayout.Label("<size=18><b><color=#FFD54F>Next: back to the slides</color></b></size>", Rich());
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
                // Bit depth: how many different heights each number may have.
                // The code allows 2^(bits-1) steps each side of zero, plus zero itself.
                int heights = 2 * (int)Mathf.Pow(2f, s.bitDepth - 1) + 1;
                string h = s.bitDepth >= 16 ? "about 65,536 possible heights (smooth)" : heights.ToString("N0") + " possible heights";
                s.bitDepth = Mathf.RoundToInt(Slider("Bit depth (bits): " + s.bitDepth + " = " + h + ". Up and down", s.bitDepth, 1f, 16f, false));
                // Hold: keep each number for N samples = a lower sample rate.
                int perSecond = AudioSettings.outputSampleRate / Mathf.Max(1, s.holdSamples);
                s.holdSamples = Mathf.RoundToInt(Slider("Hold each number for " + s.holdSamples + (s.holdSamples == 1 ? " sample = " : " samples = ") + perSecond.ToString("N0") + " new numbers a second. Side to side", s.holdSamples, 1f, 64f, false));
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

    // A labelled slider. showValue = false when the label already says the value.
    static float Slider(string label, float value, float min, float max, bool showValue = true)
    {
        GUILayout.Label(showValue ? $"{label}: {value:0.##}" : label, Rich());
        return GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(400));
    }

    static GUIStyle Rich() => new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true, fontSize = 14 };
}
