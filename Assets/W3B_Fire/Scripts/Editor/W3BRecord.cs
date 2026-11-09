#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEditor.Recorder.Encoder;
using UnityEditor.SceneManagement;
using UnityEngine;

// =====================================================================
// W3B > Record ...  Records the demo (or the two fires) to an MP4 with
// sound, using Unity Recorder. Files go to the project's Recordings folder.
// =====================================================================
[InitializeOnLoad]
public static class W3BRecord
{
    const string Flag = "W3B_Recording";
    const string NameKey = "W3B_RecordingName";
    static RecorderController controller;

    static W3BRecord() => EditorApplication.playModeStateChanged += OnPlayMode;

    [MenuItem("W3B/6. Record Demo Walkthrough (video + sound)")]
    static void RecordDemo() => Begin("Assets/W3B_Fire/Scenes/W3B_Demo.unity", "W3B_Demo_Walkthrough");

    
    [MenuItem("W3B/9. Record Complete Reference (video + sound)")]
    static void RecordStudentExample() => Begin("Assets/W3B_Fire/Scenes/W3B_Complete_Reference.unity", "W3B_Complete_Reference_A_then_B");

    [MenuItem("W3B/11. Record Finale (video + sound)")]
    static void RecordFinale() => Begin("Assets/W3B_Fire/Scenes/W3B_Finale.unity", "W3B_Finale");

    static void Begin(string scenePath, string fileName)
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene(scenePath);
        EnsureAutopilot();
        EditorSceneManager.SaveOpenScenes();
        SessionState.SetBool(Flag, true);
        SessionState.SetString(NameKey, fileName);
        EditorApplication.isPlaying = true;
    }

    static void EnsureAutopilot()
    {
        var finale = Object.FindFirstObjectByType<FinaleController>();
        if (finale != null)
        {
            if (finale.GetComponent<FinaleAutopilot>() == null) finale.gameObject.AddComponent<FinaleAutopilot>();
            return;
        }
        var seq = Object.FindFirstObjectByType<DemoSequencer>();
        if (seq != null && seq.GetComponent<DemoAutopilot>() == null) seq.gameObject.AddComponent<DemoAutopilot>();

        var fires = Object.FindObjectsByType<FireController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (fires.Length >= 2 && Object.FindFirstObjectByType<FireAutopilot>(FindObjectsInactive.Include) == null)
        {
            var pilot = new GameObject("FireAutopilot").AddComponent<FireAutopilot>();
            foreach (var f in fires)
            {
                if (f.name.Contains("Recorded")) pilot.recordedFire = f;
                if (f.name.Contains("Procedural")) pilot.proceduralFire = f;
            }
            if (Camera.main != null) pilot.cameraToOrbit = Camera.main.transform;
        }
    }

    static void OnPlayMode(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Flag, false)) StartRecording();
        if (state == PlayModeStateChange.ExitingPlayMode && controller != null)
        {
            controller.StopRecording();
            controller = null;
            SessionState.SetBool(Flag, false);
            Debug.Log("W3B: recording saved in the project's Recordings folder.");
        }
    }

    static void StartRecording()
    {
        var settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        var movie = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        movie.name = "W3B Movie";
        movie.Enabled = true;
        movie.EncoderSettings = new CoreEncoderSettings
        {
            Codec = CoreEncoderSettings.OutputCodec.MP4,
            EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.High
        };
        movie.ImageInputSettings = new GameViewInputSettings { OutputWidth = 1920, OutputHeight = 1080 };
        movie.AudioInputSettings.PreserveAudio = true;
        var folder = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Recordings");
        Directory.CreateDirectory(folder);
        movie.OutputFile = Path.Combine(folder, SessionState.GetString(NameKey, "W3B_Recording"));

        settings.AddRecorderSettings(movie);
        settings.SetRecordModeToManual();
        settings.FrameRate = 30f;

        controller = new RecorderController(settings);
        controller.PrepareRecording();
        controller.StartRecording();
        Debug.Log("W3B: recording started.");
    }
}
#endif
