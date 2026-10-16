using UnityEngine;

// =====================================================================
// NoiseLayer  (CODE route: a ready-made building block)
// WHAT IT IS: white noise (every frequency at once) through one filter,
//   with a slow wobble in its level. Sculpt it BY EAR with the sliders.
// TRY: Low Pass with a low cutoff = rumble. High Pass with a high cutoff =
//   a breathy, airy sound. Band Pass = a band in between.
// MAKING IT DYNAMIC: see MyDynamicLink.cs (one line of code).
// =====================================================================
public class NoiseLayer : ProceduralSound
{
    public enum FilterType { LowPass, HighPass, BandPass }
    public FilterType filter = FilterType.LowPass;
    [Range(20f, 15000f)] public float cutoff = 800f;     // Hz
    [Range(0.5f, 20f)] public float resonance = 2f;      // band-pass only: higher = narrower
    [Range(0f, 1f)] public float wobbleDepth = 0.3f;     // how much the level moves
    [Range(0.1f, 10f)] public float wobbleSpeed = 1f;    // how fast (times per second)
    [Range(0f, 1f)] public float level = 0.5f;

    PdLop lop; PdHip hip; PdBp bp; PdLop smooth;
    float wobble, target; int counter;

    protected override void Start()
    {
        int sr = AudioSettings.outputSampleRate;
        lop = new PdLop(cutoff, sr); hip = new PdHip(cutoff, sr); bp = new PdBp(cutoff, resonance, sr);
        smooth = new PdLop(3f, sr);
        base.Start();
    }

    protected override float NextSample()
    {
        if (smooth == null) return 0f;
        if ((counter++ & 63) == 0)                                  // update filters 750 times a second
        { lop.Set(cutoff, sampleRate); hip.Set(cutoff, sampleRate); bp.Set(cutoff, resonance, sampleRate); }

        float n = Noise();
        float y = filter == FilterType.LowPass ? lop.Tick(n) * 3f
                : filter == FilterType.HighPass ? hip.Tick(n)
                : bp.Tick(n) * 2f;

        if (Random01() < wobbleSpeed / sampleRate) target = Random01();   // new random target
        wobble += (target - wobble) * (wobbleSpeed * 4f / sampleRate);   // glide towards it
        float amp = 1f - wobbleDepth + wobbleDepth * wobble;
        return y * level * amp;
    }
}
