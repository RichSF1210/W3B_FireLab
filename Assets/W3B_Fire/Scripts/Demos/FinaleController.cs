using UnityEngine;

// =====================================================================
// FinaleController  (Rich presents, everyone back together)
// One campfire, three soundtracks:
//   1  RECORDED, STATIC   one recording on a loop: frozen in time
//   2  RECORDED, DYNAMIC  the same kind of recordings under rules (Part A)
//   3  PROCEDURAL         FireRecipe, generated from numbers (Part B)
// Try the same change on each. Some work on all three; some only in code.
// Keys: 1 2 3 switch soundtrack, B = burn down, L = add a log.
// =====================================================================
public class FinaleController : MonoBehaviour
{
    public enum Mode { RecordedStatic, RecordedDynamic, Procedural }

    public FireController staticFire;     // plays fire_full_loop.wav on a plain loop
    public FireController recordedFire;   // hiss loop + lapping loop + random crackles
    public FireController proceduralFire; // FireRecipe
    public FireRecipe recipe;
    [Range(0f, 1f)] public float intensity = 0.5f;
    public bool burnDown;
    public Mode mode = Mode.RecordedStatic;

    float recordedBytes;
    GUIStyle title, body, note, button;

    FireController[] All => new[] { staticFire, recordedFire, proceduralFire };

    void Start()
    {
        foreach (var a in recordedFire.GetComponentsInChildren<AudioSource>(true)) recordedBytes += Bytes(a.clip);
        foreach (var s in recordedFire.GetComponentsInChildren<SampleOneShotLayer>(true))
            if (s.clips != null) foreach (var c in s.clips) recordedBytes += Bytes(c);
        Apply();
    }

    static float Bytes(AudioClip c) => c == null ? 0f : c.samples * c.channels * 2f;

    public void Hear(Mode m) { mode = m; Apply(); }

    public void SetIntensity(float v)
    {
        intensity = v;
        foreach (var f in All) f.intensity = v;
    }

    public void SetBurnDown(bool on)
    {
        burnDown = on;
        foreach (var f in All) { f.burnDown = on; f.burnDownPerSecond = 0.06f; }
    }

    public void AddLog() { foreach (var f in All) f.AddLog(); }

    void Apply()
    {
        Mute(staticFire, mode != Mode.RecordedStatic);
        Mute(recordedFire, mode != Mode.RecordedDynamic);
        Mute(proceduralFire, mode != Mode.Procedural);
    }

    static void Mute(FireController f, bool muted)
    {
        foreach (var a in f.GetComponentsInChildren<AudioSource>(true)) a.mute = muted;
        f.transform.Find("Visuals").gameObject.SetActive(!muted);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Hear(Mode.RecordedStatic);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Hear(Mode.RecordedDynamic);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Hear(Mode.Procedural);
        if (Input.GetKeyDown(KeyCode.B)) SetBurnDown(!burnDown);
        // L is handled by AddLogOnKey on each campfire: the one line students see.
        if (burnDown) intensity = recordedFire.intensity;   // follow the fire as it dies
    }

    void Styles()
    {
        if (title != null) return;
        title = new GUIStyle(GUI.skin.label) { fontSize = 30, fontStyle = FontStyle.Bold, wordWrap = true };
        body = new GUIStyle(GUI.skin.label) { fontSize = 21, wordWrap = true };
        note = new GUIStyle(GUI.skin.label) { fontSize = 21, wordWrap = true, fontStyle = FontStyle.Italic };
        note.normal.textColor = new Color(1f, 0.8f, 0.4f);
        button = new GUIStyle(GUI.skin.button) { fontSize = 20 };
    }

    static readonly string[] Names = { "RECORDED, STATIC: one loop, frozen", "RECORDED, DYNAMIC: Part A", "PROCEDURAL: Part B" };

    void OnGUI()
    {
        Styles();
        float s = Screen.height / 1080f;
        GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1f));
        GUILayout.BeginArea(new Rect(30, 25, 680, 1040));

        GUILayout.Label("Now hearing: " + Names[(int)mode], title);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("1  Static", button, GUILayout.Height(44))) Hear(Mode.RecordedStatic);
        if (GUILayout.Button("2  Dynamic", button, GUILayout.Height(44))) Hear(Mode.RecordedDynamic);
        if (GUILayout.Button("3  Procedural", button, GUILayout.Height(44))) Hear(Mode.Procedural);
        GUILayout.EndHorizontal();
        GUILayout.Space(12);

        GUILayout.Label("Change the fire", title);
        GUILayout.Label("Intensity, embers to blaze: " + intensity.ToString("0.00"), body);
        float v = GUILayout.HorizontalSlider(intensity, 0f, 1f, GUILayout.Height(26));
        if (!Mathf.Approximately(v, intensity)) SetIntensity(v);
        bool b = GUILayout.Toggle(burnDown, "  B  Burn down over time (adaptive)", button, GUILayout.Height(40));
        if (b != burnDown) SetBurnDown(b);
        if (GUILayout.Button("L  Throw on a log (interactive)", button, GUILayout.Height(40))) AddLog();
        GUILayout.Label(mode == Mode.RecordedStatic
            ? "Nothing changes. A recording on a loop cannot respond: it is frozen in time."
            : mode == Mode.RecordedDynamic
            ? "Louder and quieter, more or fewer crackles. Turned down, a recorded blaze is still a blaze, not embers."
            : "The fire changes what it is: sparse ticks as it dies, a burst of crackling when a log lands.", note);
        GUILayout.Space(12);

        GUILayout.Label("Only code can do this", title);
        if (mode == Mode.Procedural && recipe != null)
        {
            GUILayout.Label("Wood: damp oak  <->  dry pine: " + recipe.dryness.ToString("0.00"), body);
            recipe.dryness = GUILayout.HorizontalSlider(recipe.dryness, 0f, 1f, GUILayout.Height(26));
            GUILayout.Label("Size: campfire  <->  bonfire: " + recipe.size.ToString("0.00"), body);
            recipe.size = GUILayout.HorizontalSlider(recipe.size, 0f, 1f, GUILayout.Height(26));
            GUILayout.Label("New wood, new size: same script, two numbers. Nothing re-recorded.", note);
        }
        else
        {
            GUILayout.Label("Different wood? A bigger fire? With recordings: go outside and record a new fire.", body);
            GUILayout.Label("Switch to Procedural (3) to try it.", note);
        }
        GUILayout.Space(12);

        GUILayout.Label("Memory", title);
        GUILayout.Label("Recorded: " + (recordedBytes / 1048576f).ToString("0.0") + " MB of audio, and it loops.\nProcedural: 0 bytes of audio. It never repeats.", body);
        GUILayout.EndArea();
    }
}
