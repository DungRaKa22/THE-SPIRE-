# The Climber — art preview v1

Generated with the built-in image_gen tool from Docs/TheSpire-ArtPrompts.md, section 1.1.

Output: ClimberAtlas-v1.png

Validation: actual size is 1254 × 1254 (requested 1024 × 1024); RGBA with true transparent pixels (alpha range 0–255). The sheet contains 16 poses in a 4 × 4 arrangement. This is a visual draft; frame alignment, foot baselines and run-cycle consistency need refinement before a Unity sprite import. It has not been assigned to the game.

## Exact generation prompt

Use case: stylized-concept.
Asset type: The Climber character sprite atlas for The Spire, a 2D side-view precision jumping game in Unity.
Create exactly one PNG sprite sheet, 1024 by 1024 pixels, with a REAL transparent alpha background, 4 columns by 4 rows of equal 256 by 256 cells. No visible grid.
Style: crisp detailed pixel art, orthographic side view, NOT isometric, no perspective tilt. Consistent pixel scale and character identity across all 16 cells. This is a game asset to slice into equal cells, not a presentation concept.
Subject: The Climber — a humble medieval-era scavenger in a worn patched cloak, small satchel at the hip, and a single improvised leg exo-brace (Neural Jump Drive), built from a salvaged relic: dark iron-and-copper mechanical shin with a faint cyan energy core at the knee. Hood down or loosely up, simple cloth boots, short dark hair, warm ochre/rust cloak. Readable silhouette, about 185 pixels tall in neutral standing poses, feet baseline at local y=230 inside EVERY cell, centered at local x=128. All 16 poses face RIGHT, consistent side view.
Row 1, left to right, subtle idle breathing:
f0 neutral idle; f1 inhale, chest up slightly; f2 neutral; f3 exhale, shoulders down slightly. Only subtle 1–2 pixel variation.
Row 2, left to right, run cycle, consistent scale and feet baseline:
f4 contact; f5 passing; f6 contact with opposite leg; f7 passing with opposite leg.
Row 3, left to right, action poses, all aligned to the same feet baseline, facing RIGHT:
f8 shallow charge crouch; f9 deep charge crouch, tension and arms back; f10 ascending jump, legs tucked and cloak trailing up; f11 descending fall, arms out and cloak flared.
Row 4, left to right:
f12 landing crouch impact; f13 upright recovery; f14 airborne wall-bounce pose, body turned toward the implied wall on the right, brace pushed out, but no wall drawn; f15 ready idle, weight on brace leg.
Palette: worn ochre cloak, dark brown cloth, iron grey brace, copper accents, a single cyan glow at the knee core. Keep same anatomy, head, satchel, brace leg, clothing and proportions in every frame. Every complete character including cloak stays strictly inside its own cell with transparent spacing between characters.
No backdrop, no checkerboard painted into image, no ground plane, no cast shadow outside character, no environmental objects, no weapons, no text, no labels, no watermark. Actual transparent alpha background.

