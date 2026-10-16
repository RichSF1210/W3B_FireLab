#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// One-click builders for W3 Lab B. Menu: W3B > ...
// Steam Audio is used if it is in the project (set up as in W2 Lab B); otherwise
// everything still works with Unity's own panning, and the Console says so.
public static class W3BSetup
{
    const string SceneFolder = "Assets/W3B_Fire/Scenes";

    // ---------- Menu items ----------

    [MenuItem("W3B/1. Use Steam Audio (project settings)")]
    public static void UseSteamAudio()
    {
        try
        {
            var steam = AudioSettings.GetSpatializerPluginNames().FirstOrDefault(n => n.Contains("Steam"));
            if (steam != null) { AudioSettings.SetSpatializerPluginName(steam); Debug.Log("W3B: spatializer set to " + steam); }
            else Debug.LogWarning("W3B: no Steam Audio spatializer found. Import SteamAudio.unitypackage first (W2 Lab B, steps 1-5).");
            // The ambisonic decoder is not needed for this lab (no ambisonic clips).
        }
        catch (Exception e) { Debug.LogWarning("W3B: could not set audio plugins: " + e.Message); }
    }

    [MenuItem("W3B/2. Build Demo Scene (Rich presents)")]
    public static void BuildDemoScene()
    {
        var scene = NewScene("W3B_Demo");
        MakeListener(new Vector3(0, 1.6f, -2f), false);
        var stepsRoot = new GameObject("Steps");
        var sequencer = new GameObject("DemoSequencer").AddComponent<DemoSequencer>();

        // 1. Frozen: one recording on a loop.
        var frozen = StepObject(stepsRoot, "1_Frozen");
        var fs = frozen.AddComponent<AudioSource>();
        fs.clip = FindClip("rain_static_3s_loop"); fs.loop = true; fs.playOnAwake = true; fs.spatialBlend = 0f;

        // 2. Thawed: recordings under rules.
        var thawed = StepObject(stepsRoot, "2_Thawed_Recordings");
        var rc = thawed.AddComponent<IntensityController>(); rc.intensity = 0.3f; rc.flicker = 0f;
        var bed = Child(thawed, "Bed"); var bs = bed.AddComponent<AudioSource>(); bs.spatialBlend = 0f; bs.playOnAwake = false;
        bs.clip = FindClip("rain_bed_loop");
        var bl = bed.AddComponent<SampleLoopLayer>(); bl.volumeRange = new Vector2(0.05f, 0.8f); bl.pitchRange = new Vector2(0.95f, 1.05f);
        var drops = Child(thawed, "Drops"); var ds = drops.AddComponent<AudioSource>(); ds.spatialBlend = 0f; ds.playOnAwake = false;
        var dl = drops.AddComponent<SampleOneShotLayer>();
        dl.clips = FindClips("rain_drop_"); dl.pitchRange = new Vector2(0.8f, 1.25f); dl.volumeRange = new Vector2(0.2f, 1f);
        dl.gapAtEmbers = new Vector2(0.3f, 1f); dl.gapAtBlaze = new Vector2(0.005f, 0.03f);

        // 3-5. Building blocks: one sine, digital numbers, noise and filters.
        var sine = StepObject(stepsRoot, "3_Sine"); Demo(sine).AddComponent<SineDemo>();
        var digital = StepObject(stepsRoot, "4_Digital"); var sd = Demo(digital).AddComponent<SineDemo>(); sd.frequency = 220f;
        var noise = StepObject(stepsRoot, "5_Noise"); Demo(noise).AddComponent<NoiseDemo>();

        // 6-9. Rich's Pure Data rain, built up layer by layer, then one slider.
        var drops1 = StepObject(stepsRoot, "6_Droplets");
        var p6 = Demo(drops1).AddComponent<PdRain>(); p6.drips = 0f; p6.rumble = 0f; p6.ambience = 0f;
        var drops2 = StepObject(stepsRoot, "7_Drips");
        var p7 = Demo(drops2).AddComponent<PdRain>(); p7.rumble = 0f; p7.ambience = 0f;
        var full = StepObject(stepsRoot, "8_Full_Rain");
        Demo(full).AddComponent<PdRain>();
        var storm = StepObject(stepsRoot, "9_One_Slider");
        var p9 = Demo(storm).AddComponent<PdRain>(); p9.intensity = 0.1f;

        // 10. The students' building blocks, ready to sculpt.
        var sculpt = StepObject(stepsRoot, "10_Your_Turn");
        var nl = Child(sculpt, "NoiseLayer"); Demo(nl).AddComponent<NoiseLayer>().cutoff = 150f;
        var el = Child(sculpt, "EventLayer"); var ev = Demo(el).AddComponent<EventLayer>(); ev.eventsPerSecond = 1f;

        // 11. The reveal: Farnell's three elements of fire, one at a time.
        var reveal = StepObject(stepsRoot, "11_Reveal_Fire");
        Demo(reveal).AddComponent<FireRecipe>();

        sequencer.steps = new[]
        {
            NewStep("Frozen: a rain recording on loop", "The ice cube. Listen for the seam: it repeats exactly.", frozen),
            NewStep("Thawed: recordings under rules", "The same kind of recordings: a bed loop and 12 single drops. Random choice, pitch, volume and timing. One slider: intensity. (Route 1)", thawed),
            NewStep("One sine wave", "Fourier: any sound can be built from sine waves. This is the building block.", sine),
            NewStep("Digital audio is numbers", "About 48,000 numbers a second. Fewer steps per number (bit depth) or fewer numbers (holding each one) and the smooth wave becomes a staircase. Listen to it get buzzy.", digital, true),
            NewStep("Noise, and shaping it", "Noise is every frequency at once. A filter keeps some: low-pass = rumble, high-pass = air.", noise),
            NewStep("Rain, layer 1: droplets", "Slow random noise, raised to the 8th power, lets only its rare peaks through. Each peak gates a short sine: a plink.", drops1),
            NewStep("Rain, layer 2: drips", "Spiky noise through a very sharp filter whose centre jumps around: bright, ringing drips.", drops2),
            NewStep("Rain, layers 3 and 4: rumble and ambience", "Noise around 333 Hz for drops on big surfaces, and a little white noise for the distant wash. The full patch.", full),
            NewStep("One slider: drizzle to storm", "Intensity changes WHAT the sound is, not just how loud: more events, more rumble. A recording cannot do this. (Route 2)", storm, true),
            NewStep("Your turn: sculpt by ear", "Two building blocks you will use: a noise layer and an event layer. Move the sliders and listen. What could each one become?", sculpt),
            NewStep("The reveal: three elements of fire", "Farnell: hissing (gas escaping), crackling (sap bursting), lapping (the flames' roar). Solo each one, then all three, then intensity. (Use at the listening share.)", reveal),
        };
        for (int i = 1; i < sequencer.steps.Length; i++) sequencer.steps[i].root.SetActive(false);
        Save(scene);
        Debug.Log("W3B: demo built. Press Play: right/left arrows step through, sliders are on screen.");
    }

