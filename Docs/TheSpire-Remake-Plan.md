# THE SPIRE — Kế hoạch remake toàn dự án

> Từ prototype **JumpDummy** (scene rời rạc, chỉ có Neon Ascent) → **The Spire**: một game leo tháp 8 khu,
> **chỉ 1 scene duy nhất**, cơ chế nhảy tích lực kiểu Jump King, art pixel nhiều lớp.

Tài liệu này là bản kế hoạch triển khai. Số liệu cân bằng là **đề xuất khởi điểm**, cần khóa lại sau playtest.
Nguồn tham chiếu:
- Ý tưởng game: `C:\Documents\game 3d\The-Spire-Y-Tuong-Game.md`
- Cấu trúc tầng Jump King: `Jump-King-main/LevelSetupFunction.js` (43 màn, mỗi màn lưới **1200×900 px**)
- Bản chơi hiện tại: `Assets/`, `Docs/`

## Quyết định đã chốt (phiên này)

| Vấn đề | Quyết định |
|---|---|
| Giai đoạn hiện tại | **Chỉ tài liệu/kế hoạch** — chưa viết code |
| Nhân vật | **The Climber mới** (áo choàng + chân trợ lực Neural Jump Drive) |
| Art | Tôi **viết prompt**; bạn sinh ảnh; sau đó tôi cắt/gán vào Unity |
| Màn cũ (`JumpLab`, `CyberpunkRooftops`, `NeonAscent`) | **Giữ làm phòng thử**, không nằm trong build The Spire |

Prompt sinh ảnh đầy đủ: xem **`Docs/TheSpire-ArtPrompts.md`**.

---

## 0. Đánh giá hiện trạng

| Hạng mục | Hiện tại | Vấn đề khi remake |
|---|---|---|
| Scene | 4 scene rời (`NeonAscent`, `CyberpunkRooftops`, `JumpLab`, `SampleScene`) | Phải gộp về **1 scene** |
| Level | Sinh bằng C# cứng trong `NeonAscentBuilder` | Cần format dữ liệu để dựng 8 khu, dễ chỉnh |
| Cân bằng | `DummyController` có 7 tham số, chưa có tài liệu chuẩn | Cần **khóa thông số + ngân sách nhảy** |
| Mechanic | Chỉ có bệ tĩnh + coin | Thiếu piston, băng chuyền, bệ điện, booster, trọng lực |
| Art | 16 sprite runner + 4 prop cyberpunk | Cần art cho 7 thời kỳ + The Sky |
| Lưu | `PlayerPrefs` theo `saveSlot`/slot | Giữ, thêm slot cho The Spire |
| Kiểm tra | Predictor vật lý cho 35 bệ | Mở rộng cho mọi khu |

Điểm tốt giữ lại: `DummyController` (cảm giác nhảy), `GameSession` (lưu/tiếp tục/kỷ lục),
`CyberFeedback` (âm + hạt), `CyberRunnerVisual` (state machine animator), và **validator vật lý**
trong `NeonAscentChecks` — nó chính là công cụ để bảo đảm mọi cú nhảy đều tới được.

---

## 1. Thông số cân bằng thống nhất

### 1.1 Nguyên tắc

1. **Vật lý nhảy khóa cứng ở mọi khu.** Người chơi học 1 đường cong nhảy duy nhất từ đầu tới cuối.
   Độ khó đến từ **bố cục**, không từ việc đổi cảm giác nhảy.
2. **Mọi thứ đo bằng "ngân sách nhảy":** H = chiều cao tối đa, D = tầm xa tối đa.
   Khoảng cách bệ luôn biểu diễn theo %H và %D.
3. **Không bao giờ bắt đạt cực đại cả 2 trục cùng lúc.** Một cú nhảy hợp lệ phải tồn tại ở mức tích lực < 1.
4. **Độ khó tăng bằng 4 núm xoay:** bề rộng bệ, khoảng cách, mật độ bệ nghỉ, độ dài cú rơi.

### 1.2 Hằng số vật lý (khóa — mọi khu dùng chung)

