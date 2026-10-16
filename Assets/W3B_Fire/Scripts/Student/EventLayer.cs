using UnityEngine;

// =====================================================================
// EventLayer  (CODE route: a ready-made building block)
// WHAT IT IS: very short bursts of noise at random moments.
//   Sculpt it BY EAR: how often, how long, how bright, how varied.
// TRY: short + bright + frequent, then long + dark + rare. What do they
//   sound like? Which matches something in the mystery sound?
// MAKING IT DYNAMIC: see MyDynamicLink.cs (one line of code).
// =====================================================================
public class EventLayer : ProceduralSound
{
    [Range(0.2f, 80f)] public float eventsPerSecond = 8f;
    [Range(0.5f, 50f)] public float eventLength = 4f;     // milliseconds
    [Range(100f, 12000f)] public float brightness = 2000f;   // high-pass cutoff, Hz
    [Range(0f, 1f)] public float variation = 0.6f;        // how different each event is
    [Range(0f, 1f)] public float level = 0.6f;

    PdHip hip; float env, decay; int counter;

    protected override void Start()
    {
        hip = new PdHip(brightness, AudioSettings.outputSampleRate);
        base.Start();
    }

    protected override float NextSample()
    {
        if (hip == null) return 0f;
        if ((counter++ & 63) == 0) hip.Set(brightness, sampleRate);

        if (Random01() < eventsPerSecond / sampleRate)               // a new event
        {
            env = 1f - variation * Random01();                        // random loudness
            float ms = eventLength * (1f - 0.8f * variation * Random01());
            decay = Mathf.Exp(-1f / (ms * 0.001f * sampleRate));     // random length
        }
        float y = hip.Tick(Noise()) * env;
        env *= decay;
        return y * level * 2f;
    }
}
