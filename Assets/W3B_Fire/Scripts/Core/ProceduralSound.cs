using UnityEngine;

// =====================================================================
// ProceduralSound
// WHAT IT IS: the base for every sound made in code (no audio file).
// HOW IT WORKS:
//   Normally an Audio Source plays a .wav. Here we create an "empty" clip
//   and Unity asks us for the numbers (samples) as it plays, about 48,000
//   per second. Each child script (FireHiss, SineDemo...) only has to answer
//   one question in NextSample(): "what is the next sample?"
//   Because the sound still comes out of a normal Audio Source, Steam Audio
//   spatialises it exactly like a recording.
// HELPERS YOU CAN USE IN YOUR OWN SCRIPT:
//   Noise()                 white noise, a random number from -1 to 1
//   Random01()              a random number from 0 to 1
//   LowPassCoefficient(hz)  the number a simple low-pass filter needs
//   sampleRate              samples per second (usually 48000)
//   intensity               0 to 1, set by the FireController
// YOU DO NOT NEED TO EDIT THIS FILE.
// =====================================================================
[RequireComponent(typeof(AudioSource))]
public abstract class ProceduralSound : IntensityLayer
{
    [Range(0f, 1f)] public float gain = 0.5f;

    protected int sampleRate;
    protected System.Random rng;

    protected virtual void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
        rng = new System.Random();

        var source = GetComponent<AudioSource>();
        // Two seconds of "clip" that is never stored: Unity asks us for samples as it plays.
        source.clip = AudioClip.Create(GetType().Name, sampleRate * 2, 1, sampleRate, true, OnAudioRead);
        source.loop = true;
        source.Play();
    }

    // The last 4096 samples, kept so the demo can draw an oscilloscope.
    public readonly float[] scope = new float[4096];
    public int scopeWrite;

    void OnAudioRead(float[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            float v = Mathf.Clamp(NextSample() * gain, -1f, 1f);
            data[i] = v;
            scope[scopeWrite] = v;
            scopeWrite = (scopeWrite + 1) & 4095;
        }
    }

    public int SampleRate => sampleRate;

    // Return one sample between -1 and 1. Called around 48,000 times a second.
    protected abstract float NextSample();

    // White noise: every frequency at once.
    protected float Noise() => (float)(rng.NextDouble() * 2.0 - 1.0);

    protected float Random01() => (float)rng.NextDouble();

    // Coefficient for a one-pole low-pass filter at the given cutoff (Hz).
    protected float LowPassCoefficient(float cutoffHz) =>
        1f - Mathf.Exp(-2f * Mathf.PI * cutoffHz / sampleRate);
}
