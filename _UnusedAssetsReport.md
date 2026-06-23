# Unused Asset Report

Generated 2026-06-22. **Nothing has been moved or deleted yet** — this is for your review.
Machine-readable lists are saved in `~/.claude/plans/unused-scan/`.

## Method & caveats

- An asset is flagged **Unused** only if its GUID appears **zero times** in any non-`.meta`
  serialized file across the whole project (all scenes incl. non-build ones, all prefabs,
  materials, `.asset`s, controllers, packages) **and** in `ProjectSettings/`.
- **Scripts** get an extra check: if no scene/prefab references the GUID *and* no other `.cs`
  file mentions the type name, it's flagged. Scripts referenced only by C# code are kept.
- **Conservative by design.** Assets referenced by each other but not by any built scene
  (islands) are NOT flagged — they count as "referenced", so they stay. So do anything under a
  `Resources/` folder. Scope is **your own content only**; the 7 imported packages are treated
  as untouchable blocks (one whole-package check below).
- Known blind spot: material remaps stored inside `.fbx.meta` import settings aren't scanned, so
  a material used *only* via an FBX remap could be mis-flagged. None of your flagged `.mat`s
  below are in that situation, but worth knowing.

## Summary

| Category | Count | Size | Action |
|---|---|---|---|
| Unused `.wav` (Sounds) | 462 | ~33 MB | move |
| Unused other assets (non-script) | 56 | ~79 MB | move |
| Unused scripts (guid + code checked) | 5 | — | move (review first) |
| **Whole package `Blink/` (0 refs)** | 191 files | **~180 MB** | review for deletion |
| Scenes not in Build Settings | 8 | — | **decide — not auto-moving** |
| Scripts referenced by code only | 3 | — | keep (no action) |

Used / referenced in your content: **179** files. Sounds: **23 of 485** `.wav` are actually used.

---

## 1. Whole-package candidate: `Blink/` (180 MB)

`Blink/` (a weapons art pack) has **0** of its 191 GUIDs referenced anywhere in your content.
It looks entirely unremoved-but-unused. Recommend moving the whole folder to `_NotUsed/Blink/`.
The other 6 packages **are** referenced and stay put:

| Package | referenced by your content |
|---|---|
| External Assets | 395 — heavily used |
| KriptoFX (effects) | 32 — used |
| LeartesStudios (mansion) | 1 — used (entry environment) |
| ModularFirstPersonController | 2 — used |
| TextMesh Pro | 2 — used (essential) |
| Paladin Mats | 2 — used |
| **Blink** | **0 — unused** |

---

## 2. Unused scripts (5) — review before moving

These have no scene/prefab GUID reference and no C# type-name reference anywhere:

- `Assets/Scripts/Bootstrap/Bootstrapper.cs`  *(a pre-existing bootstrapper, not wired up)*
- `Assets/Scripts/MagicSpells/LevelOnePuzzlesManager.cs`
- `Assets/Scripts/MagicSpells/SpellBook.cs`
- `Assets/Scripts/MeshGenerator.cs`
- `Assets/Scripts/Puzzles/BrickWallPuzzle/BrickPuzzleInteractable.cs`

**Kept** (referenced by code, no inspector link): `EventManager.cs`, `Interactable.cs`,
`MagicSpells/Spell.cs`.

---

## 3. Unused non-script assets — 56 reviewable items (excludes the 462 `.wav`s)

Full `.wav` list is in `~/.claude/plans/unused-scan/unused_assets.txt`.

**Prefabs (6):**
- `Assets/Assets 1/FirstPersonController.prefab`
- `Assets/Assets 1/GrimoireManager.prefab`
- `Assets/Assets/Wand.prefab`
- `Assets/Prefabs/DialoguePrefabs/DialogueCanvas.prefab`
- `Assets/Prefabs/InteractableBook.prefab`
- `Assets/Prefabs/PlayerCamera.prefab`

**Materials (4):**
- `Assets/Assets/UI Assets/Materials/d10_outline.mat`
- `Assets/Materials/Player Materials/Beta_HighLimbsGeoSG3.mat`
- `Assets/Materials/Player Materials/Beta_Joints_MAT1.mat`
- `Assets/Scripts/New Material.mat`  *(stray .mat sitting in Scripts/)*

