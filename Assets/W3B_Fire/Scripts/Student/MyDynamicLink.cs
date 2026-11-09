using UnityEngine;   // gives us Unity's tools: Mathf, MonoBehaviour and so on

// =====================================================================
// MyDynamicLink  (PART B: PROCEDURAL. Your one line of code.)
//
// WHAT IT DOES
//   Connects the fire's Intensity (0 = embers, 1 = blaze) to one of
//   your sliders, so the sound changes as the fire changes. This is
//   RTPC: Real-Time Parameter Control. A value from the game (here,
//   Intensity) drives a parameter of the sound (here, how many bursts).
//   Game audio engines such as Wwise and FMOD are built around this idea.
//
// HOW TO USE IT
//   1 Select Campfire B. Add Component > MyDynamicLink.
//   2 Drag Campfire B into "Fire". Drag the layer with your BurstLayer into
//     "Burst Layer", and the one with your NoiseLayer into "Noise Layer".
//   3 Find the TODO below. Delete the two slashes // in front of ONE line.
//     Save. Press Play, select Campfire B and drag Intensity from 0 to 1.
//
// THE KEY IDEA: Mathf.Lerp(a, b, i)
//   "Lerp" is short for linear interpolation: go from a to b as i goes 0 to 1.
//   Mathf.Lerp(1, 40, 0)   = 1      (embers: 1 burst a second)
//   Mathf.Lerp(1, 40, 0.5) = 20.5   (half way)
//   Mathf.Lerp(1, 40, 1)   = 40     (blaze: 40 bursts a second)
// =====================================================================
public class MyDynamicLink : MonoBehaviour      // MonoBehaviour = a script that lives on an object
{
    public FireController fire;      // drag Campfire B here: it holds the Intensity value
    public BurstLayer burstLayer;    // drag the layer that has your BurstLayer here
    public NoiseLayer noiseLayer;    // drag the layer that has your NoiseLayer here

    // Update() runs once every frame: about 60 times a second.
    void Update()
    {
        if (fire == null) return;            // nothing dragged into "Fire" yet: do nothing

        float i = fire.CurrentIntensity;     // read the fire's intensity now: a number from 0 to 1
                                             // ("float" means a decimal number)

        // TODO: YOUR ONE LINE. Delete the two slashes at the start of ONE of these lines.

        // burstLayer.burstsPerSecond = Mathf.Lerp(1f, 40f, i);
        //   A bigger fire makes more bursts: 1 a second at embers, 40 at a blaze.

        // noiseLayer.cutoff = Mathf.Lerp(200f, 2000f, i);
        //   A bigger fire is brighter: the noise filter opens from 200 Hz to 2,000 Hz.

        // Then press Play and move Intensity. What changes? Is it more than the volume?
        // STRETCH: use both lines, or change the numbers and listen to what happens.
    }
}