| Tham số | Giá trị | Ghi chú |
|---|---:|---|
| `gravityScale` | **3.0** | g_hiệu dụng ≈ 29.43 m/s² |
| `chargeDuration` | **0.85 s** | thời gian tích lực tối đa |
| `minimumJumpSpeed` | **4.0 m/s** | nhấn nhanh nhất |
| `maximumJumpSpeed` | **13.0 m/s** | giữ đủ 0.85 s |
| `horizontalJumpSpeed` | **6.0 m/s** | × `Lerp(0.4, 1, strength)` |
| `wallBounceRetention` | **0.8** | giữ năng lượng khi bật tường |
| Điều khiển trên không | **Không** | khóa hướng tại lúc thả nút |
| Sát thương do rơi | **Không** | rơi = mất tiến trình, không mất "máu" |

**Suy ra ngân sách nhảy (đây là số để thiết kế màn):**

| Đại lượng | Công thức | Giá trị |
|---|---|---:|
| Chiều cao đỉnh **H** | `maxJump² / (2·g_eff)` | **2.87 m** |
| Thời gian lên đỉnh | `maxJump / g_eff` | 0.44 s |
| Tầm xa ngang **D** (về cùng độ cao) | `horiz · 2·t_apex` | **5.30 m** |
| Chiều cao ở tích lực p | `H · p²` (bậc 2, không tuyến tính) | p=0.5 → 0.72 m |

> **Hệ quả thiết kế quan trọng:** lực nhảy là **bậc hai** theo thời gian giữ.
> Nửa thời gian tích lực chỉ được **1/4 chiều cao**. Mọi bảng khoảng cách dưới đây tính theo điều này.

### 1.3 Quy tắc khả đạt (validator sẽ ép)

Một liên kết bệ A→B hợp lệ khi tồn tại mức tích lực `p ∈ [0,1]` sao cho:
- `Δy ≤ 0.92 · H · p²` (lên tới được đỉnh bệ B)
- `|Δx| ≤ 0.85 · D · p` (tới ngang được trong lúc bay)
- Không chạm cạnh/trần của khối nào khác trên đường đi (hoặc chấp nhận bật tường có chủ đích)
- Điểm đáp nằm trong `[min.x + 0.3, max.x − 0.3]` của bệ B

Ngưỡng an toàn 0.92 / 0.85 để chừa biên cho người chơi thật (khác bot mô phỏng).

### 1.4 Núm xoay độ khó theo khu

| Núm | Ký hiệu | Dễ | Trung bình | Khó |
|---|---|---|---|---|
| Khoảng cách dọc | `Δy/H` | 0.55–0.65 | 0.65–0.80 | 0.80–0.90 |
| Khoảng cách ngang | `Δx/D` | 0.30–0.45 | 0.45–0.60 | 0.60–0.75 |
| Bề rộng bệ | W | 3.6 m (mái nghỉ) | 2.5–2.8 m | 1.7–2.1 m |
| Số mái nghỉ/khu | — | xem cột "Mái nghỉ" ở §1.5 | | |
| Cú rơi | — | ngắn (1 khu) | trung bình (2 khu) | dài (3+ khu) |

> **Sửa sau khi thiết kế khu 1–2:** cột "mật độ bệ nghỉ" dạng tỷ lệ (1/5, 1/6…) **không dùng được**.
> Khu 1 thiết kế ra **2 mái nghỉ trên 40 bệ** (≈ 1/20). Tỷ lệ 1/5 sẽ cho 8 mái nghỉ trong khu 1 —
> quá nhiều, làm mất sức nặng của khu. Dùng **số tuyệt đối** ở §1.5.

### 1.5 Phân bổ độ khó theo chiều cao

Tổng tháp **525 m**, chia theo đúng tỷ lệ trong tài liệu ý tưởng (12 / 13 / 12 / 12 / 13 / 20 / 15 / 3 %).