    [MenuItem("W3B/3. Build Student Shell Scene (room + fire, no dynamic audio yet)")]
    public static void BuildStudentShell()
    {
        var scene = NewScene("W3B_Student_Shell");
        BuildRoom();
        MakeListener(new Vector3(0, 1.6f, -2.2f), true);
        BuildCampfire("Campfire", Vector3.zero, FireMode.Shell);
        ExportSteamGeometry();
        Save(scene);
        Debug.Log("W3B: shell built. Hiss, Crackle and Lapping have spatialised Audio Sources ready; students add the layer scripts.");
    }

    [MenuItem("W3B/4. Build Solution Scene (both finished fires)")]
    public static void BuildSolution()
    {
        var scene = NewScene("W3B_Solution");
        BuildRoom();
        MakeListener(new Vector3(0, 1.6f, -2.2f), true);
        var rec = BuildCampfire("Campfire_Recorded", Vector3.zero, FireMode.Recorded);
        var pro = BuildCampfire("Campfire_Procedural", Vector3.zero, FireMode.Procedural);
        pro.SetActive(false);
        Selection.activeGameObject = rec;
        ExportSteamGeometry();
        Save(scene);
        Debug.Log("W3B: two finished fires in the same place. Switch between them to A/B recorded and procedural.");
    }

