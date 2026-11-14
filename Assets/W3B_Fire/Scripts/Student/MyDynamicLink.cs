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
//   1 Select Campfire B in the Hierarchy. Add Component > MyDynamicLink.
//   2 Fill the slots BEFORE pressing Play (drag from the Hierarchy):
//       Fire        : Campfire B (it holds the Intensity value)
//       Burst Layer : the layer that has your BurstLayer
//       Noise Layer : the layer that has your NoiseLayer
//   3 Find the TODO below. Pick ONE line to start: A or B.
//     Delete the two slashes // at the start of that line only.
//   4 Save (Ctrl+S or Cmd+S). Go back to Unity and wait for it to compile
//     the C# code. Press Play, select Campfire B, drag Intensity from 0 to 1.
//   5 Then try the other line instead. STRETCH: both lines at once.
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

    // Start() runs once, when you press Play. It checks the slots and tells you,
    // in the Console, if one is empty. A plain warning, not a red error.
    void Start()
    {
        if (fire == null)
            Debug.LogWarning("MyDynamicLink: the Fire slot is empty. Stop Play, drag Campfire B into Fire.");
        if (burstLayer == null)
            Debug.LogWarning("MyDynamicLink: the Burst Layer slot is empty. Fine if you only use line B. For line A, drag the layer with your BurstLayer into Burst Layer.");
        if (noiseLayer == null)
            Debug.LogWarning("MyDynamicLink: the Noise Layer slot is empty. Fine if you only use line A. For line B, drag the layer with your NoiseLayer into Noise Layer.");
    }

    // Update() runs once every frame: about 60 times a second.
    // So the sound follows the Intensity slider all the time, not just once.
    void Update()
    {
        if (fire == null) return;            // nothing dragged into "Fire" yet: do nothing

        float i = fire.CurrentIntensity;     // read the fire's intensity now: a number from 0 to 1
                                             // ("float" means a decimal number)

        // TODO: YOUR ONE LINE. Pick A or B. Delete the two slashes // at the start of that line.
        // ("if (burstLayer)" means "only if a layer is in the Burst Layer slot".)

        // A) MORE SNAPS as the fire grows: 1 burst a second at embers, 40 at a blaze.
        // if (burstLayer) burstLayer.burstsPerSecond = Mathf.Lerp(1f, 40f, i);

        // B) A BRIGHTER FIRE as it grows: the noise filter opens from 200 Hz to 2,000 Hz.
        // if (noiseLayer) noiseLayer.cutoff = Mathf.Lerp(200f, 2000f, i);

        // Then press Play and move Intensity. What changes? Is it more than the volume?
        // Then try the other line. STRETCH: use both lines, or change the numbers and listen.
    }
}
