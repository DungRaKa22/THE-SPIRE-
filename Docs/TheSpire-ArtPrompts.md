# THE SPIRE — Bộ prompt sinh ảnh & quy cách import

> Bạn chạy các prompt dưới đây bằng công cụ image-gen. Sau đó tôi cắt sprite, gán pivot và dựng vào
> Unity bằng editor script (đúng quy trình dự án đang dùng cho Cyberpunk).
>
> ⚠️ Giữ **alpha trong suốt thật**, **Point filter**, **không nén**, **không mipmap**.
> Ảnh trong `levelImages/` của Jump King chỉ dùng làm tham chiếu bố cục — **không** đưa vào prompt hay build.

---

## 0. Quy ước chung

### Kích thước & lưới

| Asset | Kích thước | Lưới | Ghi chú |
|---|---:|---|---|
| Atlas nhân vật | 1024×1024 | 4×4 (256×256/ô) | 16 frame, chuẩn pivot chân |
| Atlas prop | 1024×1024 hoặc lẻ | 2×2 hoặc 1 ô | Mỗi vật 1 ô, có padding |
| Nền parallax | 1200×900 × số lớp | — | Ghép ngang/tiles, xa→gần |
| Biểu tượng chung | 512×512 | 4×4 | Mặt trời tiến hóa, lõi cấu trúc |
| HUD | 512×256 | — | Khung, thanh lực, ký hiệu nguy hiểm |

### Khối "header" dùng chung cho MỌI prompt

```
Production-ready pixel-art sprite sheet for a 2D side-view precision jumping game.
Crisp detailed pixel art, orthographic FRONT/side view, NOT isometric, no perspective tilt.
Consistent pixel scale across all cells. Flat clearly horizontal walkable top surfaces
where noted. Actual transparent alpha background, no backdrop, no ground plane, no drop
shadow outside the object, no grid lines, no text unless explicitly requested.
Only one isolated object per cell, contained with padding, centered.
This is a game asset to slice into equal cells in Unity, not a presentation concept.
```

### Nguyên tắc đọc được (bắt buộc cho mọi asset)

- Mép trên bề mặt đứng được phải **sáng và rõ** trên cả nền tối lẫn sáng.
- Prop trang trí **giảm tương phản**, không dùng cùng ký hiệu hình học với bệ tương tác.
- Bệ có chu kỳ báo trạng thái bằng **hình dạng/chuyển động + màu** (không chỉ màu).

---

## 1. Nhân vật — THE CLIMBER

### 1.1 Atlas chính — `ClimberAtlas.png` (1024×1024, 4×4)

```
<HEADER>
Use case: stylized-concept, character sprite sheet.

Subject: "The Climber" — a humble medieval-era scavenger in a worn patched cloak,
small satchel at the hip, and a single improvised leg exo-brace (the "Neural Jump Drive"),
built from a salvaged relic: dark iron-and-copper mechanical shin with a faint cyan energy
core at the knee. Hood down or loosely up, simple cloth boots, short dark hair, warm-toned
cloak (ochre/rust) contrasting a cold grey-blue world. Readable silhouette, ~185 px tall,
feet baseline at y=230 inside each cell, centered at x=128. Facing RIGHT in all cells.

Exactly 4 columns × 4 rows of equal 256×256 cells, no grid lines.
Row 1 (idle breathing, 4 frames, subtle 1-2px bob):
  f0 neutral idle, f1 inhale (chest up), f2 neutral, f3 exhale (shoulders down)
Row 2 (run cycle, 4 frames, identical scale & baseline):
  f4 contact, f5 passing, f6 contact (opposite), f7 passing
Row 3 (action poses, all feet baseline, facing RIGHT):
  f8 charge crouch shallow, f9 charge crouch deep (tension, arms back),
  f10 ascending jump (legs tucked, cloak trailing up),
  f11 descending fall (arms out, cloak flared)
Row 4:
  f12 landing crouch (impact), f13 upright recovery,
  f14 airborne wall-bounce pose (body turned toward wall, brace pushed out),
  f15 idle ready (weight on brace leg), spare consistent idle

Every full character including cloak contained strictly in its own cell.
Consistent identity, body proportions and palette across all 16 frames.
Palette: worn ochre cloak, dark brown cloth, iron grey brace, copper accent,
single cyan glow on the knee core. No environmental objects, no weapons.
```

