# W3B FireLab: Sound Design and Dynamic Audio

COMSM0127 Immersive Interaction and Audio Design, Week 3 Lab B (University of Bristol).
Unity **6000.3.19f1**, spatialised with **Steam Audio 4.7.0**.

## Open the project

1. On this page: green **Code** button > **Download ZIP** (or clone it). Unzip to a short path, e.g. `C:\Unity\W3B_FireLab`.
2. Unity Hub: **Add > Add project from disk**, choose the folder, open in **6000.3.19f1**. The first open takes a few minutes.
3. Open `Assets/W3B_Fire/Scenes/W3B_Student_Shell` and press Play.

Steam Audio 4.7.0 is already included (Apache 2.0, (c) Valve Corporation). If sound is not spatialised, run **W3B > 1. Use Steam Audio**.

## Audio (`Assets/W3B_Fire/Audio`)

Real recordings by Rich Hemming: `Fire_Recorded` (hiss and lapping loops, crackles, a full fire loop), `Rain_Recorded`, `Mystery`, `Demo`.

## Scenes (`Assets/W3B_Fire/Scenes`)

| Scene | Purpose |
|---|---|
| `W3B_Demo` | The lecturer's demo: 11 steps, arrow keys, on-screen sliders and oscilloscope |
| `W3B_Student_Shell` | Students start here: room, campfire, three empty spatialised layers |
| `W3B_Solution` | Finished recorded and procedural fires to compare |
| `W3B_Student_Example` | What a finished student fire sounds like (build with W3B > 8) |
| `W3B_Finale` | Everyone back together: one fire, three soundtracks (recorded static, recorded dynamic, procedural). Burn down (B), add a log (L), change the wood and the size |

## Scripts (`Assets/W3B_Fire/Scripts`)

- **Student/**: `NoiseLayer`, `BurstLayer` (Part B: sculpt by ear), `MyRandomPlayer` (Part A) and `MyDynamicLink` (Part B): one line of C# each, every line commented, `AddLogOnKey` (the finale's interactive line).
- **Layers/**: finished layers, `FireRecipe` (Farnell's hissing, crackling and lapping), `ExampleDynamicLink`.
- **Demos/**: the demo steps, including `PdRain` (a C# port of a Pure Data rain patch).
- **Core/**: shared building blocks (`ProceduralSound`, `PdDsp` Pd-style filters, `IntensityController`).
- **Editor/**: the **W3B** menu that builds every scene and records walkthroughs (needs Unity Recorder).