| # | Khu | Độ cao (m) | % cao | Δy/H | Bề rộng | Mái nghỉ | Cú rơi tối đa | Ghi chú |
|---|---|---:|---:|---|---|---:|---|---|
| 1 | Forgotten Kingdom | 0 – 63 | 12.0 | 0.55–0.65 | 2.5–3.6 | 2 | 1 khu (cuối) | Dạy nhảy |
| 2 | Age of Steam | 63 – 131 | 13.0 | 0.4–1.9¹ | 1.9–3.6 | 2 | 2 khu | Học nhịp piston |
| 3 | Electric Age | 131 – 194 | 12.0 | 0.45–0.58 ³ | 2.2–3.6 | 3 | 2 khu | Đọc trạng thái |
| 4 | Machine Age | 194 – 257 | 12.0 | 0.70–0.85 | 2.2–2.8 | 3 | 2 khu | Kiểm soát vị trí |
| 5 | Digital Revolution | 257 – 326 | 13.1 | 0.70–0.85 | 2.0–2.6 | 3 | 2 khu | Chuỗi bật/tắt |
| 6 | Neon Megacity | 326 – 431 | 20.0 | 0.75–0.90 | 1.8–2.6 | 3 | 3 khu | Cao trào, booster |
| 7 | The Singularity | 431 – 509 | 14.9 | 0.60–0.80² | 2.5–3.6 | 2 | 2 khu | ² trọng lực thấp |
| 8 | The Sky | 509 – 525 | 3.0 | 0.50–0.60 | 3.0–3.6 | dày | 0 | Kết đoạn + lựa chọn |

¹ Khu 2 rộng hơn vì piston **cho không chiều cao** (biên độ 1.8–2.2 m), nên Δy giữa hai bệ
thấp hơn nhiều so với các khu tĩnh.

² Khu 7 giảm Δy nhưng bù bằng vùng trọng lực — độ khó không giảm, chỉ đổi trục.

³ Khu 3 cố ý giữ `Δy` **thấp hơn** dải 0.65–0.80. Khi bệ có trạng thái theo chu kỳ, thời gian
bay `t₂` cộng thẳng vào thời gian tích lực để ra hạn chót rời bệ — nên `Δy` lớn làm cửa sổ
thời gian sụp rất nhanh. Độ khó của khu 3 nằm ở **cửa sổ trạng thái**, không ở khoảng cách.
Chi tiết và công thức: `Docs/TheSpire-Level-Sector3.md` §3–§4.

Khu 1–3 đã có thiết kế chi tiết từng bệ; các khu 4–8 chưa. Cột "Mái nghỉ" của khu 4–8 là
**ước lượng**, sẽ chốt lại khi thiết kế từng khu.

### 1.5b Cửa sổ thời gian — thước đo chung của khu 2 và khu 3

Hai khu dùng hai mechanic khác nhau nhưng cùng một đại lượng quyết định độ khó:
**khoảng thời gian người chơi còn được phép hành động, tính từ lúc tiếp đất.**

| Khu | Mechanic | Cửa sổ hẹp nhất | Cửa sổ rộng nhất |
|---|---|---:|---:|
| 2 Age of Steam | piston | 0.31 s (P-C) | 0.66 s (P-A) |
| 3 Electric Age | bệ điện theo chu kỳ | 0.47 s (E-C) | 2.75 s (E-A) |

Khu 3 có **trần cao hơn hẳn** (2.75 s) nhưng **sàn cũng thấp** (0.47 s). Nghĩa là khu 3
không khó đều — nó dốc: ba tầng đầu gần như không thể trượt, hai tầng cuối siết ngang khu 2.
Đây là chủ ý: người chơi phải học đọc trạng thái ở nơi an toàn trước khi gặp nó ở nơi nguy hiểm.

> Từ khu 4 trở đi, mọi mechanic mới **phải** được đối chiếu qua bảng này. Nếu một mechanic
> không diễn đạt được bằng "cửa sổ hành động", nó chưa đủ rõ để đưa vào thiết kế.

### 1.6 Quy tắc cú rơi

- **Rơi ngắn:** mất vài cú nhảy — dùng khi giới thiệu mechanic.
- **Rơi trung bình:** quay lại đoạn đã thành thạo — tạo sức nặng cho cao trào.
- **Rơi dài:** chỉ ở đoạn đã cung cấp đủ thông tin để cân nhắc rủi ro.
- Đặt **bệ bắt rơi** ngẫu nhiên: không phải sai sót nào cũng đưa về đáy.
- Checkpoint chỉ để **thoát/tải lại**, **không** phục hồi sau cú rơi (đúng chất Jump King).

