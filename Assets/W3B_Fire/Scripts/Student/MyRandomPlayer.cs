using UnityEngine;   // gives us Unity's tools: AudioSource, Random, Time and so on

// =====================================================================
// MyRandomPlayer  (PART A: RECORDED. Your one line of code.)
//
// WHAT IT DOES
//   Plays your short recordings (snaps, pops, drips...) one at a time,
//   at random moments. Use it for any part of a sound made of separate
//   events. For a constant part, use SampleLoopLayer instead (no code).
//
// HOW TO USE IT
//   1 Under Campfire A, select a layer (e.g. Layer 2). Add Component > MyRandomPlayer.
//   2 Click the padlock at the top right of the Inspector (it stays on this layer).
//     In the Project window, select ALL your short files (click the first,
//     Shift-click the last) and drag them onto the word "Clips" (the heading,
//     not a row). The list fills, one row per file. Click the padlock again.
//   3 Press Play and listen. It repeats: every play sounds identical.
//   4 Find the TODO below. Delete the two slashes // in front of the
//     line under it. Save (Cmd+S or Ctrl+S). Go back to Unity and wait for it
//     to compile the C# code. Press Play again.
//
// WHERE THE SHAPE COMES FROM
//   In a recording, the shape is baked in: the snap you recorded is the snap
//   you get. All code can change is HOW it is played back: which file, when,
//   how loud, at what pitch. That is the limit of Part A, and the point of Part B.
//
// WHY THIS MATTERS
//   The recordings never change, but now the PLAYBACK never repeats.
//   This is how most game audio hides that it is made of a few files.
// =====================================================================

// [RequireComponent] makes sure this object always has an Audio Source
// to play through. (Ours already does, set up for Steam Audio.)
[RequireComponent(typeof(AudioSource))]
public class MyRandomPlayer : IntensityLayer    // IntensityLayer = it can be part of the fire
{
    // "public" means the setting appears in the Inspector for you to fill in.
    public AudioClip[] clips;              // [] means a LIST of clips: drag yours in here
    public float shortestGap = 0.1f;       // the shortest wait between plays, in seconds
    public float longestGap = 1.0f;        // the longest wait between plays, in seconds

    AudioSource source;                    // the speaker this object plays through
    float nextTime;                        // the time (in seconds) of the next play

    // Start() runs once, when you press Play.
    // It finds the Audio Source on this object and remembers it.
    void Start() => source = GetComponent<AudioSource>();

    // Update() runs once every frame: about 60 times a second.
    void Update()
    {
        // If there are no clips yet, or it is not time to play, stop here for this frame.
        if (clips == null || clips.Length == 0 || Time.time < nextTime) return;

        // Pick one clip from the list at random.
        // Random.Range(0, clips.Length) gives a whole number from 0 up to (but not including) the list length.
        AudioClip clip = clips[Random.Range(0, clips.Length)];

        // TODO: YOUR ONE LINE. Delete the two slashes at the start of the next line.
        // source.pitch = Random.Range(0.85f, 1.2f);
        //   What it does: sets the playback pitch to a random number between 0.85 and 1.2.
        //   1.0 is normal. 0.85 is slightly lower and slower; 1.2 is slightly higher and faster.
        //   The "f" after each number tells C# it is a decimal number (a "float").

        source.PlayOneShot(clip);          // play the chosen clip once (it can overlap the last one)

        // Choose a random wait before the next play, and remember when that is.
        nextTime = Time.time + Random.Range(shortestGap, longestGap);
    }
}
