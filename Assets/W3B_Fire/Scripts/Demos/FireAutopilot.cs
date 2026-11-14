using UnityEngine;

// =====================================================================
// FireAutopilot
// STUDENTS: you do not need this script. It was used to record the backup
// videos: it presses the buttons and moves the sliders on a timer, by itself.
// Plays the Solution scene by itself for recording: the recorded fire,
// then the generated fire, each rising from embers to a blaze while the
// camera circles the fire so you hear Steam Audio move the sound.
// =====================================================================
public class FireAutopilot : MonoBehaviour
{
    public bool runOnPlay = false;
    public FireController recordedFire;
    public FireController proceduralFire;
    public Transform cameraToOrbit;
    public float secondsPerFire = 22f;

    float t;

    void Start()
    {
        bool recording = false;
#if UNITY_EDITOR
        recording = UnityEditor.SessionState.GetBool("W3B_Recording", false);
#endif
        enabled = runOnPlay || recording;
        if (enabled && recordedFire != null) { recordedFire.gameObject.SetActive(true); proceduralFire.gameObject.SetActive(false); }
    }

    void Update()
    {
        t += Time.deltaTime;
        bool second = t > secondsPerFire;
        if (t > secondsPerFire * 2f)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            enabled = false; return;
        }
        if (second && !proceduralFire.gameObject.activeSelf)
        {
            recordedFire.gameObject.SetActive(false);
            proceduralFire.gameObject.SetActive(true);
        }
        var fire = second ? proceduralFire : recordedFire;
        float p = (t % secondsPerFire) / secondsPerFire;
        fire.intensity = Mathf.Sin(p * Mathf.PI) * 0.9f + 0.05f;

        if (cameraToOrbit != null)
        {
            // Face the playing fire squarely, so it sits in the centre of the picture and
            // the sound is balanced left to right. A slow, small, symmetric sway (left, centre,
            // right, centre) lets Steam Audio move the sound gently without pulling it to one side.
            Vector3 f = fire.transform.position;
            float sway = Mathf.Sin(p * Mathf.PI * 2f) * 0.6f;
            cameraToOrbit.position = new Vector3(f.x + sway, 1.5f, f.z - 2.6f);
            cameraToOrbit.LookAt(new Vector3(f.x, 0.4f, f.z));
        }
    }
}