### 1.7 Cảm giác & khả năng đọc

- Camera không bao giờ dịch chuyển/che điểm đáp khiến phải nhảy mù.
- Bệ tương tác phải khác biệt rõ với trang trí (viền sáng, chuyển động, ký hiệu).
- Mọi trạng thái nguy hiểm có báo trước bằng **hình ảnh**, âm thanh chỉ hỗ trợ.

---

## 2. Kiến trúc 1 scene duy nhất

### 2.1 Sơ đồ scene `Assets/Scenes/TheSpire.unity`

```
The Spire (scene)
├── Systems
│   ├── GameSession          (player, saveSlot = "Spire.v1", summitHeight, 8 sector)
│   ├── CyberFeedback        (SFX + hạt, đổi âm theo khu)
│   ├── SpireDirector        (MỚI: quản lý khu đang hoạt động, đổi nhạc/ánh sáng/nền)
│   └── SectorStreamer       (MỚI: bật/tắt khu theo camera để giữ hiệu năng)
├── Player
│   ├── DummyController      (vật lý, giữ nguyên)
│   └── Climber Visual       (Animator + CyberRunnerVisual)
├── Camera and HUD           (CyberpunkPresentation → đổi tên SpirePresentation)
├── Sectors                  (1 root chứa 8 khu, xếp chồng theo trục Y)
│   ├── 01 ForgottenKingdom  (0 – 63 m)
│   ├── 02 AgeOfSteam        (63 – 131 m)
│   ├── 03 ElectricAge       (131 – 194 m)
│   ├── 04 MachineAge        (194 – 257 m)
│   ├── 05 DigitalRevolution (257 – 326 m)
│   ├── 06 NeonMegacity      (326 – 431 m)
│   ├── 07 Singularity       (431 – 509 m)
│   └── 08 TheSky            (509 – 525 m)
└── Backgrounds (parallax theo khu, tắt/bật cùng SectorStreamer)
```

### 2.2 Vì sao 1 scene vẫn nhẹ

- Mỗi khu là con của 1 root; `SectorStreamer` bật khu khi nhân vật tới gần (buffer ±1 khu) và tắt phần còn lại.
- Nền parallax dùng chung sprite atlas theo thời kỳ, chỉ hoạt động khu đang xem.
- Tổng đường leo **525 m**; không gian toạ độ rời nên không tốn vẽ.

### 2.3 Format dữ liệu level (để dựng & chỉnh dễ)

Thay vì C# cứng như hiện tại, mỗi khu mô tả bằng **dữ liệu kê khai** (ScriptableObject hoặc JSON trong `Assets/LevelData/`):

```
SectorBlueprint {
  id, tên khu, chiềuCaoBắtĐầu, chiềuCaoKếtThúc
  floors: [ { y, bệ: [ { x, loại, rộng, cao, mechanic } ], coinGroup } ]
  transition: khu kế tiếp
  palette, nhạc, ambience
}
```

Editor script `TheSpireBuilder` đọc blueprint → **dựng 1 lần** vào scene.
Sau đó chỉnh trực tiếp trong scene (như hiện tại), builder không ghi đè.

### 2.4 Toạ độ & tỷ lệ (khớp Jump King)

| Khái niệm | Jump King | The Spire (đề xuất) |
|---|---|---|
| Lưới 1 tầng | 1200×900 px | **12 × 9 m** |
| Số tầng | 43 màn | **43 tầng**, chia 8 khu |
| Cao 1 tầng | 900 px | 9 m |
| Tổng cao | 38,700 px | **525 m** (≈ 58 tầng) |
| Tỷ lệ pixel | pixel art | **PPU 100** (art & bg), màn hình ~1 tầng rưỡi |
| Số bệ / khu | (43 màn) | **40–45 bệ / khu**, tổng ~330 bệ |
| Sàn đấu | 1200 px | **12 m** (x ∈ [−6, +6]) |

> 43 màn Jump King được **phân bổ lại** thành 8 khu theo tỷ lệ chiều cao ở mục 1.5, giữ cấu trúc
> "nhiều tầng nhỏ xếp cao" — không copy hình ảnh hay hình học 1:1 (xem ghi chú bản quyền ở mục 4.1).

