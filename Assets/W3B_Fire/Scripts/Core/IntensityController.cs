using UnityEngine;

// =====================================================================
// IntensityController (the FireController uses this)
// WHAT IT DOES: holds ONE intensity value and sends it to every layer
//   underneath it, so hiss, crackle and lapping rise and fall together.
// WHAT IS DYNAMIC HERE:
//   Flicker   - a slow random wobble, so the fire is never perfectly still
//   AddLog()  - INTERACTIVE: a boost that fades (hook it to a grab or poke)
//   Burn Down - ADAPTIVE: the fire slowly dies over time
//   Light and particles follow the same number, so sound and visuals agree.
// =====================================================================
public class IntensityController : MonoBehaviour
{
    [Range(0f, 1f)] public float intensity = 0.5f;

    [Header("Life: random flicker")]
    [Range(0f, 0.5f)] public float flicker = 0.1f;

    [Header("Interaction: AddLog() gives a boost that fades")]
    public float logBoost = 0.4f;
    public float boostFadePerSecond = 0.05f;

    [Header("Time: burn down")]
    public bool burnDown = false;
    public float burnDownPerSecond = 0.01f;

    [Header("Optional visuals")]
    public Light fireLight;
    public Vector2 lightRange = new Vector2(0.5f, 3f);
    public ParticleSystem flames;
    public Vector2 emissionRange = new Vector2(5f, 60f);

    public float CurrentIntensity { get; private set; }

    IntensityLayer[] layers;
    float boost;

    void OnEnable() => layers = GetComponentsInChildren<IntensityLayer>(true);

    // Call from a poke or grab event (Inspector: When Select -> AddLog),
    // or right-click the component header -> Add Log while playing.
    [ContextMenu("Add Log")]
    public void AddLog() => boost = Mathf.Min(boost + logBoost, 1f);

    void Update()
    {
        if (burnDown) intensity = Mathf.Max(0f, intensity - burnDownPerSecond * Time.deltaTime);
        boost = Mathf.Max(0f, boost - boostFadePerSecond * Time.deltaTime);

        float wobble = (Mathf.PerlinNoise(Time.time * 0.7f, 0f) - 0.5f) * 2f * flicker;
        CurrentIntensity = Mathf.Clamp01(intensity + boost + wobble);

        foreach (var layer in layers) layer.intensity = CurrentIntensity;

        if (fireLight != null) fireLight.intensity = Mathf.Lerp(lightRange.x, lightRange.y, CurrentIntensity);
        if (flames != null)
        {
            var emission = flames.emission;
            emission.rateOverTimeMultiplier = Mathf.Lerp(emissionRange.x, emissionRange.y, CurrentIntensity);
        }
    }
}