### 1.2 Overlay tiến hóa theo thời kỳ (tùy chọn) — 7 lớp nhỏ

Vẽ **cùng dáng**, chỉ thêm dấu vết lên atlas gốc bằng lớp phủ riêng, gắn theo khu:

```
<HEADER>
Use case: stylized-concept, character overlay layer.
Same "Climber" silhouette, facing RIGHT, feet baseline y=230, centered x=128,
transparent everywhere except the added wear layer. Keep the base palette visible.

Layer topic: <điền theo bảng dưới>.
```

| Khu | Lớp phủ |
|---|---|
| Forgotten Kingdom | (không có — atlas gốc) |
| Age of Steam | vết bụi than đen mờ trên vai/áo choàng |
| Electric Age | vài điểm phản chiếu ánh đèn ấm trên mép áo |
| Machine Age | dầu mỡ loang, một miếng băng vải trên tay |
| Digital Revolution | đường mạch mảnh phát sáng nhạt dưới lớp vải |
| Neon Megacity | phản chiếu cyan/magenta nhấp nháy trên mép hood |
| The Singularity | ánh chrome lạnh ở khớp, lớp vải bạc màu |

---

## 2. Prop theo khu (mỗi khu 6 object)

Dùng khuôn này, mỗi ô là một phần tử:

```
<HEADER>
Square canvas divided into exactly 2 columns and 2 rows equal cells (or a single cell).
Each object fits entirely within its own cell with at least 8% transparent padding,
no contact between cells. Bright readable walkable top edge on platform pieces.

Era: <tên thời kỳ>. Style palette: <bảng màu>. Material: <vật liệu>.
Cells, in reading order:
  1: <tên> — <mô tả>
  2: <tên> — <mô tả>
  3: <tên> — <mô tả>
  4: <tên> — <mô tả>
```

### 2.1 Forgotten Kingdom (đá, gỗ, lửa)

```
Era: medieval / gothic ruins. Palette: cracked grey stone, dark timber, warm firelight
gold, muted moss green. Materials: rough cut stone, weathered wood, rusty iron.
Cells: 1) wide stone battlement platform (bright flat top), 2) broken wooden roof plank,
3) stone column / broken pillar with flat capped top, 4) wooden crate stack; plus a
secondary sheet: 1) church bell, 2) hanging rope, 3) iron wall sconce with small flame,
4) stone gargoyle / knight statue (decorative, low contrast).
```

### 2.2 Age of Steam (đồng, than, cam lò)

```
Era: industrial revolution / steampunk. Palette: copper, brass, soot black, forge orange.
Materials: riveted copper pipe, cast iron, glowing furnace grate.
Cells: 1) wide piston platform with a visible flat copper top cap and a vertical shaft
below, 2) gear-wheel platform (flat top), 3) maintenance catwalk segment,
4) steam vent pipe with a round pressure-gauge face; secondary sheet: 1) large flywheel
(decorative), 2) pressure gauge cluster, 3) coal hopper, 4) hanging chains & hook.
```

### 2.3 Electric Age (đèn vàng, cyan nhạt, thép tối)

```
Era: early 20th century electrification / dieselpunk. Palette: warm bulb yellow,
pale cyan, dark steel, amber glass. Materials: porcelain insulators, steel lattice,
glass bulbs, copper wire.
Cells: 1) electric platform with THREE clear states drawn as separate small variants
(striped safe top / amber warning chevrons / cyan sparks on contact edge),
2) steel lattice tower segment with flat top, 3) transformer box, 4) street-lamp
platform with flat top; secondary sheet: 1) porcelain insulator stack, 2) coiled cable,
3) warning sign plate (blank), 4) hanging industrial bulb string.
```

