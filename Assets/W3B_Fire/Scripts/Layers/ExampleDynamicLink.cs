using UnityEngine;

// =====================================================================
// ExampleDynamicLink: what a finished Part B might look like.
// Each line is an RTPC (real-time parameter control, the Wwise/FMOD idea):
// one game value (Intensity) is mapped onto one sound parameter.
// Students write ONE of these lines; this example has three.
// =====================================================================
public class ExampleDynamicLink : MonoBehaviour
{
    public FireController fire;
    public BurstLayer crackleLayer;
    public NoiseLayer hissLayer;
    public NoiseLayer roarLayer;

    void Update()
    {
        if (fire == null) return;
        float i = fire.CurrentIntensity;

        crackleLayer.burstsPerSecond = Mathf.Lerp(1f, 30f, i);   // more events
        roarLayer.cutoff = Mathf.Lerp(150f, 700f, i);             // brighter roar
        hissLayer.level = Mathf.Lerp(0.05f, 0.35f, i);            // louder hiss
    }
}