    [MenuItem("W3B/5. Add Shell Fire To Current Scene (e.g. a W2 Lab B room)")]
    public static void AddFireToCurrentScene()
    {
        Vector3 pos = Vector3.zero;
        var view = SceneView.lastActiveSceneView;
        if (view != null && Physics.Raycast(view.camera.transform.position, view.camera.transform.forward, out var hit, 50f))
            pos = hit.point;
        Selection.activeGameObject = BuildCampfire("Campfire", pos, FireMode.Shell);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("W3B: fire added. If you moved or added room geometry, run Steam Audio > Export Active Scene again.");
    }

    [MenuItem("W3B/8. Build Student Example Scene (what a finished student fire sounds like)")]
    public static void BuildStudentExample()
    {
        var scene = NewScene("W3B_Student_Example");
        BuildRoom();
        MakeListener(new Vector3(0, 1.6f, -2.2f), true);

        // Route 1 result: recordings, varied (what MyRandomPlayer + the pitch line gives).
        var rec = BuildCampfire("Campfire_Recorded_Route1", Vector3.zero, FireMode.Shell);
        var r1 = rec.transform.Find("Layer 1").gameObject; r1.GetComponent<AudioSource>().clip = FindClip("fire_hiss_loop"); r1.AddComponent<SampleLoopLayer>();
        var r2 = rec.transform.Find("Layer 2").gameObject; r2.AddComponent<SampleOneShotLayer>().clips = FindClips("fire_crackle_");
        var r3 = rec.transform.Find("Layer 3").gameObject; r3.GetComponent<AudioSource>().clip = FindClip("fire_lapping_loop"); r3.AddComponent<SampleLoopLayer>();

        // Route 2 result: sculpted from the two building blocks, plus the link.
        var pro = BuildCampfire("Campfire_Procedural_Route2", Vector3.zero, FireMode.Shell);
        var hiss = pro.transform.Find("Layer 1").gameObject.AddComponent<NoiseLayer>();
        hiss.filter = NoiseLayer.FilterType.HighPass; hiss.cutoff = 4000f; hiss.wobbleDepth = 0.5f; hiss.wobbleSpeed = 3f; hiss.level = 0.2f;
        var crackle = pro.transform.Find("Layer 2").gameObject.AddComponent<EventLayer>();
        crackle.eventsPerSecond = 8f; crackle.eventLength = 3f; crackle.brightness = 2500f; crackle.variation = 0.7f; crackle.level = 0.6f;
        var roar = pro.transform.Find("Layer 3").gameObject.AddComponent<NoiseLayer>();
        roar.filter = NoiseLayer.FilterType.LowPass; roar.cutoff = 300f; roar.wobbleDepth = 0.6f; roar.wobbleSpeed = 0.8f; roar.level = 0.7f;
        var link = pro.AddComponent<ExampleDynamicLink>();
        link.fire = pro.GetComponent<FireController>(); link.crackleLayer = crackle; link.hissLayer = hiss; link.roarLayer = roar;
        pro.SetActive(false);

        ExportSteamGeometry();
        Save(scene);
        Debug.Log("W3B: student example built. Campfire_Recorded_Route1 is on; switch to Campfire_Procedural_Route2 to compare.");
    }

    // ---------- Fire ----------

    enum FireMode { Shell, Recorded, Procedural }