### 2.4 Machine Age (bê tông, CRT, đỏ)

```
Era: modern industry / retro-futurism / automation. Palette: concrete grey, CRT green,
warning red, oily black. Materials: poured concrete, rubber conveyor, painted steel.
Cells: 1) conveyor belt platform with directional chevrons and visible roller ends,
2) concrete slab platform with bright flat top, 3) industrial robot arm (flat top
mounting plate), 4) control console with CRT screen; secondary sheet: 1) magnetic
platform with two visible pole coils and a flat top, 2) empty locker bank,
3) taped cardboard box, 4) ceiling-mounted warning beacon.
```

### 2.5 Digital Revolution (xanh lam, trắng, tím)

```
Era: near-future digital. Palette: electric blue, bright white, violet, cool grey.
Materials: server rack metal, glass panel, fibre-optic cable, LED strip.
Cells: 1) server-rack platform with flat top and blinking LED strip,
2) glitch platform drawn in TWO variants: solid (continuous bright rim + contact shadow)
   and glitching (broken/dashed rim, faint scanline), 3) fibre-optic conduit segment,
4) holographic emitter node; secondary sheet: 1) floating data screen (no readable text),
2) camera drone, 3) cable bundle, 4) antenna array.
```

### 2.6 Neon Megacity (cyan, magenta, tím, đen)

```
Era: cyberpunk megacity. Palette: cyan, magenta, deep violet, near-black, wet reflections.
Materials: wet metal grating, neon tube, holographic billboard, composite drone hull.
Cells: 1) rooftop exhaust-vent platform with flat cyan-lit top and rain streaks,
2) jump-booster pad with a clear magenta UP arrow marking on a flat top,
3) neon billboard platform (walkable top, abstract glowing glyphs, no readable words),
4) maintenance bridge segment; secondary sheet: 1) hover drone platform with a flat
landing pad and a small countdown indicator (blank digits), 2) neon sign frame with
abstract glyphs, 3) rain pipe, 4) antenna mast with aviation light.
```

### 2.7 The Singularity (trắng, đen, chrome)

```
Era: post-cyberpunk / post-human. Palette: white, black, chrome, pale cyan energy.
Materials: seamless ceramic, polished chrome, floating light frames, no visible seams.
Cells: 1) white floating platform with a thin bright continuous rim and flat top,
2) gravity-frame platform with clear directional arrows pointing UP and a visible
   defined boundary edge, 3) chrome service arm (android maintenance), 4) light gate
   frame segment; secondary sheet: 1) floating data monolith, 2) maintenance android
   partial silhouette (decorative, low contrast), 3) resonance crystal, 4) thin
   light pillar.
```

### 2.8 The Sky (bầu trời thật)

```
Era: the real sky above the tower. Palette: natural daylight blue, pale gold, soft white,
high-altitude haze. Materials: bare structural girders, no city textures.
Cells: 1) spire summit platform segment, 2) orbital tether anchor with flat top,
3) simple metal railing, 4) observatory deck plate; secondary sheet: 1) small antenna,
2) The First Light interface pedestal (clean white with a single warm glow),
3) weather vane, 4) distant structure silhouette (decorative).
```

---

## 3. Nền parallax theo khu (3–4 lớp mỗi khu)

Vẽ **từng lớp riêng**, 1200×900 mỗi lớp, ghép ngang. Xa = ít chi tiết, tối; gần = rõ hơn.
**Không** đặt chi tiết tương phản cao ở giữa màn (tránh tranh chấp với điểm đáp).

```
<HEADER>
Wide parallax background layer, 1200×900, tiles horizontally. No walkable surfaces,
no props, no characters, no text. Muted, low-contrast, deep depth.
Era: <tên>. Layer <n>/<tổng>: <mô tả>.
```

