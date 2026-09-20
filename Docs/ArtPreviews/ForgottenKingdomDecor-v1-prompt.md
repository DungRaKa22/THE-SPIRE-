# Forgotten Kingdom — decorative atlas preview v1

Generated with the built-in image_gen tool from Docs/TheSpire-ArtPrompts.md, section 2.1 (secondary sheet) and the shared header.

Output: ForgottenKingdomDecor-v1.png

Reading order: church bell, hanging rope, iron wall sconce with flame, stone gargoyle statue.

Validation: actual size 1254 × 1254 (requested 1024 × 1024); Format32bppArgb; alpha range 0–255 with 1,182,858 fully transparent pixels. Four distinct objects appear in a 2 × 2 arrangement. This remains a visual draft: normalize size and cell padding before Unity import; the rope and flame approach their cell edges more closely than the requested 8% margin. Decorative rendering contrast should be tuned within the game scene. Not imported or assigned to the game.

## Exact generation prompt

Use case: stylized-concept.
Asset type: secondary decorative sprite atlas for Forgotten Kingdom, medieval / gothic ruins sector in The Spire, a 2D side-view precision jumping game.
Create ONE pixel-art PNG sprite sheet, exactly 1024 x 1024 pixels, exactly 2 columns and 2 rows of equal 512 x 512 cells. No visible grid. Genuine transparent alpha background.
Crisp detailed pixel art in orthographic FRONT / side elevation, NOT isometric, no perspective tilt. Consistent pixel scale and stylistic treatment across the four cells. This is a sprite atlas for slicing in Unity, not a presentation concept.
Palette: cracked grey stone, dark weathered timber, rusty iron, aged bronze, restrained warm firelight gold, muted moss green. Medieval / gothic ruins. Decorative assets should have restrained contrast, softer edge highlights than interactive platforms, and no bright horizontal walkable-edge markings.
Exactly FOUR complete isolated objects in reading order:
TOP LEFT: one old church bell, aged dark bronze, hanging from a small weathered timber support and rusty iron mounting, visible clapper. Bell and compact support are a single connected object. No church or scenery.
TOP RIGHT: one hanging rope, coarse weathered brown hemp, suspended vertically from a simple small rusty iron attachment, subtle natural curve and frayed lower tip. Long enough to read clearly as a hanging rope, fully inside its cell.
BOTTOM LEFT: one iron wall sconce with a small warm flame, simple medieval wrought iron bracket supporting a fire cup. Bracket, cup and flame form one connected isolated object. No wall behind it. Flame remains compact; no big bloom or opaque glow backdrop.
BOTTOM RIGHT: one weathered stone gargoyle statue, a compact crouching gothic creature with folded wings on a small integral stone base, low contrast grey stone with subtle cracks and very restrained moss. Clearly a decorative statue, not an enemy or playable character, no glowing eyes or flat luminous landing edge.
Each object centered within its own cell, fully contained with at least 8 percent transparent padding on all four sides (at least 41 pixels for 512-pixel cells). No touching or crossing between cells, no clipping.
No scenery, no backdrop, no painted checkerboard, no ground plane, no detached objects, no cast shadows outside objects, no characters, no text, no labels, no watermark, no visible grid. Actual transparent alpha, including around the fire.

