using UnityEngine;

// =====================================================================
// SampleOneShotLayer  (RECORDINGS route: no code needed)
// USE FOR: short one-off sounds, e.g. your crackles or pops.
// HOW: add to an object with an Audio Source, drag several clips into Clips.
// WHAT IS DYNAMIC HERE (this is how game audio hides repetition):
//   - picks a random clip, and never the same one twice in a row
//   - random pitch and volume on every play, so no two sound identical
//   - random gaps between plays; higher intensity = more often
// =====================================================================
[RequireComponent(typeof(AudioSource))]
public class SampleOneShotLayer : IntensityLayer
{
    public AudioClip[] clips;
    public Vector2 pitchRange = new Vector2(0.85f, 1.2f);
    public Vector2 volumeRange = new Vector2(0.3f, 1f);
    public Vector2 gapAtEmbers = new Vector2(0.5f, 2f);    // seconds between plays, intensity 0
    public Vector2 gapAtBlaze = new Vector2(0.03f, 0.25f); // seconds between plays, intensity 1

    AudioSource source;
    float nextTime;
    int lastIndex = -1;

    void Start() => source = GetComponent<AudioSource>();

    void Update()
    {
        if (clips == null || clips.Length == 0 || Time.time < nextTime) return;

        int i;
        do { i = Random.Range(0, clips.Length); }
        while (clips.Length > 1 && i == lastIndex);
        lastIndex = i;

        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.PlayOneShot(clips[i], Random.Range(volumeRange.x, volumeRange.y));

        float minGap = Mathf.Lerp(gapAtEmbers.x, gapAtBlaze.x, intensity);
        float maxGap = Mathf.Lerp(gapAtEmbers.y, gapAtBlaze.y, intensity);
        nextTime = Time.time + Random.Range(minGap, maxGap);
    }
}
