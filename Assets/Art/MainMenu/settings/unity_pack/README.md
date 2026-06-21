# Echoes of the Eternal — UI Atom Pack (Unity)

Drop-in, **2× resolution**, true-alpha UI sprites for the Settings screen, built as
**reusable atoms** (not baked screenshots) so you assemble real, functional, resizable
UI in Unity from them.

```
unity_pack/
├─ Sprites/            18 transparent PNGs — the atoms
├─ Editor/             EchoesUISpriteImporter.cs (auto-sets Sprite type + 9-slice borders)
├─ Atlas/              ui_atlas.png + ui_atlas.json (TexturePacker-style frames + borders)
├─ ui_slices.json      machine-readable 9-slice borders + pivots (engine-agnostic)
├─ Reference/          the 3 full Settings frames + composite rows (design reference only)
└─ README.md
```

## 1. Install
Copy the whole `unity_pack` folder into your project's `Assets/`. The editor script
(`Editor/EchoesUISpriteImporter.cs`) runs automatically on import and configures every
sprite in `Sprites/`:
- Texture Type → **Sprite (2D and UI)**
- **Alpha Is Transparency** on (no dark edge fringe)
- **9-slice border** preset per sprite (see table)
- Bilinear filter, no mipmaps, clamp wrap, 100 px/unit

If a sprite imports without borders, right-click it → **Reimport**.

## 2. Atoms & how to use them

| Sprite | Use | Image Type |
|---|---|---|
| `panel_bg`, `panel_reliquary` | section/menu backgrounds | **Sliced** |
| `btn_primary_normal` / `_hover` | Apply/confirm button bg | **Sliced** |
| `btn_ghost_normal` / `_hover` | Back/Reset button bg | **Sliced** |
| `slider_track` | slider groove (Background) | **Sliced** |
| `slider_fill` | slider fill (Fill Area → Fill) | **Sliced** |
| `slider_knob` | slider handle | Simple |
| `toggle_track_on` / `_off` | toggle background | **Sliced** |
| `toggle_knob_on` / `_off` | toggle handle | Simple |
| `cycler_bg` | option-cycler frame | **Sliced** |
| `arrow_left`, `arrow_right` | cycler ‹ › buttons | Simple |
| `diamond_bullet` | section-header bullet | Simple |
| `rule_gold` | header underline / divider | **Sliced** (horizontal) |
| `title_settings` | baked “SETTINGS” title + flourish | Simple |
| `title_rule` | flanking flourish line for an editable title | **Sliced** (horizontal) |

> **Important:** for any *Sliced* sprite, set the **Image → Image Type = Sliced** in the
> Inspector so the 9-slice borders are honored and it scales to any size without
> distortion. Text/labels are **not** baked in — use TextMeshPro on top
> (font: Cinzel for labels, Cormorant Garamond italic for values).

### Build a Settings row (example: a slider row)
1. Empty `RowMaster` (Horizontal Layout Group).
2. **Label** — TMP text "Master Volume" (Cinzel).
3. **Slider** (UI → Slider):
   - Background image → `slider_track` (Sliced)
   - Fill → `slider_fill` (Sliced)
   - Handle → `slider_knob`
4. **Value** — TMP text "80%" (Cormorant Garamond italic), driven by the slider.

### Title (two ways)
- **Baked / drop-in:** use `title_settings` as a plain Image — done, no font setup.
- **Editable:** put a TMP “SETTINGS” (Cinzel Decorative) between two `title_rule` images
  (Sliced) so you can re-letter or localize it. `title_settings` is no-border; `title_rule` 9-slices horizontally.

### Build a button
`UI → Button`, set its Image to `btn_primary_normal` (Sliced), and in the Button
**Transition = Sprite Swap**, set Highlighted Sprite → `btn_primary_hover`. Child TMP
for the label. Scales to any width thanks to 9-slice.

### Build a toggle
`UI → Toggle`: Background → `toggle_track_off`, Checkmark → `toggle_knob_on`; swap
`_on`/`_off` track in your handler, or use Sprite Swap transitions.

## 3. 9-slice border reference (pixels, at 2×)
border = inset of each edge. Unity `spriteBorder` = (x: left, y: bottom, z: right, w: top).

| Sprite | L | T | R | B |
|---|---|---|---|---|
| panel_bg | 34 | 34 | 34 | 34 |
| panel_reliquary | 38 | 38 | 38 | 38 |
| btn_primary_normal / hover | 30 | 30 | 30 | 30 |
| btn_ghost_normal / hover | 28 | 28 | 28 | 28 |
| slider_track | 26 | 8 | 26 | 8 |
| slider_fill | 22 | 8 | 22 | 8 |
| toggle_track_on / off | 24 | 24 | 24 | 24 |
| cycler_bg | 28 | 28 | 28 | 28 |
| rule_gold | 40 | 1 | 40 | 1 |
| title_rule | 40 | 1 | 40 | 1 |

(`slider_knob`, `toggle_knob_*`, `arrow_*`, `diamond_bullet` are plain sprites — no border.)

## 4. Scale / DPI
Sprites are authored at **2×**. With `pixelsPerUnit = 100` they look crisp on 4K.
For a 1× project, set the project's reference resolution or scale the Canvas Scaler;
the 9-slice borders keep corners sharp at any size.

## 5. Atlas (optional)
`Atlas/ui_atlas.png` + `ui_atlas.json` pack all atoms with frame rects **and** the
9-slice border per frame, for engines/tools that consume a TexturePacker-style sheet.
Unity users can ignore this and use the individual sprites, or build a Unity
`SpriteAtlas` from the `Sprites/` folder.

## 6. Editable source
The look comes from `Asset Factory.dc.html` (atoms) and `Settings Menu.dc.html`
(full screens). Edit colors/metrics there and re-export to regenerate the sprites.

---
Palette: gold `#e8cf97` / `#a07d3e`, bright `#f4e1ad`, border `rgba(202,167,101,*)`,
dark glass `rgba(20,14,8,0.8)`, groove `rgba(8,5,2,0.78)`.