### 2.5 Tài liệu thiết kế từng khu

| Khu | Tài liệu | Trạng thái |
|---|---|---|
| 1 Forgotten Kingdom | `Docs/TheSpire-Level-Sector1.md` | ✅ 40 bệ (01–40), mọi liên kết đã kiểm tra |
| 2 Age of Steam | `Docs/TheSpire-Level-Sector2.md` | ✅ 43 bệ (41–83), có toán thời gian piston |
| 3 Electric Age | `Docs/TheSpire-Level-Sector3.md` | ✅ 42 bệ (84–125), có toán cửa sổ trạng thái |
| 4–8 | — | ⏳ chưa thiết kế |

### 2.6 Quy ước đánh số bệ (bắt buộc)

`PlatformCoins.Begin` sắp xếp **toàn bộ** collider trong scene theo `int.Parse(name.Split(' ')[0])`
rồi chia nhóm 5. Vì vậy:

1. Số **duy nhất và tăng dần theo độ cao trên toàn tháp** (khu 1 = 01–40, khu 2 = 41–83, khu 3 từ 84).
2. **Không** đánh số tường, sàn, trần, trang trí, vùng mechanic — nếu đánh số, chúng bị tính là bệ và coin rơi sai nhóm.
3. Khu dài 43 bệ → `43 / 5 = 8` nhóm coin; 3 bệ cuối chưa đủ nhóm nên **không sinh coin**.

> **Cần quyết định:** ~330 bệ → ~66 coin (660 điểm). Đề xuất giữ luật và coi coin là điểm tùy chọn,
> hoặc đổi thành 1 coin / 8 bệ từ khu 4 trở lên. Chưa chốt.

---

## 3. Cấu trúc 8 khu

Mỗi khu theo nhịp: **Giới thiệu → Luyện → Kết hợp → Cao trào → Điểm nghỉ**.

| Khu | Mechanic chính | Component cần thêm | Vật liệu nền |
|---|---|---|---|
| 1 Forgotten Kingdom | Bệ tĩnh | — (đã có) | Đá nứt, mái gỗ, tháp chuông |
| 2 Age of Steam | Piston di chuyển | `MovingPiston` | Lò hơi, bánh răng, ống đồng |
| 3 Electric Age | Bệ nhiễm điện theo chu kỳ | `CycleHazardPlatform` | Nhà máy điện, cột dây |
| 4 Machine Age | Băng chuyền | `ConveyorBelt` | Bê tông, robot, CRT |
| 5 Digital Revolution | Bệ bật/tắt theo mẫu | `TogglePlatform` | Server, LED, sợi quang |
| 6 Neon Megacity | Bệ tăng lực | `JumpBooster`, `DronePlatform` | Mái cao ốc, neon, mưa |
| 7 The Singularity | Vùng trọng lực cục bộ | `GravityZone` | Trắng–đen–chrome |
| 8 The Sky | Đoạn leo ngắn + lựa chọn | `EndingChoice` | Bầu trời thật, vành quỹ đạo |

**Quy tắc mechanic (để không phá cảm giác nhảy):**
- Piston: chu kỳ **cố định**, có âm/đèn báo vị trí; lần đầu bệ rộng.
- Bệ điện: 3 pha an toàn → cảnh báo → nhiễm điện; điện **bật** nhân vật, không trừ máu.
- Băng chuyền: đổi **điểm xuất phát tích lực**, không đổi đường cong nhảy.
- Bệ bật/tắt: trạng thái sắp mất có viền đứt nét; luôn có chỗ đứng an toàn để quan sát trọn chu kỳ.
- Booster: vùng kích hoạt có mũi tên hướng; cú đầu đích rộng, nhìn thấy trước.
- Trọng lực: chỉ **giảm** trong vùng có biên nhìn thấy; ra khỏi biên trở về bình thường.

---

## 4. Phương án art

### 4.1 Trả lời thẳng câu hỏi "bạn có thiết kế ảnh không?"

