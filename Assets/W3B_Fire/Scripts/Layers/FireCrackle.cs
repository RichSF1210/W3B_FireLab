using UnityEngine;

// =====================================================================
// FireCrackle  (CODE route, finished version)
// FARNELL'S LAYER 2: CRACKLING, sap and fuel bursting.
// RECIPE: at random moments, a very short burst of noise that dies fast
//   (1 to 6 milliseconds). Sometimes one crackle sets off another (clusters).
// WHAT IS DYNAMIC HERE: every crackle has a new loudness and length, timing
//   is random, and intensity sets how many per second (1 to 40).
// =====================================================================
public class FireCrackle : ProceduralSound
{
    [Range(0f, 1f)] public float clustering = 0.4f;   // chance a crackle sets off another

    float envelope, decay, lowPassed;
    int clusterDelay = -1;

    protected override float NextSample()
    {
        // From 1 crackle a second (embers) to 40 (blaze).
        float perSecond = Mathf.Lerp(1f, 40f, intensity);
        if (Random01() < perSecond / sampleRate) Spark();

        if (clusterDelay > 0 && --clusterDelay == 0) Spark();

        float n = Noise();
        lowPassed += LowPassCoefficient(1500f) * (n - lowPassed);
        float sharp = n - lowPassed;                      // crisp, not thuddy

        float output = sharp * envelope;
        envelope *= decay;
        return output;
    }

    void Spark()
    {
        envelope = Mathf.Lerp(0.2f, 1f, Random01());
        float seconds = Mathf.Lerp(0.001f, 0.006f, Random01());   // 1 to 6 ms
        decay = Mathf.Exp(-1f / (seconds * sampleRate));
        clusterDelay = Random01() < clustering
            ? (int)(sampleRate * Mathf.Lerp(0.01f, 0.06f, Random01()))
            : -1;
    }
}
