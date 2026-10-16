using UnityEngine;

// =====================================================================
// MyDynamicLink  (CODE route: YOUR ONE LINE)
// Your layers sound right. Now make them ALIVE: connect the fire's
// Intensity (0 = embers, 1 = blaze) to one of your sliders.
// HOW: put this on the Campfire, drag your layer into the slot, then
// write ONE line inside Update().
// =====================================================================
public class MyDynamicLink : MonoBehaviour
{
    public FireController fire;      // drag the Campfire here
    public EventLayer eventLayer;    // drag a layer here (or a NoiseLayer below)
    public NoiseLayer noiseLayer;

    void Update()
    {
        if (fire == null) return;
        float i = fire.CurrentIntensity;   // 0 to 1, changes as the fire changes

        // TODO: ONE line. Choose a slider and let intensity drive it, e.g.
        // eventLayer.eventsPerSecond = Mathf.Lerp(1f, 40f, i);
        // noiseLayer.cutoff = Mathf.Lerp(200f, 2000f, i);
        // Then press Play and move Intensity. What changes? Is it more than volume?
    }
}