**Trong phiên này tôi KHÔNG có công cụ sinh ảnh.** Tool list của tôi chỉ gồm đọc/sửa file, chạy lệnh,
tìm code. Các `RunnerAtlas.png` / `EnvironmentAtlas.png` trong dự án là do một công cụ image-gen
bên ngoài tạo ở phiên trước, không phải tôi vẽ trực tiếp.

Tôi có **4 cách thay thế thực dụng** (chọn ở cuối tài liệu):

1. **Viết prompt sinh ảnh chi tiết** cho từng asset (nhân vật, prop mỗi thời kỳ, nền mỗi khu) để bạn
   chạy bằng công cụ image-gen của bạn; tôi lo phần **cắt sprite, gán pivot, dựng Animator, gắn vào scene**
   bằng editor script (đúng như dự án đang làm).
2. **Sinh pixel art bằng code C#** ngay trong Unity (như `PlatformCoins` tự vẽ sprite coin) — phù hợp
   cho prop hình học, ký hiệu, hạt, HUD, nền gradient nhiều lớp.
3. **Blockout hình khối màu** trước (đúng Bước 3 trong tài liệu ý tưởng): dựng toàn tuyến bằng hình
   chữ nhật màu, khóa gameplay trước, thay art sau.
4. **Hướng dẫn pipeline cho bạn tự vẽ/import** (Aseprite, `com.unity.2d.aseprite` đã có trong dự án).

Tôi đề xuất **kết hợp 1 + 3**: blockout để khóa cân bằng ngay, prompt để bạn sinh art song song.

> ⚠️ **Bản quyền:** ảnh trong `levelImages/` là art có bản quyền của Jump King. Chỉ được dùng làm
> **tham chiếu bố cục/độ khó** khi thiết kế, **không** được ship hay sao chép vào build.

### 4.2 Danh mục asset cần có

| Nhóm | Số lượng | Định dạng | Ghi chú |
|---|---:|---|---|
| Nhân vật (Climber) | 16 frame / 4×4 atlas | PNG trong suốt 1024² | Idle, Run, Charge, Deep, Jump, Fall, Land, Bounce |
| Trang phục theo thời kỳ | 7 biến thể | overlay hoặc recolor | Bụi than → phản chiếu điện → neon → chrome |
| Prop tương tác | 4–6 / khu | atlas 2×2 hoặc lẻ | Khớp vùng va chạm rõ |
| Prop trang trí | 6–10 / khu | atlas | Giảm tương phản |
| Nền parallax | 3–4 lớp / khu | PNG ngang 1200×(900·n) | Xa → gần, ít chi tiết cạnh tranh điểm đáp |
| Biểu tượng chung | 1 bộ | atlas | Mặt trời tiến hóa qua 7 thời kỳ, lõi cấu trúc xuyên tầng |
| HUD + ký hiệu nguy hiểm | 1 bộ | atlas | Đọc được trên nền tối/sáng |

**Tổng:** ~8 khu × (6 prop + 4 lớp nền) ≈ **48 nền + 48 prop + 1 atlas nhân vật + 7 overlay**.

### 4.3 Prompt mẫu (khuôn chung — điền theo khu)

```
Create one production-ready transparent PNG sprite atlas for a 2D pixel-art jumping game
(with the exact dimensions and grid below). Crisp pixel art, consistent scale, actual
alpha background, no backdrop, no ground shadow, no text unless requested.
[KHU] = <tên thời kỳ>; palette = <bảng màu>; material = <vật liệu>;
walkable top edge must read clearly against the background.
Output: <kích thước>, <số cột>×<số hàng> equal cells, no grid lines.
Cells: <liệt kê từng ô: tên + mô tả ngắn>.
```

**Bảng màu theo thời kỳ:**

| Khu | Ánh sáng | Bảng màu |
|---|---|---|
| Forgotten Kingdom | lửa ấm | xám đá, nâu gỗ, vàng lửa, xanh rêu |
| Age of Steam | cam lò | đồng, đen than, cam |
| Electric Age | đèn vàng | vàng điện, cyan nhạt, thép tối |
| Machine Age | CRT xanh | xám, xanh lá CRT, đỏ cảnh báo |
| Digital Revolution | trắng–tím | xanh lam, trắng, tím |
| Neon Megacity | neon | cyan, magenta, tím, đen |
| The Singularity | lạnh | trắng, đen, chrome |
| The Sky | tự nhiên | xanh trời thật, vàng nhạt |

