using UnityEngine;

// =====================================================================
// MyRandomPlayer  (RECORDINGS route: YOUR ONE LINE)
// Plays your short recordings at random moments.
// HOW: put this on a layer object, drag your clips into Clips, press Play.
// Listen: it repeats. Now write ONE line so no two plays sound the same.
// =====================================================================
[RequireComponent(typeof(AudioSource))]
public class MyRandomPlayer : IntensityLayer
{
    public AudioClip[] clips;
    public float shortestGap = 0.1f;   // seconds
    public float longestGap = 1.0f;

    AudioSource source;
    float nextTime;

    void Start() => source = GetComponent<AudioSource>();

    void Update()
    {
        if (clips == null || clips.Length == 0 || Time.time < nextTime) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        // TODO: ONE line. Give every play a slightly different pitch, e.g.
        // source.pitch = Random.Range(0.85f, 1.2f);

        source.PlayOneShot(clip);
        nextTime = Time.time + Random.Range(shortestGap, longestGap);
    }
}
