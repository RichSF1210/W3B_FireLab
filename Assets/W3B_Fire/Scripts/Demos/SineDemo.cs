using UnityEngine;

// =====================================================================
// SineDemo: one sine wave. Fourier's building block.
// DIGITAL AUDIO IS JUST NUMBERS: about 48,000 of them every second.
//   Bit Depth     - how many steps each number can take. 16 bits = 65,536
//                   steps (smooth). 1 or 2 bits = a crude staircase (buzzy).
//   Hold Samples  - keep each number for this many samples, as if the sample
//                   rate were lower. High values = a coarse, gritty staircase.
// =====================================================================
public class SineDemo : ProceduralSound
{
    [Range(50f, 2000f)] public float frequency = 440f;
    [Range(1, 16)] public int bitDepth = 16;
    [Range(1, 64)] public int holdSamples = 1;

    double phase;
    int held;
    float heldValue;

    protected override float NextSample()
    {
        phase += 2.0 * System.Math.PI * frequency / sampleRate;
        if (phase > 2.0 * System.Math.PI) phase -= 2.0 * System.Math.PI;
        float s = (float)System.Math.Sin(phase);

        if (--held <= 0)
        {
            held = holdSamples;
            float steps = Mathf.Pow(2f, bitDepth - 1);
            heldValue = Mathf.Round(s * steps) / steps;
        }
        return heldValue;
    }
}