---

## 5. Gợi ý nâng cấp (ngoài yêu cầu gốc)

**Cảm giác & khả năng tiếp cận**
- `SpirePresentation`: thanh lực tùy chọn, giảm rung/chớp, chỉnh âm lượng từng nhóm, phụ đề lời thoại.
- Đường "vệt rơi" mờ cho thấy quỹ đạo vừa rồi — giúp học mà không phá độ khó.

**Nội dung & chiều sâu**
- NPC tại điểm nghỉ + câu thoại đổi theo số lần rơi (theo tài liệu ý tưởng).
- Mảnh lore nhặt được (7 mảnh) mở khóa đoạn AI giải thích ở The Sky.
- Kết ASCEND / RETURN + cho tải lại trạng thái trước quyết định.
- Chế độ luyện tập tách biệt thành tích (điểm trở lại riêng).

**Hệ thống & công cụ**
- `SectorBlueprint` + editor preview dựng từng khu nhanh, không cần Play.
- Mở rộng validator vật lý `NeonAscentChecks` thành `SpireChecks` chạy **mọi liên kết** mọi khu và
  **xuất ảnh cao trào** mỗi khu (như script hiện tại đang làm cho Neon Ascent).
- Lưu ảnh chụp từng khu thành contact-sheet để review nghệ thuật.
- Bảng kỷ lục theo khu (thời gian / số lần nhảy / cú rơi).

**Kỹ thuật**
- Pooling cho coin và hạt để 1 scene ~260 bệ vẫn mượt.
- `SectorStreamer` + LOD collider: khu không hoạt động tắt collider để giảm physics.
- Deterministic RNG theo seed cho coin/trang trí (giữ như hiện tại).

---

## 6. Lộ trình triển khai

| GĐ | Việc | Kết quả |
|---|---|---|
| **1. Nền tảng** | Khóa `GameBalance`, viết `SpirePresentation`, `SectorStreamer`; tạo scene `TheSpire.unity` rỗng + Systems | 1 scene chạy được, chưa có màn |
| **2. Blockout** | `TheSpireBuilder` dựng 8 khu bằng khối màu theo blueprint | Toàn tuyến leo/rơi kiểm tra được |
| **3. Cân bằng** | `SpireChecks` xác nhận 100% liên kết tới được; chỉnh trong ngưỡng 1.3 | Độ khó khóa bằng số liệu |
| **4. Mechanic** | Thêm piston, băng chuyền, bệ điện, bật/tắt, booster, trọng lực | 7 khu khác biệt về luật |
| **5. Art** | Nhận/cắt sprite, gán vào prefab, dựng Animator, thêm nền parallax | Thay blockout bằng art |
| **6. Âm & lore** | Ambience + nhạc theo khu, NPC, mảnh lore, hai ending | Trải nghiệm hoàn chỉnh |
| **7. Đóng gói** | Build Windows, playtest đầu-cuối, cân lại độ dài cú rơi | Bản chơi hoàn chỉnh |

**Việc dọn dẹp:** xóa/gộp `JumpLab`, `CyberpunkRooftops`, `SampleScene` sau khi The Spire chạy;
hoặc giữ làm phòng thử mechanic (không nằm trong build).

---

## 7. Tiêu chí nghiệm thu

- [ ] Chỉ còn **1 scene** trong Build Settings.
- [ ] Mọi liên kết bệ trong 8 khu **tới được** bằng mô phỏng vật lý thật.
- [ ] Nhảy giống hệt nhau ở mọi khu (không đổi tham số theo thời gian chơi).
- [ ] Mỗi mechanic có báo trước bằng hình ảnh và có chỗ quan sát an toàn.
- [ ] Mọi cú rơi có thể hiểu được nguyên nhân; có bệ bắt rơi hợp lý.
- [ ] Lưu/tiếp tục, hai ending, và ảnh chụp từng khu đều hoạt động.
- [ ] 60 FPS ở 1 scene đầy đủ.
