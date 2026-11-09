using UnityEngine;   // gives us Unity's tools: Input, KeyCode, MonoBehaviour

// =====================================================================
// AddLogOnKey  (the finale: making the fire INTERACTIVE)
//
// WHAT IT DOES
//   Press L and a log goes on the fire: Intensity jumps up, then fades
//   back. Every layer follows, recorded or procedural.
//
// IN YOUR EXHIBIT
//   Call fire.AddLog() from a poke or a grab instead of a key:
//   XR Interactable > Interactable Events > Select Entered > AddLog.
// =====================================================================
public class AddLogOnKey : MonoBehaviour
{
    public FireController fire;   // drag the campfire here (A or B)

    // Update() runs once every frame: about 60 times a second.
    void Update()
    {
        // THE LINE: "if the player pressed L this frame, put a log on the fire."
        // Input.GetKeyDown is true only in the one frame the key goes down.
        if (Input.GetKeyDown(KeyCode.L)) fire.AddLog();
    }
}