| Khu | Lớp 1 (xa) | Lớp 2 | Lớp 3 | Lớp 4 (gần) |
|---|---|---|---|---|
| Forgotten Kingdom | sương núi xám | tháp chuông & mái thành | giàn giáo, cối xay | tường đá tối, dây thừng |
| Age of Steam | khói bụi cam | ống khói khổng lồ | bánh răng & bồn hơi | dầm sắt tối, đường ống |
| Electric Age | trời chiều vàng | tháp ăng-ten | trạm biến áp | khung thép, bóng đèn |
| Machine Age | khói xám phẳng | nhà xưởng bê tông | robot nền lặp nhịp | tường bê tông, cáp |
| Digital Revolution | gradient tím-lam | tòa server | màn LED mờ | tường dữ liệu, sợi quang |
| Neon Megacity | mưa & mây tím | skyline cao ốc neon | biển quảng cáo mờ | face tiền tối, ống thông gió |
| The Singularity | khoảng không trắng | khối lơ lửng mờ | khung sáng mảnh | mặt phẳng chrome |
| The Sky | mây cao & vành quỹ đạo | đường chân trời xa | — | — |

---

## 4. Bộ biểu tượng chung + HUD

### 4.1 `SpireIcons.png` (512×512, 4×4) — biểu tượng nối thế giới

```
<HEADER>
4×4 cells. Theme: the recurring symbols that tie all eras together.
Cells: 1) sun relief carving (medieval stone), 2) sun industrial logo (stamped metal),
3) sun electric sign (bulb outline), 4) sun CRT vector emblem, 5) sun digital glyph,
6) sun neon hologram, 7) sun chrome First Light interface symbol, 8) real natural sun.
9) structural core / conduit cross-section (stone), 10) same (copper pipe),
11) same (electrical conduit), 12) same (data cable), 13) same (neon tube),
14) same (chrome light channel), 15) climber-mark scratch on stone, 16) same mark
preserved in digital data. Monochrome-friendly, readable at small size.
```

### 4.2 `SpireHud.png` (512×256)

```
<HEADER>
UI elements on transparent alpha: a slim horizontal charge meter frame, a charge fill
bar (solid, with a distinct "max charge" cap end), a pause icon, a restart icon,
a sector-progress vertical rail with 8 tick marks, a small danger triangle icon,
a coin icon (spinning gold coin), and a lore fragment icon (torn page / data shard).
Bright rim highlights so the meter reads on both dark and bright backgrounds.
No text.
```

---

## 5. Quy cách import (khớp code hiện có)

| Thiết lập | Giá trị |
|---|---|
| Texture Type | Sprite (2D and UI) |
| Sprite Mode | Multiple |
| Filter Mode | Point (no filter) |
| Compression | None |
| Mipmaps | Off |
| Alpha Is Transparency | On |
| NPOT Scale | None |
| Pixels Per Unit | **100** cho art The Spire (nhân vật có thể để riêng) |
| Pivot nhân vật | chân, căn theo pixel có alpha trong mỗi ô |
| Pivot platform | mép trên giữa (0.5, 1) |
| Pivot prop trang trí | tùy vật |
| Sorting Layer | Background < Decor < Platform < Player < FX < HUD |

Đặt file vào `Assets/Art/TheSpire/<Khu>/`, tên file **đúng** như tài liệu này để script cắt khớp.

---

## 6. Checklist giao asset

- [ ] `ClimberAtlas.png` — 16 frame, 4×4, alpha thật
- [ ] 7 lớp phủ tiến hóa (tùy chọn)
- [ ] 8 khu × 6 prop tương tác
- [ ] 8 khu × 4 prop trang trí
- [ ] 8 khu × 3–4 lớp nền parallax
- [ ] `SpireIcons.png` + `SpireHud.png`
- [ ] Tất cả có alpha thật, không viền nền, không chữ