    static GameObject BuildCampfire(string name, Vector3 pos, FireMode mode)
    {
        var root = new GameObject(name);
        root.transform.position = pos;
        var controller = root.AddComponent<FireController>();

        var visuals = new GameObject("Visuals");
        visuals.transform.SetParent(root.transform, false);
        for (int i = 0; i < 4; i++)
        {
            var log = Prop(visuals, PrimitiveType.Cylinder, "Log", new Color(0.25f, 0.15f, 0.08f));
            // Logs lean inwards like a tipi: tilt 30 degrees, sit 0.18 m out from the centre.
            log.transform.localScale = new Vector3(0.08f, 0.3f, 0.08f);
            log.transform.localRotation = Quaternion.Euler(0f, i * 90f, 0f) * Quaternion.Euler(-30f, 0f, 0f);
            log.transform.localPosition = Quaternion.Euler(0f, i * 90f, 0f) * new Vector3(0f, 0.26f, 0.15f);
        }
        for (int i = 0; i < 10; i++)
        {
            var stone = Prop(visuals, PrimitiveType.Sphere, "Stone", new Color(0.4f, 0.4f, 0.4f));
            stone.transform.localScale = new Vector3(0.18f, 0.12f, 0.18f);
            stone.transform.localPosition = Quaternion.Euler(0, i * 36f, 0) * new Vector3(0, 0.05f, 0.5f);
        }

        var lightGo = Child(visuals, "FireLight");
        lightGo.transform.localPosition = new Vector3(0, 0.6f, 0);
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Point; light.color = new Color(1f, 0.55f, 0.2f); light.range = 6f;
        controller.fireLight = light;

        var flamesGo = Child(visuals, "Flames");
        flamesGo.transform.localPosition = new Vector3(0, 0.2f, 0);
        flamesGo.transform.localRotation = Quaternion.Euler(-90f, 0, 0);
        var flames = flamesGo.AddComponent<ParticleSystem>();
        var main = flames.main;
        main.startLifetime = 1.0f; main.startSpeed = 0.5f; main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
        main.startColor = new Color(1f, 0.5f, 0.1f, 0.8f);
        var shape = flames.shape; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 8f; shape.radius = 0.12f;
        var fade = flames.colorOverLifetime; fade.enabled = true;
        var g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(new Color(1f, 0.85f, 0.3f), 0f), new GradientColorKey(new Color(1f, 0.35f, 0.05f), 0.5f), new GradientColorKey(new Color(0.3f, 0.05f, 0f), 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.9f, 0.15f), new GradientAlphaKey(0f, 1f) });
        fade.color = g;
        var shrink = flames.sizeOverLifetime; shrink.enabled = true; shrink.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.2f));
        var flameRenderer = flamesGo.GetComponent<ParticleSystemRenderer>();
        flameRenderer.sharedMaterial = FlameMaterial();
        controller.flames = flames;
        controller.emissionRange = new Vector2(15f, 120f);

        // Students get neutral names: working out what the layers are is part of the task.
        bool shell = mode == FireMode.Shell;
        var hiss = Layer(root, shell ? "Layer 1" : "Hiss");
        var crackle = Layer(root, shell ? "Layer 2" : "Crackle");
        var lapping = Layer(root, shell ? "Layer 3" : "Lapping");
        if (mode == FireMode.Procedural)
        {
            hiss.AddComponent<FireHiss>(); crackle.AddComponent<FireCrackle>(); lapping.AddComponent<FireLapping>();
        }
        else if (mode == FireMode.Recorded)
        {
            hiss.GetComponent<AudioSource>().clip = FindClip("fire_hiss_loop"); hiss.AddComponent<SampleLoopLayer>();
            lapping.GetComponent<AudioSource>().clip = FindClip("fire_lapping_loop"); lapping.AddComponent<SampleLoopLayer>();
            crackle.AddComponent<SampleOneShotLayer>().clips = FindClips("fire_crackle_");
        }
        return root;
    }

    static GameObject Layer(GameObject root, string name)
    {
        var go = Child(root, name);
        go.transform.localPosition = new Vector3(0, 0.3f, 0);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        ConfigureSpatial(go, src);
        return go;
    }

    // ---------- Room ----------

    static void BuildRoom()
    {
        // A small room, 6 x 6 x 3 m, already set up for Steam Audio as in W2 Lab B.
        var room = new GameObject("Room");
        Block(room, "Floor", new Vector3(0, -0.05f, 0), new Vector3(6, 0.1f, 6), new Color(0.35f, 0.3f, 0.25f));
        Block(room, "Ceiling", new Vector3(0, 3.05f, 0), new Vector3(6, 0.1f, 6), new Color(0.2f, 0.2f, 0.2f));
        Block(room, "Wall_N", new Vector3(0, 1.5f, 3), new Vector3(6, 3, 0.2f), new Color(0.5f, 0.45f, 0.4f));
        Block(room, "Wall_S", new Vector3(0, 1.5f, -3), new Vector3(6, 3, 0.2f), new Color(0.5f, 0.45f, 0.4f));
        Block(room, "Wall_E", new Vector3(3, 1.5f, 0), new Vector3(0.2f, 3, 6), new Color(0.5f, 0.45f, 0.4f));
        Block(room, "Wall_W", new Vector3(-3, 1.5f, 0), new Vector3(0.2f, 3, 6), new Color(0.5f, 0.45f, 0.4f));

        var geometry = AddByName(room, "SteamAudio.SteamAudioGeometry");
        if (geometry != null)
        {
            Set(geometry, "exportAllChildren", true);
            var mat = FindSteamMaterial("Brick", "Concrete", "Rock");
            if (mat != null) Set(geometry, "material", mat);
        }

        var sun = new GameObject("Dim Light").AddComponent<Light>();
        sun.type = LightType.Directional; sun.intensity = 0.15f;
        sun.transform.rotation = Quaternion.Euler(50, -30, 0);
    }

    // An additive, soft-edged particle material, saved as an asset so scenes keep it.
    static Material FlameMaterial()
    {
        EnsureFolder();
        const string path = SceneFolder + "/W3B_Flame.mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;
        var shader = Shader.Find("Legacy Shaders/Particles/Additive") ?? Shader.Find("Particles/Standard Unlit");
        var mat = new Material(shader) { name = "W3B_Flame" };
        var tex = AssetDatabase.GetBuiltinExtraResource<Texture2D>("Default-Particle.psd");
        if (tex != null) mat.mainTexture = tex;
        if (mat.HasProperty("_TintColor")) mat.SetColor("_TintColor", new Color(1f, 0.55f, 0.15f, 0.8f));
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    static UnityEngine.Object FindSteamMaterial(params string[] preferred)
    {
        var type = FindType("SteamAudio.SteamAudioMaterial");
        if (type == null) return null;
        var all = AssetDatabase.FindAssets("t:" + type.Name)
            .Select(g => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(g), type)).Where(o => o != null).ToList();
        foreach (var p in preferred)
        {
            var hit = all.FirstOrDefault(o => o.name.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);
            if (hit != null) return hit;
        }
        return all.FirstOrDefault();
    }

    static void Block(GameObject parent, string name, Vector3 pos, Vector3 size, Color colour)
    {
        var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
        b.name = name;
        b.transform.SetParent(parent.transform, false);
        b.transform.localPosition = pos;
        b.transform.localScale = size;
        Tint(b, colour);
    }

    static void ExportSteamGeometry()
    {
        var meshType = FindType("SteamAudio.SteamAudioStaticMesh");
        var dataType = FindType("SteamAudio.SerializedData");
        if (meshType == null || dataType == null) return;

        // Pre-create the data asset so Steam Audio does not stop to ask where to save it.
        EnsureFolder();
        var sceneName = SceneManager.GetActiveScene().name;
        var path = AssetDatabase.GenerateUniqueAssetPath(SceneFolder + "/" + sceneName + "_SteamAudio.asset");
        var data = ScriptableObject.CreateInstance(dataType);
        AssetDatabase.CreateAsset(data, path);

        var holder = new GameObject("Steam Audio Static Mesh");
        var mesh = holder.AddComponent(meshType);
        Set(mesh, "asset", data);
        Set(mesh, "sceneNameWhenExported", sceneName);

        if (!EditorApplication.ExecuteMenuItem("Steam Audio/Export Active Scene"))
            Debug.LogWarning("W3B: run Steam Audio > Export Active Scene yourself.");
        else
            Debug.Log("W3B: Steam Audio geometry exported to " + path);
    }

    static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/W3B_Fire")) AssetDatabase.CreateFolder("Assets", "W3B_Fire");
        if (!AssetDatabase.IsValidFolder(SceneFolder)) AssetDatabase.CreateFolder("Assets/W3B_Fire", "Scenes");
    }

    // ---------- Helpers ----------

    static Scene NewScene(string name)
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) throw new OperationCanceledException();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = name;
        return scene;
    }

    static void Save(Scene scene)
    {
        EnsureFolder();
        EditorSceneManager.SaveScene(scene, SceneFolder + "/" + scene.name + ".unity");
        Debug.Log("W3B: saved " + SceneFolder + "/" + scene.name + ".unity. Press Play and listen.");
    }

    static void MakeListener(Vector3 pos, bool canWalk)
    {
        var cam = new GameObject("Main Camera");
        cam.tag = "MainCamera";
        cam.transform.position = pos;
        if (canWalk) cam.transform.rotation = Quaternion.Euler(25f, 0f, 0f); // look down at the fire
        cam.AddComponent<Camera>();
        cam.AddComponent<AudioListener>();
        if (canWalk) cam.AddComponent<DesktopWalker>();
    }

    static GameObject StepObject(GameObject parent, string name) => Child(parent, name);

    // Demo sounds play straight to the speakers (no 3D), so the room hears them clearly.
    static GameObject Demo(GameObject step)
    {
        var src = step.AddComponent<AudioSource>();
        src.spatialBlend = 0f; src.playOnAwake = false;
        return step;
    }

    static DemoSequencer.Step NewStep(string title, string hint, GameObject root, bool extraControls = false) =>
        new DemoSequencer.Step { title = title, hint = hint, root = root, showExtraControls = extraControls };

    static GameObject Child(GameObject parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        return go;
    }

    static GameObject Prop(GameObject parent, PrimitiveType shape, string name, Color colour)
    {
        var go = GameObject.CreatePrimitive(shape);
        go.name = name;
        UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        go.transform.SetParent(parent.transform, false);
        Tint(go, colour);
        return go;
    }

    static AudioClip[] FindClips(string prefix) =>
        AssetDatabase.FindAssets(prefix + " t:AudioClip")
            .Select(g => AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(g)))
            .Where(c => c != null && c.name.StartsWith(prefix)).OrderBy(c => c.name).ToArray();

    static void ConfigureSpatial(GameObject go, AudioSource src)
    {
        src.spatialBlend = 1f;
        src.spatialize = true;
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.minDistance = 1f;
        src.maxDistance = 20f;
        var steam = AddByName(go, "SteamAudio.SteamAudioSource");
        if (steam != null)
        {
            Set(steam, "occlusion", true);
            Set(steam, "transmission", true);
            Set(steam, "reflections", true);
        }
    }

    static AudioClip FindClip(string name)
    {
        var guid = AssetDatabase.FindAssets(name + " t:AudioClip").FirstOrDefault();
        return guid == null ? null : AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guid));
    }

    static void Tint(GameObject go, Color c)
    {
        var r = go.GetComponent<Renderer>();
        if (r == null || r.sharedMaterial == null) return;
        var m = new Material(r.sharedMaterial) { color = c };
        r.sharedMaterial = m;
    }

    static Type FindType(string fullName)
    {
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            var t = a.GetType(fullName);
            if (t != null) return t;
        }
        return null;
    }

    static Component AddByName(GameObject go, string fullName)
    {
        var t = FindType(fullName);
        if (t == null) { Debug.LogWarning("W3B: " + fullName + " not found. Is Steam Audio imported?"); return null; }
        var existing = go.GetComponent(t);
        return existing != null ? existing : go.AddComponent(t);
    }

    static void Set(object target, string member, object value)
    {
        var type = target.GetType();
        var f = type.GetField(member, BindingFlags.Public | BindingFlags.Instance);
        if (f != null && f.FieldType.IsAssignableFrom(value.GetType())) { f.SetValue(target, value); return; }
        var p = type.GetProperty(member, BindingFlags.Public | BindingFlags.Instance);
        if (p != null && p.CanWrite && p.PropertyType.IsAssignableFrom(value.GetType())) { p.SetValue(target, value); return; }
        Debug.LogWarning("W3B: could not set " + type.Name + "." + member + ". Tick it by hand in the Inspector.");
    }
}
#endif
