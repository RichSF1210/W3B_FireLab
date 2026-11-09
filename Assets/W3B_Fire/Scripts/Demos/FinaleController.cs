using UnityEngine;

// =====================================================================
// FinaleController  (Rich presents, everyone back together)
// One campfire, two soundtracks: RECORDED (wav files, Route 1) and
// GENERATED (FireRecipe, Route 2). Switch between them and try the same
// change on each. Some changes work on both. Some only work in code.
// Keys: R = recorded, G = generated, L = add a log, B = burn down.
// =====================================================================
public class FinaleController : MonoBehaviour
{
    public FireController recordedFire;
    public FireController generatedFire;
    public FireRecipe recipe;
    [Range(0f, 1f)] public float intensity = 0.5f;
    public bool burnDown;
    public bool hearGenerated;

    float recordedBytes;
    GUIStyle title, body, note, button;

    void Start()
    {
        foreach (var s in recordedFire.GetComponentsInChildren<SampleLoopLayer>(true))
            recordedBytes += Bytes(s.GetComponent<AudioSource>().clip);
        foreach (var s in recordedFire.GetComponentsInChildren<SampleOneShotLayer>(true))
            if (s.clips != null) foreach (var c in s.clips) recordedBytes += Bytes(c);
        Apply();
    }

    static float Bytes(AudioClip c) => c == null ? 0f : c.samples * c.channels * 2f;

    public void Hear(bool generated) { hearGenerated = generated; Apply(); }

    public void SetIntensity(float v)
    {
        intensity = v;
        recordedFire.intensity = v; generatedFire.intensity = v;
    }

    public void SetBurnDown(bool on)
    {
        burnDown = on;
        recordedFire.burnDown = on; generatedFire.burnDown = on;
        recordedFire.burnDownPerSecond = generatedFire.burnDownPerSecond = 0.06f;
    }

    public void AddLog() { recordedFire.AddLog(); generatedFire.AddLog(); }

    void Apply()
    {
        foreach (var a in recordedFire.GetComponentsInChildren<AudioSource>(true)) a.mute = hearGenerated;
        foreach (var a in generatedFire.GetComponentsInChildren<AudioSource>(true)) a.mute = !hearGenerated;
        recordedFire.transform.Find("Visuals").gameObject.SetActive(!hearGenerated);
        generatedFire.transform.Find("Visuals").gameObject.SetActive(hearGenerated);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Hear(false);
        if (Input.GetKeyDown(KeyCode.G)) Hear(true);
        if (Input.GetKeyDown(KeyCode.B)) SetBurnDown(!burnDown);
        // L is handled by AddLogOnKey on each campfire: the one line students see.
        if (burnDown) intensity = recordedFire.intensity;   // follow the fire as it dies
    }

    void Styles()
    {
        if (title != null) return;
        title = new GUIStyle(GUI.skin.label) { fontSize = 30, fontStyle = FontStyle.Bold, wordWrap = true };
        body = new GUIStyle(GUI.skin.label) { fontSize = 20, wordWrap = true };
        note = new GUIStyle(GUI.skin.label) { fontSize = 20, wordWrap = true, fontStyle = FontStyle.Italic };
        note.normal.textColor = new Color(1f, 0.8f, 0.4f);
        button = new GUIStyle(GUI.skin.button) { fontSize = 20 };
    }

    void OnGUI()
    {
        Styles();
        float s = Screen.height / 1080f;
        GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1f));
        GUILayout.BeginArea(new Rect(30, 25, 640, 1040));

        GUILayout.Label(hearGenerated ? "Now hearing: GENERATED (code, Route 2)" : "Now hearing: RECORDED (wav files, Route 1)", title);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("R  Recorded", button, GUILayout.Height(44))) Hear(false);
        if (GUILayout.Button("G  Generated", button, GUILayout.Height(44))) Hear(true);
        GUILayout.EndHorizontal();
        GUILayout.Space(14);

        GUILayout.Label("Both can do this", title);
        GUILayout.Label("Intensity, embers to blaze: " + intensity.ToString("0.00"), body);
        float v = GUILayout.HorizontalSlider(intensity, 0f, 1f, GUILayout.Height(26));
        if (!Mathf.Approximately(v, intensity)) SetIntensity(v);
        bool b = GUILayout.Toggle(burnDown, "  B  Burn down over time (adaptive)", button, GUILayout.Height(40));
        if (b != burnDown) SetBurnDown(b);
        if (GUILayout.Button("L  Throw on a log (interactive)", button, GUILayout.Height(40))) AddLog();
        GUILayout.Label(hearGenerated
            ? "The fire changes what it is: fewer, sparser snaps as it dies; a burst of crackling when a log lands."
            : "The recording only gets quieter or louder. Turned down, a blaze is still a blaze, not embers.", note);
        GUILayout.Space(14);

        GUILayout.Label("Only code can do this", title);
        if (hearGenerated && recipe != null)
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
            GUILayout.Label("Switch to Generated (G) to try it.", note);
        }
        GUILayout.Space(14);

        GUILayout.Label("Memory", title);
        GUILayout.Label("Recorded: " + (recordedBytes / 1048576f).ToString("0.0") + " MB of audio, and it loops.\nGenerated: 0 bytes of audio. It never repeats.", body);
        GUILayout.EndArea();
    }
}
