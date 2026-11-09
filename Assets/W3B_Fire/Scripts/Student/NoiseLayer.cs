using UnityEngine;

// =====================================================================
// NoiseLayer  (PART B: PROCEDURAL, a ready-made building block)
//
// WHAT IT IS
//   White noise (every frequency at once, like radio static) through one
//   filter, with a slow random wobble in its level. No audio file at all:
//   it is calculated from numbers as it plays, so it never repeats.
//   Use it for any CONSTANT part of a sound: a roar, a hiss, a rumble.
//
// THE SLIDERS (in the Inspector)
//   Filter       : Low Pass keeps the lows, High Pass keeps the highs,
//                  Band Pass keeps a band in between
//   Cutoff       : where the filter cuts, in Hz (20 = very low, 15,000 = very high)
//   Resonance    : Band Pass only. Higher = a narrower, more whistling band
//   Wobble Depth : how much the level moves up and down (0 = steady)
//   Wobble Speed : how fast it moves, in wobbles per second
//   Level        : volume
//
// TRY
//   Low Pass with a low cutoff: rumble. High Pass with a high cutoff: air.
//
// MAKING IT DYNAMIC: see MyDynamicLink.cs (your one line of code).
// =====================================================================
public class NoiseLayer : ProceduralSound          // ProceduralSound does the audio plumbing for us
{
    public enum FilterType { LowPass, HighPass, BandPass }   // the three filter choices
    public FilterType filter = FilterType.LowPass;
    [Range(20f, 15000f)] public float cutoff = 800f;     // Hz
    [Range(0.5f, 20f)] public float resonance = 2f;      // band-pass only: higher = narrower
    [Range(0f, 1f)] public float wobbleDepth = 0.3f;     // how much the level moves
    [Range(0.1f, 10f)] public float wobbleSpeed = 1f;    // how fast, in wobbles per second
    [Range(0f, 1f)] public float level = 0.5f;           // volume

    // Three filters, copies of Pure Data's lop~ (low-pass), hip~ (high-pass) and bp~ (band-pass).
    PdLop lop; PdHip hip; PdBp bp;
    PdLop smooth;                 // (kept for compatibility)
    float wobble, target;         // the wobble glides from where it is towards a random target
    int counter;                  // counts samples, so we only update the filters now and then

    protected override void Start()
    {
        int sr = AudioSettings.outputSampleRate;                       // samples per second (usually 48,000)
        lop = new PdLop(cutoff, sr); hip = new PdHip(cutoff, sr); bp = new PdBp(cutoff, resonance, sr);
        smooth = new PdLop(3f, sr);
        base.Start();                                                  // start the sound
    }

    // Unity asks for the next sample about 48,000 times a second.
    protected override float NextSample()
    {
        if (smooth == null) return 0f;                                 // not ready yet: silence
        if ((counter++ & 63) == 0)                                     // every 64 samples, follow the sliders
        { lop.Set(cutoff, sampleRate); hip.Set(cutoff, sampleRate); bp.Set(cutoff, resonance, sampleRate); }

        float n = Noise();                                             // one random number from -1 to 1: white noise
        float y = filter == FilterType.LowPass ? lop.Tick(n) * 3f      // keep the lows (boosted, as lows are quieter)
                : filter == FilterType.HighPass ? hip.Tick(n)          // keep the highs
                : bp.Tick(n) * 2f;                                     // keep a band

        if (Random01() < wobbleSpeed / sampleRate) target = Random01();   // now and then, pick a new random target
        wobble += (target - wobble) * (wobbleSpeed * 4f / sampleRate);    // glide smoothly towards it
        float amp = 1f - wobbleDepth + wobbleDepth * wobble;              // turn the wobble into a volume
        return y * level * amp;                                           // filtered noise, at that volume
    }
}
