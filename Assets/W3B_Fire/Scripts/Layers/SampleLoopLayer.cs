using UnityEngine;

// =====================================================================
// SampleLoopLayer  (PART A: RECORDED. No code needed.)
// USE FOR: a looping bed, e.g. your hiss or lapping recording.
// HOW: add to an object with an Audio Source, drag your loop into AudioClip.
// WHAT IS DYNAMIC HERE (the recording never changes, the playback does):
//   - starts at a random point, so it never lines up the same way twice
//   - intensity sets volume and pitch (bigger fire = louder, slightly higher)
//   - add an "Audio Low Pass Filter" component and intensity also makes it
//     brighter as the fire grows
// =====================================================================
[RequireComponent(typeof(AudioSource))]
public class SampleLoopLayer : IntensityLayer
{
    public Vector2 volumeRange = new Vector2(0.1f, 0.8f);  // at intensity 0 and 1
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    public Vector2 cutoffRange = new Vector2(1500f, 22000f);

    AudioSource source;
    AudioLowPassFilter lowPass;

    void Start()
    {
        source = GetComponent<AudioSource>();
        lowPass = GetComponent<AudioLowPassFilter>();
        if (source.clip == null) { Debug.LogWarning(name + ": no clip assigned"); return; }
        source.loop = true;
        source.time = Random.Range(0f, source.clip.length);
        source.Play();
    }

    void Update()
    {
        if (source == null) return;
        source.volume = Mathf.Lerp(volumeRange.x, volumeRange.y, intensity);
        source.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, intensity);
        if (lowPass != null) lowPass.cutoffFrequency = Mathf.Lerp(cutoffRange.x, cutoffRange.y, intensity);
    }
}
