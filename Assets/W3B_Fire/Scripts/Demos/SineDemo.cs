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
    [Range(50f, 2000f)] public float frequency = 440f;   // the note, in Hz (cycles a second). 440 = the note A
    [Range(1, 16)] public int bitDepth = 16;             // how finely each number is measured (up and down)
    [Range(1, 64)] public int holdSamples = 1;           // repeat each number N times = a lower sample rate (side to side)

    double phase;       // where we are in the wave: 0 to 2 pi (one full cycle)
    int held;           // how many more samples to repeat the current number
    float heldValue;    // the number being repeated

    protected override float NextSample()
    {
        phase += 2.0 * System.Math.PI * frequency / sampleRate;   // move along the wave by one sample
        if (phase > 2.0 * System.Math.PI) phase -= 2.0 * System.Math.PI;   // after one full cycle, start again
        float s = (float)System.Math.Sin(phase);                   // the smooth sine wave: a number from -1 to 1

        if (--held <= 0)                                           // time for a new number?
        {
            held = holdSamples;
            float steps = Mathf.Pow(2f, bitDepth - 1);             // steps each side of zero: 2 bits = 2, 16 bits = 32,768
            heldValue = Mathf.Round(s * steps) / steps;            // round to the nearest step: the staircase
        }
        return heldValue;                                          // the number Unity plays
    }
}
