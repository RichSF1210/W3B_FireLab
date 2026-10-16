using UnityEngine;

// YOUR TASK: build the CRACKLE layer: tiny bursts of noise at random moments.
// Finished version (if you are stuck): Layers/FireCrackle.cs
public class MyFireCrackle : ProceduralSound
{
    float envelope;          // how loud the current crackle is right now
    float decay = 0.99f;     // how fast it dies (closer to 1 = longer)

    protected override float NextSample()
    {
        // TODO 1: decide how many crackles per second, using intensity.
        float perSecond = 10f;   // try: Mathf.Lerp(1f, 40f, intensity)

        // TODO 2: start a crackle at random. This is true about perSecond times a second:
        if (Random01() < perSecond / sampleRate)
        {
            envelope = 1f;       // TODO 3: random loudness, e.g. Mathf.Lerp(0.2f, 1f, Random01())
        }

        float output = Noise() * envelope;
        envelope *= decay;       // the crackle fades each sample

        // STRETCH: random decay per crackle (1-6 ms), or clusters of 2-3 crackles.
        return output;
    }
}
