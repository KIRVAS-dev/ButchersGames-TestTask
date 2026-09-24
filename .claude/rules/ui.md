---
paths:
  - "ButchersGames/Assets/_Project/Scripts/UI/**"
  - "ButchersGames/Assets/_Project/Prefabs/UI/**"
  - "ButchersGames/Assets/_Project/Graphics/UI/**"
  - "ButchersGames/Assets/_Project/Graphics/Fonts/**"
---

# UI (uGUI, TMP)

Screen code (View / Presenter / `Api/`) — [feature-anatomy.md](../reference/feature-anatomy.md), "UI screens". Scene / prefab edits go through Coplay / Pipeline — [unity-mcp.md](unity-mcp.md). This file covers layout, assets and import settings.

## Screens

Current scheme (may change with the UI architecture):

- A screen is a prefab in `Prefabs/UI/`; its instance lives under `Canvas/SafeZone`
- The screen root GameObject stays **active**; the View shows / hides its own `Content` (`_root`). A disabled root means the screen never appears

## Graphic

Every `Image`, `RawImage`, `TextMeshProUGUI` and other `MaskableGraphic`, when created or edited:

- **Raycast Target** — off, except a Button's target graphic or a deliberate click blocker. Gameplay input comes from the Input System (`DragInput`), not from UI raycasts
- **Maskable** — off, unless the element sits under a `Mask` / `RectMask2D` that must clip it

In the report, name the elements that keep either flag on and why.

## Animation

UI motion — **DOTween**, not Animator.

## Fonts

- Only **static** TMP font assets from `Graphics/Fonts/` (bake Latin + Cyrillic + digits + punctuation; clear `m_SourceFontFile`), so the TTF does not get into the build (`NotoSansCJK-Bold.ttf` is 19 MB)
- Material presets live next to their font asset: `{Font} - {Purpose}.mat`; a preset must use its own font's atlas (`_MainTex`)
- `TextMeshPro/Mobile/Distance Field`: outline works only with the `OUTLINE_ON` keyword; an underlay hidden under the outline needs `_UnderlayDilate` > 0

## Sprites

- Everything UI uses lives in `Graphics/UI/{Screen}/`; shared sprites — `Graphics/Common/Sprites/`
- No `Visual/Sprite/*.asset` — they point to missing textures; use PNG / JPG textures imported as sprites
- Import: Texture Type **Sprite**, Mesh Type **Full Rect**, Generate Physics Shape **off**, mipmaps **off**, Read/Write **off**, Max Size — by the on-screen size
- Every UI sprite is packed into `Graphics/UI/UI.spriteatlasv2` (Sprite Atlas V2 — Enabled): padding 4, no rotation / tight packing, no mipmaps; Android ASTC 6×6, WebGL compressed + Crunch. Sprites outside an atlas with sides not divisible by 4 stay uncompressed RGBA32
- Exception: 1×1 stretched sprites (`1PixelSprite`) stay outside the atlas — bilinear sampling would bleed neighbouring atlas pixels into them
