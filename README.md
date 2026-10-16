# W3B FireLab: Sound Design and Dynamic Audio

COMSM0127 Immersive Interaction and Audio Design, Week 3 Lab B (University of Bristol).
Unity **6000.3.19f1**, spatialised with **Steam Audio 4.7.0**.

## Open the project

1. Clone or download this repository.
2. Unity Hub: **Add > Add project from disk**, choose the folder, open in **6000.3.19f1**.
3. Import Steam Audio exactly as in Week 2 Lab B: download `steamaudio_unity_4.7.0.zip` from the
   [Steam Audio releases](https://github.com/ValveSoftware/steam-audio/releases), then
   **Assets > Import Package > Custom Package > SteamAudio.unitypackage**.
4. Run **W3B > 1. Use Steam Audio**.

## Scenes (`Assets/W3B_Fire/Scenes`)

| Scene | Purpose |
|---|---|
| `W3B_Demo` | The lecturer's demo: 11 steps, arrow keys, on-screen sliders and oscilloscope |
| `W3B_Student_Shell` | Students start here: room, campfire, three empty spatialised layers |
| `W3B_Solution` | Finished recorded and procedural fires to compare |
| `W3B_Student_Example` | What a finished student fire sounds like (build with W3B > 8) |

## Scripts (`Assets/W3B_Fire/Scripts`)

- **Student/**: `NoiseLayer`, `EventLayer` (sculpt by ear), `MyRandomPlayer` and `MyDynamicLink` (one line of C# each).
- **Layers/**: finished layers, `FireRecipe` (Farnell's hissing, crackling and lapping), `ExampleDynamicLink`.
- **Demos/**: the demo steps, including `PdRain` (a C# port of a Pure Data rain patch).
- **Core/**: shared building blocks (`ProceduralSound`, `PdDsp` Pd-style filters, `IntensityController`).
- **Editor/**: the **W3B** menu that builds every scene and records walkthroughs (needs Unity Recorder).
