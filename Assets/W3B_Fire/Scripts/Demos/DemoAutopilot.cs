using UnityEngine;

// =====================================================================
// DemoAutopilot: plays the W3B demo by itself (for recording, or rehearsal).
// Tick "Run On Play" to watch it without recording.
// =====================================================================
public class DemoAutopilot : MonoBehaviour
{
    public bool runOnPlay = false;
    public float speed = 1f;

    DemoSequencer sequencer;
    float t;
    int step = -1;

    // Seconds on each of the 10 steps.
    readonly float[] durations = { 8f, 16f, 12f, 8f, 12f, 8f, 8f, 8f, 16f, 14f, 30f };

    void Start()
    {
        sequencer = GetComponent<DemoSequencer>();
        bool recording = false;
#if UNITY_EDITOR
        recording = UnityEditor.SessionState.GetBool("W3B_Recording", false);
#endif
        enabled = sequencer != null && (runOnPlay || recording);
    }

    void Update()
    {
        t += Time.deltaTime * speed;
        float start = 0f;
        for (int i = 0; i < durations.Length && i < sequencer.steps.Length; i++)
        {
            if (t < start + durations[i]) { Drive(i, (t - start) / durations[i]); return; }
            start += durations[i];
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        enabled = false;
    }

    void Drive(int i, float p)
    {
        if (i != step) { step = i; sequencer.Show(i); }
        var root = sequencer.steps[i].root;
        if (root == null) return;
        switch (i)
        {
            case 4: root.GetComponent<IntensityController>().intensity = Mathf.Sin(p * Mathf.PI) * 0.9f + 0.1f; break;
            case 0: root.GetComponent<SineDemo>().frequency = Mathf.Lerp(220f, 660f, Mathf.Sin(p * Mathf.PI)); break;
            case 1: // digital: bit depth down to 2, then sample hold up
                var s = root.GetComponent<SineDemo>();
                s.frequency = 220f;
                s.bitDepth = p < 0.5f ? Mathf.RoundToInt(Mathf.Lerp(16f, 2f, p * 2f)) : 16;
                s.holdSamples = p < 0.5f ? 1 : Mathf.RoundToInt(Mathf.Lerp(1f, 40f, (p - 0.5f) * 2f));
                break;
            case 2:
                var n = root.GetComponent<NoiseDemo>();
                n.noiseMix = Mathf.Clamp01(p * 4f);
                n.highPass = p > 0.7f;
                n.cutoff = p < 0.7f ? Mathf.Lerp(20000f, 200f, Mathf.Clamp01((p - 0.25f) / 0.45f)) : 3000f;
                break;
            case 8: root.GetComponent<PdRain>().intensity = Mathf.Lerp(0.05f, 1f, p); break;
            case 10: // reveal: each element alone, then all, then a blaze
                var f = root.GetComponent<FireRecipe>();
                f.solo = p < 0.15f ? FireRecipe.Solo.Hissing : p < 0.3f ? FireRecipe.Solo.Crackling : p < 0.45f ? FireRecipe.Solo.Lapping : FireRecipe.Solo.All;
                f.intensity = p < 0.55f ? 0.5f : Mathf.Sin((p - 0.55f) / 0.45f * Mathf.PI) * 0.9f + 0.05f;
                break;
            case 9:
                var e = root.GetComponentInChildren<BurstLayer>();
                var nl = root.GetComponentInChildren<NoiseLayer>();
                if (nl != null) nl.cutoff = Mathf.Lerp(150f, 3000f, Mathf.Clamp01(p * 2f));
                if (e != null) { e.burstsPerSecond = Mathf.Lerp(1f, 40f, Mathf.Clamp01(p * 2f - 1f)); }
                break;
        }
    }
}
