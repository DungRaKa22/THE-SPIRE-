# Forgotten Kingdom — primary platform atlas preview v1

Generated with the built-in image_gen tool from Docs/TheSpire-ArtPrompts.md, section 2.1 and the shared header.

Output: ForgottenKingdomPlatforms-v1.png

Reading order: stone battlement, broken wooden roof plank, stone pillar, wooden crate stack.

Validation: actual size is 1254 × 1254 (requested 1024 × 1024), Format32bppArgb, alpha range 0–255, 1,122,380 fully transparent pixels. Four isolated objects are arranged in a 2 × 2 sheet. This remains an art preview: normalize atlas dimensions and padding, and flatten the raised ends of the battlement standing surface before assigning a simple flat collider in Unity. The image has not been imported or assigned to the game.

## Exact generation prompt

Use case: stylized-concept.
Asset type: primary platform sprite atlas for Forgotten Kingdom, the medieval/gothic ruins sector in The Spire, a 2D side-view precision jumping game.
Create one production-ready pixel-art PNG sprite sheet, exactly 1024 x 1024 pixels. Exactly 2 columns and 2 rows of equal 512 x 512 cells, with NO drawn grid. Actual transparent alpha background.
Crisp detailed pixel art, orthographic FRONT/side view, NOT isometric, no perspective tilt. Consistent pixel scale across all cells. This is a game asset to slice into equal cells in Unity, not a presentation concept.
Era: medieval / gothic ruins.
Palette: cracked grey stone, dark timber, warm firelight gold, muted moss green.
Materials: rough-cut stone, weathered wood, rusty iron.
Exactly FOUR isolated objects in reading order:
1. TOP LEFT: wide stone battlement platform, bright flat continuous horizontal walkable top, chunky worn masonry below, subtle cracks and restrained moss. Keep the standing surface level and readable.
2. TOP RIGHT: broken wooden roof plank platform, weathered dark timber and rusty iron fasteners, clear flat horizontal top surface, worn splintered ends below the top. Side elevation, no sloped standing surface.
3. BOTTOM LEFT: stone column / broken pillar with a flat capped top. Worn grey masonry, simple medieval/gothic character, chipped shaft, restrained moss, bright readable horizontal cap that a character can stand on.
4. BOTTOM RIGHT: wooden crate stack, one combined isolated stack of weathered dark wooden crates with braces and rusty fasteners, clearly horizontal uppermost walkable edge.
Only one complete isolated object per cell, centered in its cell, with at least 8 percent transparent padding on ALL sides. No contact between cells, no clipping, no detached fragments outside an object's cell. Platform standing top edges should contrast clearly on both dark and light backgrounds through a subtle bright edge, not glowing neon.
No backdrop, no painted checkerboard, no ground plane, no ground or cast shadow outside the object, no environmental scene, no characters, no text, no labels, no watermark, no visible grid lines. Preserve genuine transparent alpha.

