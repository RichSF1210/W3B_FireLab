#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// =====================================================================
// W3B > Walkthrough: reproduces what a student does, one step at a time,
// selecting the object so the Inspector shows the result. Used to make the
// worksheet screenshots and the run-through video. Nothing is saved.
// =====================================================================
public static class W3BWalkthrough
{
    const string Shell = "Assets/W3B_Fire/Scenes/W3B_Student_Shell.unity";

    static GameObject Fire() => GameObject.Find("Campfire");
    static GameObject Layer(int n) => Fire().transform.Find("Layer " + n).gameObject;
    // Select the object; fold the set-up components so the student's new component is in view.
    static void Show(GameObject go)
    {
        foreach (var c in go.GetComponents<Component>())
        {
            string n = c.GetType().Name;
            bool fold = n == "Transform" || n == "AudioSource" || n == "SteamAudioSource" || n == "FireController";
            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(c, !fold);
        }
        Selection.activeGameObject = go;
        EditorGUIUtility.PingObject(go);
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }

    static AudioClip[] Clips(string prefix) => AssetDatabase.FindAssets(prefix + " t:AudioClip")
        .Select(g => AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(g)))
        .Where(c => c != null && c.name.StartsWith(prefix)).OrderBy(c => c.name).ToArray();

    [MenuItem("W3B/Walkthrough/1 Open the empty shell")]
    static void S1() { EditorSceneManager.OpenScene(Shell); Show(Fire()); }

    [MenuItem("W3B/Walkthrough/2 An empty layer")]
    static void S2() { Show(Layer(2)); }

    [MenuItem("W3B/Walkthrough/3 Part A: MyRandomPlayer with clips")]
    static void S3()
    {
        var l = Layer(2);
        var p = l.GetComponent<MyRandomPlayer>(); if (p == null) p = l.AddComponent<MyRandomPlayer>();
        p.clips = Clips("fire_crackle_");
        Show(l);
    }

    [MenuItem("W3B/Walkthrough/4 Part A: SampleLoopLayer with a loop")]
    static void S4()
    {
        var l = Layer(1);
        l.GetComponent<AudioSource>().clip = Clips("fire_hiss_loop").FirstOrDefault();
        if (l.GetComponent<SampleLoopLayer>() == null) l.AddComponent<SampleLoopLayer>();
        Show(l);
    }

    [MenuItem("W3B/Walkthrough/5 Part B: NoiseLayer")]
    static void S5()
    {
        EditorSceneManager.OpenScene(Shell);
        var n = Layer(3).AddComponent<NoiseLayer>();
        n.filter = NoiseLayer.FilterType.LowPass; n.cutoff = 300f; n.wobbleDepth = 0.6f; n.wobbleSpeed = 0.8f; n.level = 0.7f;
        Show(Layer(3));
    }

    [MenuItem("W3B/Walkthrough/6 Part B: BurstLayer")]
    static void S6()
    {
        var e = Layer(2).GetComponent<BurstLayer>(); if (e == null) e = Layer(2).AddComponent<BurstLayer>();
        e.burstsPerSecond = 8f; e.burstLength = 3f; e.brightness = 2500f; e.variation = 0.7f;
        Show(Layer(2));
    }

    [MenuItem("W3B/Walkthrough/7 Part B: MyDynamicLink on the Campfire")]
    static void S7()
    {
        var f = Fire();
        var link = f.GetComponent<MyDynamicLink>(); if (link == null) link = f.AddComponent<MyDynamicLink>();
        link.fire = f.GetComponent<FireController>();
        link.burstLayer = Layer(2).GetComponent<BurstLayer>();
        link.noiseLayer = Layer(3).GetComponent<NoiseLayer>();
        Show(f);
    }
}
#endif
