using UnityEngine;

// =====================================================================
// FireRecipe: Farnell's three elements of fire, in one readable script.
// (Andy Farnell, Designing Sound, 2010: the fire practical.)
//
// Fire is not one sound. Listen closely and you hear three things at once:
//
//   1 HISSING   gas escaping from the hot fuel. A constant, airy noise with
//               only high frequencies, flickering slightly in level.
//   2 CRACKLING sap and moisture boiling inside the wood until it bursts.
//               Short snaps at random moments, sometimes in quick clusters.
//   3 LAPPING   the flames themselves: hot air rising and tumbling. A deep,
//               low roar that swells and falls in irregular gusts.
//
// Each element below is only a few lines: NOISE, then a FILTER to choose
// which frequencies we keep, then an ENVELOPE to shape it over time.
// Three simple parts, and the ear completes the fire (remember Kanizsa).
//
// INTENSITY (0 = embers, 1 = blaze) is set by the FireController. Each
// element responds differently: that is real-time parameter control (RTPC).
// =====================================================================
public class FireRecipe : ProceduralSound
{
    public enum Solo { All, Hissing, Crackling, Lapping }
    [Tooltip("Listen to one element on its own.")]
    public Solo solo = Solo.All;

    [Range(0f, 2f)] public float hissing = 1f;
    [Range(0f, 2f)] public float crackling = 1f;
    [Range(0f, 2f)] public float lapping = 1f;

    // WHAT ONLY CODE CAN DO: change the fire itself, not just play it back.
    [Tooltip("0 = damp oak (few, dull snaps, more steam), 1 = dry pine (many sharp snaps).")]
    [Range(0f, 1f)] public float dryness = 0.5f;
    [Tooltip("0 = small campfire, 1 = bonfire (deeper, louder roar, slower gusts).")]
    [Range(0f, 1f)] public float size = 0.5f;

    // State the filters and envelopes remember between samples.
    float hissLow, flicker = 0.5f, flickerTarget = 0.5f;
    float snapEnvelope, snapDecay, ring1, ring2, coef1, coef2, ringGain;
    int clusterCountdown = -1;
    float roar1, roar2, gust;

    protected override float NextSample()
    {
        float i = intensity;

        // ---------- 1 HISSING ------------------------------------------
        float n = Noise();
        hissLow += LowPassCoefficient(3000f) * (n - hissLow);   // the lows of the noise...
        float hiss = n - hissLow;                                // ...taken away: only highs remain
        if (Random01() < 6f / sampleRate) flickerTarget = Random01();
        flicker += (flickerTarget - flicker) * 0.0004f;         // slow flicker in level
        hiss *= (0.3f + 0.7f * flicker) * Mathf.Lerp(0.04f, 0.26f, i);   // RTPC: louder when bigger
        hiss *= Mathf.Lerp(1.6f, 0.4f, dryness);                 // damp wood steams: more hiss

        // ---------- 2 CRACKLING ----------------------------------------
        float snapsPerSecond = Mathf.Lerp(1f, 30f, i) * Mathf.Lerp(0.3f, 1.7f, dryness);   // RTPC: more snaps when bigger or drier
        if (Random01() < snapsPerSecond / sampleRate || clusterCountdown == 0) NewSnap();
        if (clusterCountdown > 0) clusterCountdown--;
        // A burst of noise, rung through a resonant filter so each snap has a woody "tone".
        float o = ringGain * Noise() * snapEnvelope + coef1 * ring1 + coef2 * ring2;
        ring2 = ring1; ring1 = o;
        snapEnvelope *= snapDecay;                               // dies in a few milliseconds
        float crackle = o * 0.9f;

        // ---------- 3 LAPPING ------------------------------------------
        float cutoff = Mathf.Lerp(150f, 600f, i) * Mathf.Lerp(1.6f, 0.4f, size);   // RTPC: brighter when hotter, deeper when bigger
        float k = LowPassCoefficient(cutoff);
        roar1 += k * (Noise() - roar1);
        roar2 += k * (roar1 - roar2);                            // two low-passes: deep and dark
        gust += LowPassCoefficient(Mathf.Lerp(5f, 1f, size)) * (Noise() - gust);   // a slow random wind; slower for a big fire
        float swell = 0.35f + Mathf.Min(Mathf.Abs(gust) * 25f, 1.3f);
        float lap = roar2 * 6f * swell * Mathf.Lerp(0.25f, 1f, i) * Mathf.Lerp(0.4f, 1.6f, size);

        // ---------- MIX ------------------------------------------------
        hiss *= hissing; crackle *= crackling; lap *= lapping;
        if (solo == Solo.Hissing) return hiss;
        if (solo == Solo.Crackling) return crackle;
        if (solo == Solo.Lapping) return lap;
        return hiss + crackle + lap;
    }

    // Start one snap: random loudness, length and "wood tone"; 35% set off a second snap soon after.
    void NewSnap()
    {
        snapEnvelope = Mathf.Lerp(0.3f, 1f, Random01());
        float length = Mathf.Lerp(0.001f, 0.006f, Random01()) * Mathf.Lerp(1.8f, 0.5f, dryness);   // damp snaps are duller and longer
        snapDecay = Mathf.Exp(-1f / (length * sampleRate));
        float centre = Mathf.Lerp(Mathf.Lerp(600f, 2200f, dryness), Mathf.Lerp(1800f, 7000f, dryness), Random01())
                       * Mathf.Lerp(1.3f, 0.75f, size);                // bigger logs, lower tone
        float w = centre * 2f * Mathf.PI / sampleRate, r = 1f - w / 6f;
        coef1 = 2f * r * Mathf.Cos(w); coef2 = -r * r; ringGain = (1f - r) * 3f;
        clusterCountdown = Random01() < Mathf.Lerp(0.1f, 0.6f, dryness) ? (int)(Mathf.Lerp(0.01f, 0.05f, Random01()) * sampleRate) : -1;
    }
}
