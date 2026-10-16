using UnityEngine;

// YOUR TASK: build the LAPPING layer: the low roar of the flames.
// Finished version (if you are stuck): Layers/FireLapping.cs
public class MyFireLapping : ProceduralSound
{
    float stage1, stage2;

    protected override float NextSample()
    {
        float n = Noise();

        // TODO 1: low-pass the noise hard, so only the rumble is left.
        float k = LowPassCoefficient(200f);   // try changing 200
        stage1 += k * (n - stage1);

        // TODO 2: a second stage makes it darker still.
        //   stage2 += k * (stage1 - stage2);   then return stage2 instead

        // TODO 3: low-passed noise is quiet, so boost it (x4 to x6),
        //         and use intensity for both level AND cutoff (bigger fire = brighter roar).

        // STRETCH: a slow random swell so it rises and falls like flame.
        return stage1 * 4f;
    }
}
