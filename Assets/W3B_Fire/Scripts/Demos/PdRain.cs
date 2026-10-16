using UnityEngine;

// =====================================================================
// PdRain: Rich's Pure Data rain patch, rebuilt in C#, sample for sample.
// FOUR LAYERS (each can be turned up or down):
//   droplets - three voices. Slow random noise (bp~ 2 Hz) is raised to the
//              8th power, so only its rare peaks get through: sparse random
//              events. Each event gates a sine whose pitch wobbles with the
//              same noise: a short pitched "plink".
//   drips    - REBUILT from Farnell's physics (ch. 38): drips arrive at random
//              (Poisson) times; each is a tiny parabolic impact pulse plus, most
//              of the time, a bubble that rings at 1-3 kHz, rises in pitch and
//              dies in 10-45 ms. (The patch's original drips divided by an
//              almost-zero slow noise, which pinned the signal at its clip
//              limit 96% of the time, so a sharp filter whistled constantly.)
//   rumble   - noise around 333 Hz, swelling with the droplet noise: drops
//              hitting big things.
//   ambience - a little white noise: the distant wash.
// INTENSITY (0-1) raises the droplet noise before the 8th power, so a small
// change makes many more events: drizzle to downpour. This is the point of
// procedural audio: one number changes what the sound IS, not just its volume.
// =====================================================================
public class PdRain : ProceduralSound
{
    [Header("Layers")]
    [Range(0f, 2f)] public float droplets = 1f;
    [Range(0f, 2f)] public float drips = 1f;
    [Range(0f, 2f)] public float rumble = 1f;
    [Range(0f, 2f)] public float ambience = 1f;

    PdBp bp1, bp2, bp3, bpRumble;
    PdOsc o1, o2, o3;
    PdHip h1a, h1b, h2a, h2b, h3a, h3b;
    PdLop l1, l2, l3, master;
    PdHip dripHip; PdLop dripLop;
    const int DripVoices = 12;
    readonly int[] pulseLeft = new int[DripVoices], pulseLen = new int[DripVoices];
    readonly float[] dAmp = new float[DripVoices], dEnv = new float[DripVoices], dDecay = new float[DripVoices],
                     dAttack = new float[DripVoices], dFreq = new float[DripVoices], dRise = new float[DripVoices];
    readonly double[] dPhase = new double[DripVoices];
    readonly bool[] dBubble = new bool[DripVoices];
    int nextDrip;

    protected override void Start()
    {
        int sr = AudioSettings.outputSampleRate;
        bp1 = new PdBp(2.064f, 0.002f, sr); bp2 = new PdBp(2.064f, 0.002f, sr); bp3 = new PdBp(2.064f, 0.002f, sr);
        bpRumble = new PdBp(333f, 4f, sr);
        o1 = new PdOsc(); o2 = new PdOsc(); o3 = new PdOsc();
        h1a = new PdHip(500f, sr); h1b = new PdHip(500f, sr); h2a = new PdHip(500f, sr);
        h2b = new PdHip(500f, sr); h3a = new PdHip(500f, sr); h3b = new PdHip(500f, sr);
        l1 = new PdLop(10000f, sr); l2 = new PdLop(10000f, sr); l3 = new PdLop(10000f, sr);

        master = new PdLop(12000f, sr);
        dripHip = new PdHip(250f, sr); dripLop = new PdLop(9000f, sr);
        base.Start();
    }

    void StartDrip()
    {
        int v = nextDrip; nextDrip = (nextDrip + 1) % DripVoices;
        float gauss = (Random01() + Random01() + Random01()) / 3f;           // roughly bell-shaped loudness
        dAmp[v] = Mathf.Clamp01(gauss * 1.4f - 0.2f);
        pulseLen[v] = pulseLeft[v] = Mathf.Max(8, (int)(Mathf.Lerp(0.0005f, 0.0025f, Random01()) * sampleRate));
        dBubble[v] = Random01() < 0.7f;
        float radius = Mathf.Lerp(0.0011f, 0.0028f, Random01());              // metres
        dFreq[v] = 3.26f / radius;                                            // Minnaert: ~1.2-3 kHz
        float dur = Mathf.Lerp(0.012f, 0.045f, Random01());
        dDecay[v] = Mathf.Exp(-1f / (dur * sampleRate));
        dRise[v] = Mathf.Pow(1.6f, 1f / (dur * 4f * sampleRate));             // +60% over the ring
        dEnv[v] = 1f; dAttack[v] = 1f; dPhase[v] = 0;
    }

    static float SoftClip(float x) => x / (1f + Mathf.Abs(x));

    float Drop(float c, float spread, float centre, PdOsc osc, PdHip ha, PdHip hb, PdLop lo)
    {
        float e = c * c; e *= e; e *= e;                        // c to the 8th: sparse peaks
        float s = osc.Tick(centre + spread * c, sampleRate) * e * 0.3f;
        return lo.Tick(hb.Tick(ha.Tick(s)) * 0.01f);
    }

    protected override float NextSample()
    {
        if (master == null) return 0f;
        float n1 = Noise(), n2 = Noise(), n3 = Noise();

        float g = Mathf.Lerp(2.4f, 4.0f, intensity);            // 3.2 at intensity 0.5, as in the patch
        float c1 = bp1.Tick(n1) * g, c2 = bp2.Tick(n2) * g, c3 = bp3.Tick(n3) * g * 1.1875f;

        float drops = Drop(c1, 344f, 3000f, o1, h1a, h1b, l1) * 0.01f
                    + Drop(c2, 3000f, 2000f, o2, h2a, h2b, l2) * 0.02f
                    + Drop(c3, 5000f, 2000f, o3, h3a, h3b, l3) * 0.01f;

        float rum = bpRumble.Tick(n3 * c3) * 0.4f;
        float amb = n1 * 0.007f;

        if (Random01() < Mathf.Lerp(2f, 14f, intensity) / sampleRate) StartDrip();
        float drp = 0f;
        for (int v = 0; v < DripVoices; v++)
        {
            if (pulseLeft[v] > 0)                                   // impact: parabolic pressure pulse
            {
                float x = 1f - (float)pulseLeft[v] / pulseLen[v];
                drp += (1f - (2f * x - 1f) * (2f * x - 1f)) * dAmp[v] * 0.6f;
                pulseLeft[v]--;
            }
            if (dBubble[v] && dEnv[v] > 0.0005f)                    // bubble: rising, decaying sine
            {
                dPhase[v] += dFreq[v] / sampleRate;
                dFreq[v] *= dRise[v];
                drp += Mathf.Sin((float)(dPhase[v] * 6.283185307179586)) * dEnv[v] * (1f - dAttack[v]) * dAmp[v] * 0.5f;
                dEnv[v] *= dDecay[v];
                dAttack[v] *= 0.97f;
            }
        }
        drp = dripLop.Tick(dripHip.Tick(drp)) * 0.35f;

        float mix = drops * droplets * 2f
                  + drp * drips * Mathf.Lerp(0.4f, 1.4f, intensity)
                  + rum * rumble * Mathf.Lerp(0.3f, 1.5f, intensity)
                  + amb * ambience * Mathf.Lerp(0.5f, 1.5f, intensity);
        // Make-up gain (Pd's output is quiet), then a soft limiter so peaks never clip harshly.
        return SoftClip(master.Tick(mix) * 9f);
    }
}
