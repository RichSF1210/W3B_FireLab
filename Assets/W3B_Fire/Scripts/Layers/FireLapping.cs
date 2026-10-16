using UnityEngine;

// =====================================================================
// FireLapping  (CODE route, finished version)
// FARNELL'S LAYER 3: LAPPING, the low roar of moving flame.
// RECIPE: white noise -> keep only the lows (two low-pass filters) ->
//   a slow random swell so it breathes.
// WHAT IS DYNAMIC HERE: intensity raises the level AND opens the filter,
//   so a bigger fire is brighter as well as louder. A recording cannot do
//   that without extra processing.
// =====================================================================
public class FireLapping : ProceduralSound
{
    float stage1, stage2, swell, swellTarget;
    int counter;

    protected override float NextSample()
    {
        float cutoff = Mathf.Lerp(120f, 400f, intensity);
        float k = LowPassCoefficient(cutoff);
        stage1 += k * (Noise() - stage1);
        stage2 += k * (stage1 - stage2);                  // two stages = steeper, darker

        if (--counter <= 0) { swellTarget = Random01(); counter = sampleRate / 3; }
        swell += 0.0002f * (swellTarget - swell);

        float level = Mathf.Lerp(0.2f, 1f, intensity);
        return stage2 * 6f * level * (0.4f + 0.6f * swell);  // x6: low-passed noise is quiet
    }
}
