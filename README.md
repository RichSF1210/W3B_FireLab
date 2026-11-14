# W3B FireLab: Sound Design and Dynamic Audio

COMSM0127 Immersive Interaction and Audio Design, Week 3 Lab B (University of Bristol).
Unity **6000.3.19f1**, spatialised with **Steam Audio 4.7.0**.

## Worksheet

- [Worksheet (PDF)](Docs/W3B_Worksheet.pdf): every step, with pictures. Start here.
- [Quick Card (PDF)](Docs/W3B_Quick_Card.pdf): a two-page summary for your desk.

## Get the project (use GitHub Desktop)

Use **GitHub Desktop** (free: [desktop.github.com](https://desktop.github.com)). Downloading the ZIP causes problems: Windows blocks the extract, and macOS blocks the Steam Audio plugin ("damaged and can't be opened"). GitHub Desktop avoids both.

1. On this page: green **Code** button > **Open with GitHub Desktop**. (Or in GitHub Desktop: **File > Clone Repository > URL**, paste `RichSF1210/W3B_FireLab`.)
2. **Local Path:** choose a folder on this computer that does **not** sync to OneDrive or iCloud, e.g. `C:\Unity\` on Windows, or a folder in your home folder on a Mac. Click **Clone**. You do not need to sign in to GitHub.
3. **Your own computer?** Open **Unity Hub** once and sign in. If it asks for a licence, add the free **Personal** licence (Hub > Settings > Licenses > Add).
4. **Open it in Unity:**
   - **Windows:** in File Explorer, open `Assets/W3B_Fire/Scenes` and double-click **W3B_Student_Shell**.
   - **Mac:** in Unity Hub: **Add > Add project from disk**, choose the `W3B_FireLab` folder, open it with **6000.3.19f1**.
   - The first open takes a few minutes.
5. In Unity's Project window: `Assets/W3B_Fire/Scenes`, double-click **W3B_Student_Shell** if it is not already open. Press Play. Walk with **W A S D**; hold the right mouse button to look.

**Never press Commit or Push in GitHub Desktop.** Your copy is yours to change; you cannot change this page.

**If GitHub Desktop is not available:** **Code > Download ZIP**. Windows: right-click the zip > **Properties** > tick **Unblock** > **OK**, then **Extract all**. Mac: if you see "audioplugin_phonon.bundle is damaged", press **Cancel**, never **Move to Bin**, and ask for help.

By the end of the lab you have a finished Campfire A and Campfire B in this one project.

Steam Audio 4.7.0 is already included (Apache 2.0, (c) Valve Corporation). If sound is not spatialised, run **W3B > 1. Use Steam Audio**.

## Audio (`Assets/W3B_Fire/Audio`)

Real recordings by Rich Hemming: `Fire_Recorded` (example recordings to use if yours do not work), `Rain_Recorded`, `Mystery` (the mystery sound from the start of the lab), `Demo`. Put your own recordings in `My_Recordings`.

## Scenes (`Assets/W3B_Fire/Scenes`)

| Scene | Purpose |
|---|---|
| `W3B_Demo` | The lecturer's demo: 11 steps, arrow keys, on-screen sliders and oscilloscope |
| `W3B_Student_Shell` | **Students start here.** A room and two silent campfires: **Campfire A (Recorded)** for Part A (amber sign) and **Campfire B (Procedural)** for Part B (blue sign). Three empty spatialised layers each: one part of the sound per layer |
| `W3B_Complete_Reference` | Both campfires finished, to compare with yours afterwards. A: hiss loop, random crackles, lapping loop. B: hiss and roar NoiseLayers, a crackle BurstLayer, linked to Intensity |
| `W3B_Finale` | Everyone back together: one fire, three soundtracks (recorded static, recorded dynamic, procedural). Burn down (B), add a log (L), change the wood and the size |

## Scripts (`Assets/W3B_Fire/Scripts`)

- **Student/**: `NoiseLayer`, `BurstLayer` (Part B: sculpt by ear), `MyRandomPlayer` (Part A) and `MyDynamicLink` (Part B): one line of C# each, every line commented, `AddLogOnKey` (the finale's interactive line).
- **Layers/**: finished layers, `FireRecipe` (Farnell's hissing, crackling and lapping; also how the finale's Wood and Size work), `ExampleDynamicLink`. **Layers/OldExercises/**: the expert stretch: write your own building block (see its README).
- **Demos/**: the demo steps, including `PdRain` (a C# port of a Pure Data rain patch).
- **Core/**: shared building blocks (`ProceduralSound`, `PdDsp` Pd-style filters, `IntensityController`).
- **Editor/**: the **W3B** menu that builds every scene and records walkthroughs (needs Unity Recorder). **W3B > Walkthrough** does each live-build step for you if you get lost.

## Where a procedural sound gets its shape

Source (noise) > filter (what it seems made of) > envelope (how it moves in time) > randomness (it never repeats) > parameters (what the game can change). Each Part B script explains these steps line by line.
