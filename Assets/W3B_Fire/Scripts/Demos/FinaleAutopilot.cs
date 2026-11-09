using UnityEngine;

// =====================================================================
// FinaleAutopilot: plays the finale by itself for recording a video.
// =====================================================================
public class FinaleAutopilot : MonoBehaviour
{
    public bool runOnPlay = false;
    FinaleController f; float t; int step = -1;
    // Seconds at which each step starts.
    readonly float[] at = { 0, 10, 22, 34, 46, 56, 66, 80, 94, 110 };

    void Start()
    {
        bool recording = false;
#if UNITY_EDITOR
        recording = UnityEditor.SessionState.GetBool("W3B_Recording", false);
#endif
        f = GetComponent<FinaleController>();
        enabled = (runOnPlay || recording) && f != null;
    }

    void Update()
    {
        t += Time.deltaTime;
        while (step + 1 < at.Length && t >= at[step + 1]) { step++; Enter(step); }
        if (step == 1 || step == 3) f.SetIntensity(Mathf.PingPong((t - at[step]) / 6f, 1f) * 0.9f + 0.05f); // sweep
        if (step == 7) f.recipe.dryness = Mathf.Clamp01(1f - (t - at[7]) / 12f);   // dry pine -> damp oak
        if (step == 8) f.recipe.size = Mathf.Clamp01((t - at[8]) / 14f);            // campfire -> bonfire
        if (step == 9)
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
            case 0: f.Hear(false); f.SetIntensity(0.6f); break;                 // recorded, steady
            case 1: break;                                                       // recorded, sweep
            case 2: f.Hear(true); f.SetIntensity(0.6f); break;                   // generated, steady
            case 3: break;                                                       // generated, sweep
            case 4: f.Hear(false); f.SetIntensity(0.8f); f.SetBurnDown(true); break;   // recorded burns down
            case 5: f.SetBurnDown(false); f.SetIntensity(0.8f); f.Hear(true); f.SetBurnDown(true); break; // generated burns down
            case 6: f.SetBurnDown(false); f.SetIntensity(0.25f); f.AddLog(); Invoke(nameof(Log), 4f); break; // logs on
            case 7: f.recipe.size = 0.5f; f.SetIntensity(0.6f); break;
            case 8: f.recipe.dryness = 0.5f; break;
        }
    }
    void Log() => f.AddLog();
}
