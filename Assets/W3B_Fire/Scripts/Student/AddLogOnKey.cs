using UnityEngine;

// =====================================================================
// AddLogOnKey  (the finale: making the fire INTERACTIVE)
// Press L and a log goes on the fire: Intensity jumps up, then fades.
// Every layer follows, recorded or generated.
// In your exhibit, call fire.AddLog() from a poke or grab instead
// (XR Interactable > Interactable Events > Select Entered > AddLog).
// =====================================================================
public class AddLogOnKey : MonoBehaviour
{
    public FireController fire;   // drag the Campfire here

    void Update()
    {
        // THE LINE: when the player presses L, put a log on the fire.
        if (Input.GetKeyDown(KeyCode.L)) fire.AddLog();
    }
}
