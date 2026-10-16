using UnityEngine;

// =====================================================================
// IntensityLayer
// WHAT IT IS: the shared "plug" every fire layer has. One number, 0 to 1.
//   0 = embers, 1 = blaze. The FireController sets it on every layer.
// WHY: because every layer (recorded or coded) has the same plug, you can
//   mix and match them and one slider still controls the whole fire.
// YOU DO NOT NEED TO EDIT THIS FILE.
// =====================================================================
public abstract class IntensityLayer : MonoBehaviour
{
    [Range(0f, 1f)] public float intensity = 0.5f;
}
