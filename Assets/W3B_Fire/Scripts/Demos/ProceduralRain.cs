using UnityEngine;

// ProceduralRain: an earlier, simpler rain made from noise and sine waves.
// Not used in the W3B_Demo scene now (PdRain replaced it). Kept as a short,
// readable example you can put on a layer and try.
// DEMO 3: rain from noise and sine waves, driven by one Intensity slider.
// Two layers:
//   bed   - band-limited noise: the wash of distant rain
//   drops - each drop is a short sine "plink" that rises slightly in pitch
//           and dies away (the Minnaert bubble: frequency ~ 3.26 / radius in metres)
// Intensity sets how many drops fall per second and how loud the bed is.
public class ProceduralRain : ProceduralSound
{
    [Range(0f, 1f)] public float bedLevel = 0.6f;
    [Range(0f, 1f)] public float dropLevel = 0.5f;
    [Range(0f, 1f)] public float dropSize = 0.5f;  // 0 = tiny high plinks, 1 = big low plonks
    [Range(0f, 1f)] public float surface = 0.5f;   // 0 = soft (dull bed, short drops), 1 = hard (bright bed, ringing drops)
    public float minDropsPerSecond = 3f;    // at intensity 0 (drizzle)
    public float maxDropsPerSecond = 600f;  // at intensity 1 (downpour)

    const int Voices = 48;
    readonly float[] freq = new float[Voices];
    readonly float[] phase = new float[Voices];
    readonly float[] amp = new float[Voices];
    readonly float[] decay = new float[Voices];
    readonly float[] chirp = new float[Voices];
    int nextVoice;
    float bedLow, bedHigh;

    protected override float NextSample()
    {
        // Bed: noise between roughly 400 Hz and 6 kHz.
        float n = Noise();
        float bedTop = Mathf.Lerp(2500f, 12000f, surface);          // harder surface = brighter wash
        bedHigh += LowPassCoefficient(bedTop) * (n - bedHigh);
        bedLow += LowPassCoefficient(400f) * (bedHigh - bedLow);
        float bed = (bedHigh - bedLow) * bedLevel * intensity;

        // New drop? From minDropsPerSecond (drizzle) to maxDropsPerSecond (downpour).
        float dropsPerSecond = Mathf.Lerp(minDropsPerSecond, maxDropsPerSecond, intensity * intensity);
        if (Random01() < dropsPerSecond / sampleRate) StartDrop();

        float drops = 0f;
        for (int v = 0; v < Voices; v++)
        {
            if (amp[v] < 0.0005f) continue;
            phase[v] += 2f * Mathf.PI * freq[v] / sampleRate;
            if (phase[v] > 2f * Mathf.PI) phase[v] -= 2f * Mathf.PI;
            drops += Mathf.Sin(phase[v]) * amp[v];
            amp[v] *= decay[v];
            freq[v] *= chirp[v];
        }

        return bed + drops * dropLevel;
    }

    void StartDrop()
    {
        int v = nextVoice;
        nextVoice = (nextVoice + 1) % Voices;

        float smallest = Mathf.Lerp(0.0004f, 0.002f, dropSize);  // bubble radius in metres
        float largest = Mathf.Lerp(0.0012f, 0.006f, dropSize);
        float radius = Mathf.Lerp(smallest, largest, Random01());
        freq[v] = 3.26f / radius;                                 // Minnaert: smaller bubble = higher pitch
        phase[v] = 0f;
        amp[v] = Mathf.Lerp(0.05f, 0.4f, Random01());
        float decaySeconds = Mathf.Lerp(0.004f, 0.025f, Random01()) * Mathf.Lerp(0.5f, 2f, surface);
        decay[v] = Mathf.Exp(-1f / (decaySeconds * sampleRate));
        chirp[v] = 1f + 2f / sampleRate;                          // gentle upward sweep
    }
}
