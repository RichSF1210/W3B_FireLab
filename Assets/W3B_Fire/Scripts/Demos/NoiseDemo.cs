using UnityEngine;

// DEMO 2: from order to chaos, then shaping it.
// Noise Mix 0 = pure sine, 1 = pure white noise.
// Cutoff shapes the noise: low = rumble, high = hiss (tick High Pass).
public class NoiseDemo : ProceduralSound
{
    [Range(50f, 2000f)] public float frequency = 440f;
    [Range(0f, 1f)] public float noiseMix = 0f;
    [Range(50f, 20000f)] public float cutoff = 20000f;
    public bool highPass = false;

    double phase;
    float lowPassed;

    protected override float NextSample()
    {
        phase += 2.0 * System.Math.PI * frequency / sampleRate;
        if (phase > 2.0 * System.Math.PI) phase -= 2.0 * System.Math.PI;
        float sine = (float)System.Math.Sin(phase);

        float noise = Noise();
        lowPassed += LowPassCoefficient(cutoff) * (noise - lowPassed);
        float shaped = highPass ? noise - lowPassed : lowPassed;

        return Mathf.Lerp(sine, shaped, noiseMix);
    }
}
