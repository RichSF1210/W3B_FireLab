using UnityEngine;

// =====================================================================
// PdDsp: small filters that behave like their Pure Data namesakes.
// DSP = digital signal processing: making or changing sound with maths,
// one number (sample) at a time. A filter is DSP.
//   PdLop  = lop~   (low-pass: keeps the lows)
//   PdHip  = hip~   (high-pass: keeps the highs)
//   PdBp   = bp~    (band-pass: keeps a band around a centre frequency)
//   PdVcf  = vcf~   (a resonant band-pass whose centre can move)
//   PdOsc  = osc~   (a cosine oscillator: a sine wave)
// Each has Tick(x): put one sample in, get one sample out.
// YOU DO NOT NEED TO EDIT THIS FILE.
// =====================================================================
public class PdLop
{
    float k, s;
    public PdLop(float hz, int sr) => Set(hz, sr);
    public void Set(float hz, int sr) => k = Mathf.Clamp01(hz * 6.2831853f / sr);
    public float Tick(float x) { s = k * x + (1f - k) * s; return s; }
}

public class PdHip
{
    float c, norm, last;
    public PdHip(float hz, int sr) => Set(hz, sr);
    public void Set(float hz, int sr) { c = Mathf.Clamp01(1f - hz * 6.2831853f / sr); norm = 0.5f * (1f + c); }
    public float Tick(float x) { float n = x + c * last; float y = (n - last) * norm; last = n; return y; }
}

public class PdBp
{
    float c1, c2, g, last, prev;
    public PdBp(float hz, float q, int sr) => Set(hz, q, sr);
    public void Set(float hz, float q, int sr)
    {
        float om = hz * 6.2831853f / sr;
        float omr = q < 0.001f ? 1f : Mathf.Min(om / q, 1f);
        float r = 1f - omr;
        c1 = 2f * QCos(om) * r; c2 = -r * r; g = 2f * omr * (omr + r * om);
    }
    static float QCos(float f)
    {
        if (f < -1.5707963f || f > 1.5707963f) return 0f;
        float g = f * f;
        return ((g * g * g * (-1f / 720f) + g * g * (1f / 24f)) - g * 0.5f) + 1f;
    }
    public float Tick(float x) { float o = x + c1 * last + c2 * prev; prev = last; last = o; return g * o; }
}

public class PdVcf
{
    float re, im;
    public float Tick(float x, float centreHz, float q, int sr)
    {
        float w = centreHz * 6.2831853f / sr;
        float r = Mathf.Max(1f - w / q, 0f), omr = 1f - r, amp = 2f - 2f / (q + 2f);
        float cr = r * Mathf.Cos(w), ci = r * Mathf.Sin(w), re2 = re;
        re = amp * omr * x + cr * re2 - ci * im;
        im = ci * re2 + cr * im;
        return re;
    }
}

public class PdOsc
{
    double phase;
    public float Tick(float hz, int sr)
    {
        phase += hz / sr;
        phase -= System.Math.Floor(phase);
        return (float)System.Math.Cos(phase * 6.283185307179586);
    }
}