**Art / UI textures (24):**
- `Assets/Art/MainMenu/echoes-eternal-02-seance-clean.png`
- `Assets/Art/MainMenu/echoes-eternal-02-seance.png`
- `Assets/Art/MainMenu/settings-background-4k.png`
- `Assets/Art/MainMenu/settings-title-4k.png`
- `Assets/Art/MainMenu/settings-title-8k.png`
- `Assets/Art/MainMenu/settings/unity_pack/README.md`
- `Assets/Art/MainMenu/settings/unity_pack/Reference/frame_01_engraved.png`
- `Assets/Art/MainMenu/settings/unity_pack/Reference/frame_02_seance.png`
- `Assets/Art/MainMenu/settings/unity_pack/Reference/frame_03_reliquary.png`
- `Assets/Art/MainMenu/settings/unity_pack/Sprites/arrow_right.png`
- `Assets/Art/MainMenu/settings/unity_pack/Sprites/btn_primary_hover.png`
- `Assets/Art/MainMenu/settings/unity_pack/Sprites/btn_primary_normal.png`
- `Assets/Art/MainMenu/settings/unity_pack/Sprites/cycler_bg.png`
- `Assets/Art/MainMenu/settings/unity_pack/Sprites/panel_bg.png`
- `Assets/Art/MainMenu/settings/unity_pack/Sprites/panel_reliquary.png`
- `Assets/Art/MainMenu/settings/unity_pack/Sprites/rule_gold.png`
- `Assets/Art/MainMenu/settings/unity_pack/Sprites/slider_knob.png`
- `Assets/Art/MainMenu/settings/unity_pack/Sprites/title_settings.png`
- `Assets/Art/MainMenu/settings/unity_pack/Sprites/toggle_knob_off.png`
- `Assets/Art/MainMenu/settings/unity_pack/ui_slices.json`
- `Assets/Assets/UI Assets/arrow_diagonal_cross_divided.png`
- `Assets/Assets/UI Assets/d10_outline_number.png`
- `Assets/Assets/UI Assets/iconsDefault.png`
- `Assets/Shaders/Water Normals/Water 0341normal.jpg`
- `Assets/Shaders/Water Normals/shallow_normal.png`

**Shaders / misc (4):**
- `Assets/Shaders/Moon.shadersubgraph`
- `Assets/Settings/DefaultVolumeProfile.asset`
- `Assets/UI Toolkit/UnityThemes/UnityDefaultRuntimeTheme.tss`
- `Assets/Animations/Top_Lod.controller`

**Audio (1 mp3):** `Assets/Sounds/Jazz Me Blues - E's Jammy Jams.mp3`

**Stale bake outputs for scene `1920` (16, regenerable on next bake):**
- `Assets/Scenes/1920/Lightmap-0..5_comp_dir.png` / `_comp_light.exr` (12 files)
- `Assets/Scenes/1920/ReflectionProbe-0..3.exr` (4 files)
  *(Current GI uses Adaptive Probe Volumes `.bytes`, so these old lightmaps are leftovers.)*

---

## 4. Scenes NOT in Build Settings — your call (not auto-moving)

These aren't in the build, but may be dev/test scenes you still want. I will **not** move scenes
unless you tell me which. Build scenes: `Bootstrap`, `1920_MainMenu`, `1920`.

- `Level 1.unity`, `Level 2.unity`, `SampleScene.unity`, `SceneSimon.unity`
- `PuzzleTesting.unity`, `PuzzleTestingNEW.unity`, `Room2Tests.unity`, `Testing.unity`

---

## Proposed move (on your approval)

Create `Assets/_NotUsed/` mirroring original paths, and move:
1. `Blink/` → `_NotUsed/Blink/`
2. The 462 unused `.wav`s + 56 other assets → `_NotUsed/<original path>`
3. The 5 unused scripts → `_NotUsed/Scripts/...`

Each file moves **with its `.meta`** (GUIDs preserved, so if something was a false positive,
moving it back restores all references). Scenes-not-in-build handled separately per your choice.
Nothing deleted — `_NotUsed/` is a quarantine you can delete later in one action.
