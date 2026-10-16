using UnityEngine;

// YOUR TASK: build the HISS layer of the fire from white noise.
// Finished version (if you are stuck): Layers/FireHiss.cs
public class MyFireHiss : ProceduralSound
{
    [Range(500f, 8000f)] public float brightness = 2500f;

    float lowPassed;

    protected override float NextSample()
    {
        float n = Noise();

        // TODO 1: low-pass the noise.
        //   lowPassed += LowPassCoefficient(brightness) * (n - lowPassed);

        // TODO 2: noise minus its lows leaves only the highs. That is your hiss.
        float hiss = n; // change this to: n - lowPassed

        // TODO 3: make it louder as the fire grows.
        //   multiply by Mathf.Lerp(0.1f, 0.7f, intensity)

        // STRETCH: make the level wobble slowly so the flame "breathes".
        return hiss;
    }
}
