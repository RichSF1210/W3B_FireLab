using UnityEngine;

// DEMO 2: from order to chaos, then shaping it.
// Noise Mix 0 = pure sine, 1 = pure white noise.
// Cutoff shapes the noise: low = rumble, high = hiss (tick High Pass).
public class NoiseDemo : ProceduralSound
{
    [Range(50f, 2000f)] public float frequency = 440f;
    [Range(0f, 1f)] public float noiseMix = 0f;         // 0 = pure sine, 1 = pure noise
    [Range(50f, 20000f)] public float cutoff = 20000f;  // where the filter cuts, in Hz
    public bool highPass = false;                        // off = keep the lows (rumble); on = keep the highs (hiss)

    double phase;
    float lowPassed;

    protected override float NextSample()
    {
        phase += 2.0 * System.Math.PI * frequency / sampleRate;
        if (phase > 2.0 * System.Math.PI) phase -= 2.0 * System.Math.PI;
        float sine = (float)System.Math.Sin(phase);

        float noise = Noise();                                   // white noise: every frequency at once
        lowPassed += LowPassCoefficient(cutoff) * (noise - lowPassed);   // a low-pass filter (DSP): smooths away the highs
        float shaped = highPass ? noise - lowPassed : lowPassed;   // high-pass = the noise minus its lows

        return Mathf.Lerp(sine, shaped, noiseMix);             // blend from sine to shaped noise
    }
}
