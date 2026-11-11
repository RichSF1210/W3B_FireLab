# W3B FireLab: Sound Design and Dynamic Audio

COMSM0127 Immersive Interaction and Audio Design, Week 3 Lab B (University of Bristol).
Unity **6000.3.19f1**, spatialised with **Steam Audio 4.7.0**.

## Worksheet

- [Lab worksheet (PDF)](Docs/W3-LabB_Worksheet.pdf)
- [One-page A4 sheet (PDF)](Docs/W3-LabB_Student_Sheet_A4.pdf)

## Open the project

1. On this page: green **Code** button > **Download ZIP** (or clone it).
2. **Windows: unblock the zip before extracting.** Right-click the zip > **Properties** > tick **Unblock** > **OK**. Without this, university machines block the extract ("Windows found that this file is potentially harmful") and leave an empty folder.
3. Right-click the zip > **Extract all**. The Desktop is fine.
4. Open `Assets/W3B_Fire/Scenes` in File Explorer and double-click **W3B_Student_Shell**. Unity opens the project with that scene (first open takes a few minutes). Unity Hub (**Add > Add project from disk**, **6000.3.19f1**) also works, but on university machines its file browser may not find your Desktop.
5. Press Play. Walk with **W A S D**; hold the right mouse button to look.

By the end of the lab you have a finished Campfire A and Campfire B in this one project.

Steam Audio 4.7.0 is already included (Apache 2.0, (c) Valve Corporation). If sound is not spatialised, run **W3B > 1. Use Steam Audio**.

## Audio (`Assets/W3B_Fire/Audio`)

Real recordings by Rich Hemming: `Fire_Recorded` (hiss and lapping loops, crackles, a full fire loop), `Rain_Recorded`, `Mystery`, `Demo`.

## Scenes (`Assets/W3B_Fire/Scenes`)

| Scene | Purpose |
|---|---|
| `W3B_Demo` | The lecturer's demo: 11 steps, arrow keys, on-screen sliders and oscilloscope |
| `W3B_Student_Shell` | **Students start here.** A room and two silent campfires: **Campfire A (Recorded)** for Part A (amber sign) and **Campfire B (Procedural)** for Part B (blue sign). Three empty spatialised layers each: one part of the sound per layer |
| `W3B_Complete_Reference` | Both campfires finished, to compare with yours afterwards. A: hiss loop, random crackles, lapping loop. B: hiss and roar NoiseLayers, a crackle BurstLayer, linked to Intensity |
| `W3B_Finale` | Everyone back together: one fire, three soundtracks (recorded static, recorded dynamic, procedural). Burn down (B), add a log (L), change the wood and the size |

## Scripts (`Assets/W3B_Fire/Scripts`)

- **Student/**: `NoiseLayer`, `BurstLayer` (Part B: sculpt by ear), `MyRandomPlayer` (Part A) and `MyDynamicLink` (Part B): one line of C# each, every line commented, `AddLogOnKey` (the finale's interactive line).
- **Layers/**: finished layers, `FireRecipe` (Farnell's hissing, crackling and lapping), `ExampleDynamicLink`.
- **Demos/**: the demo steps, including `PdRain` (a C# port of a Pure Data rain patch).
- **Core/**: shared building blocks (`ProceduralSound`, `PdDsp` Pd-style filters, `IntensityController`).
- **Editor/**: the **W3B** menu that builds every scene and records walkthroughs (needs Unity Recorder). **W3B > Walkthrough** does each live-build step for you if you get lost.

## Where a procedural sound gets its shape

Source (noise) > filter (what it seems made of) > envelope (how it moves in time) > randomness (it never repeats) > parameters (what the game can change). Each Part B script explains these steps line by line.
