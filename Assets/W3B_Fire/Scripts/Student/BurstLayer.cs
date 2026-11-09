using UnityEngine;
using UnityEngine.Serialization;

// =====================================================================
// BurstLayer  (PART B: PROCEDURAL, a ready-made building block)
//
// WHAT IT IS
//   Very short bursts of noise, at random moments. No audio file at all:
//   every burst is calculated from numbers as the sound plays.
//   Think of anything made of many tiny, separate events: a snap, a drip,
//   a click, a tick. You decide which, BY EAR, with the sliders.
//
// THE SLIDERS (in the Inspector)
//   Bursts Per Second : how often a burst happens, on average
//   Burst Length      : how long each burst rings, in milliseconds
//   Brightness        : a high-pass filter. Higher = thinner, sharper
//   Variation         : how different each burst is (0 = all identical)
//   Level             : volume
//
// TRY
//   Short + bright + frequent, then long + dark + rare. What does each
//   remind you of? Does either match a part of the mystery sound?
//
// MAKING IT DYNAMIC: see MyDynamicLink.cs (your one line of code).
// =====================================================================
public class BurstLayer : ProceduralSound          // ProceduralSound does the audio plumbing for us
{
    [FormerlySerializedAs("burstsPerSecond")]
    [Range(0.2f, 80f)] public float burstsPerSecond = 8f;   // average bursts each second
    [FormerlySerializedAs("burstLength")]
    [Range(0.5f, 50f)] public float burstLength = 4f;       // milliseconds each burst lasts
    [Range(100f, 12000f)] public float brightness = 2000f;  // high-pass filter cutoff, in Hz
    [Range(0f, 1f)] public float variation = 0.6f;          // 0 = every burst the same, 1 = all different
    [Range(0f, 1f)] public float level = 0.6f;              // volume

    PdHip hip;       // the high-pass filter (a copy of Pure Data's hip~ object)
    float env;       // the burst's current loudness: jumps up, then dies away
    float decay;     // how much env shrinks each sample (sets the burst length)
    int counter;     // counts samples, so we only update the filter now and then

    protected override void Start()
    {
        hip = new PdHip(brightness, AudioSettings.outputSampleRate);   // make the filter
        base.Start();                                                  // then start the sound
    }

    // Unity asks for the next sample about 48,000 times a second.
    protected override float NextSample()
    {
        if (hip == null) return 0f;                                    // not ready yet: silence
        if ((counter++ & 63) == 0) hip.Set(brightness, sampleRate);    // follow the slider (every 64 samples)

        // Is it time for a new burst? Roll a dice every sample:
        // a chance of burstsPerSecond / sampleRate gives that many per second, at random moments.
        if (Random01() < burstsPerSecond / sampleRate)                   // 4 RANDOM: when
        {
            env = 1f - variation * Random01();                         // 2 ENVELOPE: instant attack, at a random loudness
            float ms = burstLength * (1f - 0.8f * variation * Random01());   // a random length
            decay = Mathf.Exp(-1f / (ms * 0.001f * sampleRate));       // turn that length into a decay rate
        }

        float y = hip.Tick(Noise()) * env;   // 1 SOURCE (noise), 3 FILTER (brightness), shaped by 2 ENVELOPE
        env *= decay;                        // 2 ENVELOPE: the burst dies away a little every sample (exponential decay)
        return y * level * 2f;               // set the volume
    }
}
