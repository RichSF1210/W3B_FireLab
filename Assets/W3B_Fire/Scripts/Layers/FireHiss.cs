using UnityEngine;

// =====================================================================
// FireHiss  (CODE route, finished version)
// FARNELL'S LAYER 1: HISSING, gas escaping from the burning fuel.
// RECIPE: white noise -> remove the lows (high-pass) -> slow wobble in level.
// WHAT IS DYNAMIC HERE: it is generated live, so it never repeats; intensity
//   sets the level; Brightness and Flutter can be changed while it plays.
// =====================================================================
public class FireHiss : ProceduralSound
{
    [Range(500f, 8000f)] public float brightness = 2500f;  // high-pass cutoff
    [Range(0f, 1f)] public float flutter = 0.5f;

    float lowPassed, wobble, wobbleTarget;
    int counter;

    protected override float NextSample()
    {
        float n = Noise();
        lowPassed += LowPassCoefficient(brightness) * (n - lowPassed);
        float hiss = n - lowPassed;                       // keep only the highs

        // Slow random wobble: pick a new target about 8 times a second, glide to it.
        if (--counter <= 0) { wobbleTarget = Random01(); counter = sampleRate / 8; }
        wobble += 0.0005f * (wobbleTarget - wobble);

        float level = Mathf.Lerp(0.1f, 0.7f, intensity);
        return hiss * level * (1f - flutter + flutter * wobble);
    }
}
