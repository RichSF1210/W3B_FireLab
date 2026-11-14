using UnityEngine;
using M = FinaleController.Mode;

// =====================================================================
// FinaleAutopilot: plays the finale by itself for recording a video.
// STUDENTS: you do not need this script. It was used to record the backup
// videos: it presses the buttons and moves the sliders on a timer, by itself.
// =====================================================================
public class FinaleAutopilot : MonoBehaviour
{
    public bool runOnPlay = false;
    FinaleController f; float t; int step = -1;
    // Seconds at which each step starts (see Enter for what each does).
    public static readonly float[] At = { 0, 18, 24, 36, 42, 54, 64, 74, 88, 102, 118 };

    void Start()
    {
        bool recording = false;
#if UNITY_EDITOR
        recording = UnityEditor.SessionState.GetBool("W3B_Recording", false);
#endif
        f = GetComponent<FinaleController>();
        enabled = (runOnPlay || recording) && f != null;
    }

    float Sweep(int s) => Mathf.PingPong((t - At[s]) / 6f, 1f) * 0.9f + 0.05f;

    void Update()
    {
        t += Time.deltaTime;
        while (step + 1 < At.Length && t >= At[step + 1]) { step++; Enter(step); }
        if (step == 0 && t > 10f) f.SetIntensity(Sweep(0));          // static: sweep, nothing happens
        if (step == 2 || step == 4) f.SetIntensity(Sweep(step));     // dynamic and procedural sweeps
        if (step == 8) f.recipe.dryness = Mathf.Clamp01(1f - (t - At[8]) / 12f);   // dry pine -> damp oak
        if (step == 9) f.recipe.size = Mathf.Clamp01((t - At[9]) / 14f);           // campfire -> bonfire
        if (step == 10)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            enabled = false;
        }
    }

    void Enter(int s)
    {
        switch (s)
        {
            case 0: f.Hear(M.RecordedStatic); f.SetIntensity(0.6f); break;
            case 1: f.Hear(M.RecordedDynamic); f.SetIntensity(0.6f); break;
            case 2: break;
            case 3: f.Hear(M.Procedural); f.SetIntensity(0.6f); break;
            case 4: break;
            case 5: f.Hear(M.RecordedDynamic); f.SetIntensity(0.8f); f.SetBurnDown(true); break;
            case 6: f.SetBurnDown(false); f.SetIntensity(0.8f); f.Hear(M.Procedural); f.SetBurnDown(true); break;
            case 7: f.SetBurnDown(false); f.SetIntensity(0.25f); f.AddLog(); Invoke(nameof(Log), 5f); break;
            case 8: f.recipe.size = 0.5f; f.SetIntensity(0.6f); break;
            case 9: f.recipe.dryness = 0.5f; break;
        }
    }
    void Log() => f.AddLog();
}
