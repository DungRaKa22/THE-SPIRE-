# 01 — Kiến trúc & thông số khóa

## Ngân sách nhảy (khóa cứng, dùng cho mọi thiết kế)

Từ `DummyController`: `gravityScale 3` → gₑff ≈ 29.43 m/s², `chargeDuration 0.85 s`,
jump 4→13 m/s, ngang `6 × Lerp(0.4, 1, strength)`.

- **H** = chiều cao tối đa ≈ **2.87 m**, **D** = tầm xa tối đa ≈ **5.30 m**
- Lực nhảy **bậc hai** theo thời gian giữ: nửa lực → 1/4 chiều cao. Khoảng cách bệ
  luôn diễn đạt theo %H / %D, không theo trực giác tuyến tính.
- Ngưỡng khả đạt: `Δy ≤ 0.92·H(p)` **và** `Δx ≤ 0.85·D(Δy,p)`.
  Trần thiết kế: **Δy ≤ 2.64 m**. `t₂` tính nhánh rơi xuống (đúng kiểu Jump King).
- Khu 1 chỉ dùng J1–J3 (Δy ≤ 1.9). J4 (Δy ≥ 2.3) dành từ khu 4+.

## Quy cách bảng bệ v2 (chốt 20/09/2026 — mọi khu dùng chung)

Bảng bệ trong `Docs/TheSpire-Level-Sector*.md` là **hợp đồng dữ liệu** giữa tài liệu, builder
(`TheSpireBuilder`) và validator (`SpireChecks`). Sai tên cột hoặc sai nghĩa cột ⇒
`Tools/jumpcheck.py` và validator hiểu sai. Cột bắt buộc:

| Cột | Nghĩa | Từ khu |
|---|---|---|
| `#` | số bệ, duy nhất, **tăng dần toàn tháp**. Tường/sàn/trần/trang trí **không đánh số** | 1 |
| `Tên` | tên bệ; bệ nghỉ ghi `Rest Roof — <tên>` | 1 |
| `Loại` | `tĩnh` · `P-A/P-B/P-C` (piston) · `băng chuyền` · `bật-tắt` · `booster` · `drone` · `vùng trọng lực` | 2 |
| `x` | hoành độ **tâm** bệ (m) | 1 |
| `y đỉnh` | cao độ **mặt đứng được** (m). Bệ di chuyển ghi thêm `y_lo`/`y_hi` | 1 |
| `rộng` | bề rộng bệ (m) | 1 |
| `Δy` | `y đỉnh` đích − `y đỉnh` nguồn. Piston: nguồn → **đáy** piston (cú mount) hoặc đỉnh → đỉnh (cú cross) | 1 |
| `Δx` | **mép gần bệ nguồn → tâm bệ đích**: `Δx = max(0, |x_đích − x_nguồn| − rộng_nguồn/2)` | 1 |
| `p_min` | mức tích lực **nhỏ nhất** thoả **cả hai** điều kiện (độ cao **và** tầm xa) | 1 |
| `p_max` | mức **lớn nhất** còn hợp lệ (trần/che khuất/chướng ngại). Không giới hạn ⇒ `1.00` | 4 |
| `W` | cửa sổ hành động (giây) theo §1.5b — chỉ với bệ có trạng thái/chu kỳ | 2 |

### Luật

1. **Cột `p` cũ bị bỏ, thay bằng `p_min`.** Khu 1–3 đang ghi `p` theo mức tối thiểu **chỉ độ cao**
   (19 dòng lệch, xem `Docs/TheSpireJumpCheck.txt` mục CONV/WEAK). Đây là lỗi quy ước, không phải
   lỗi thiết kế: mọi liên kết vẫn tới được, chỉ là con số ghi trong bảng nhỏ hơn thực tế.
2. **`p_min` lấy từ công cụ, không tính tay:** `python3 Tools/jumpcheck.py`. Công cụ dùng đúng
   hằng số khoá ở §Ngân sách nhảy phía trên.
3. **Liên kết căng** khi `p_max − p_min < 0.10` ⇒ phải ghi chú lý do ngay dưới bảng, và phải có
   bệ bắt rơi hoặc mái nghỉ trước đó. Cửa sổ bằng 0 (`p_max < p_min`) là **lỗi cứng**.
4. `Δy ≤ 2.64 m` (0.92·H) cho mọi liên kết — công cụ chặn.
5. Khu 1–3 khi chuyển sang v2 giữ nguyên số `#`, chỉ đổi tên/nội dung cột và sửa 4 lỗi hình học.

### Ví dụ tính tay (để đối chiếu với công cụ)

**Ca 1 — bảng khu 2 dòng `046 Piston A2`, `Δy = 1.20 m`, `Δx = 2.30 m`:**

- Chỉ theo độ cao: `v ≥ √(1.20·2·29.43 / 0.92) = 8.76` ⇒ `p ≥ 0.529` → đây là con số `0.53` đang ghi
  trong bảng (mức **chỉ độ cao**, sai nghĩa cột).
- Thêm tầm xa ở `p = 0.70`: `v = 10.3`, `horiz = 4.92`, `t₂ = 0.552` ⇒ `0.85·D = 2.31 ≥ 2.30` ✓
- Ở `p = 0.69`: `0.85·D = 2.26 < 2.30` ✗ ⇒ **`p_min = 0.70`** (công cụ cho đúng 0.70).

**Ca 2 — bài học trần thấp khu 1 (`010 → 011`, trần `Machicolation` ở `y = 15.10`):**

- `p_min = 0.66` (với `Δx` thật 1.80 m, không phải 1.50 m đang ghi trong bảng).
- Trần cho `h(p) ≤ 2.30 m` ⇒ `v ≤ 11.64` ⇒ **`p_max = 0.85`**.
- Cửa sổ `p ∈ [0.66, 0.85]` — rộng 0.19, đủ dùng. Đây là ví dụ mẫu của cột `p_max`.

## Tháp & scene

- Tổng cao **525 m**, 8 khu đúng tỷ lệ ý tưởng: FK 12% / Steam 13% / Electric 12% /
  Machine 12% / Digital 13% / Neon 20% (105 m) / Singularity 15% / Sky 3%.
- Mục tiêu: **`TheSpire.unity` duy nhất**, `SectorBlueprint` (dữ liệu kê khai) +
  builder dựng một lần + `SectorStreamer` bật/tắt theo camera.
- Bệ đánh số liên tục toàn tháp (khu 3 kết thúc ở 125). Coin: nhóm 5 bệ, 1 coin/nhóm
  (theo `PlatformCoins` hiện có).
- Quy tắc khu (từ kế hoạch): p ≤ 0.80 phần thân, ≤ 0.90 điểm cao trào; bệ nghỉ
  **số tuyệt đối** 2/khu (không dùng tỷ lệ); rơi ngắn giữa khu, rơi dài chỉ khi
  đã có bệ bắt rơi; §1.5b — mọi mechanic mới phải diễn đạt được bằng "cửa sổ hành động".

## Code hiện có

| File | Vai trò |
|---|---|
| `DummyController.cs` | Nhảy tích lực, đất/đá/bật tường. Có `ApplyImpulse` (cửa vào duy nhất cho hất ngoài), `LastImpulseTime` (mốc xung chung), `CurrentSurface` + `CheckGrounded` (đất theo **vận tốc tương đối**) |
| `ElectricCircuit.cs` | Đồng hồ chung khu điện; preset E-A 6.0/4.0/0.8, E-B 4.6/2.8/0.6, E-C 3.4/1.9/0.5 (T/safe/warn); `OffsetForImpendingLive` để đặt pha deterministic |
| `ElectricPlatform.cs` | Không di chuyển, không tắt collider; LIVE → **poll `OverlapBox` mỗi FixedUpdate** (không dùng collision event — lỗi đã chặn) |
| `MovingSurface.cs` | Interface `IMovingSurface.SurfaceVelocity` cho piston/băng chuyền/drone |
| `GameSession.cs` | Trạng thái Title/Playing/Paused/Complete, lưu bằng PlayerPrefs |
| `PlatformCoins.cs` / `PlatformCoinPickup.cs` | Coin theo nhóm 5 bệ |
| `Cyberpunk/*` | HUD, camera, SFX, Animator state (đọc `LastImpulseTime` cho state Bounce) |
| `Editor/ElectricPlatformChecks.cs` | Menu check khu điện — **chưa ai chạy được Play Mode** |

## Quy tắc theo mechanic (từ tài liệu khu)

**Piston (khu 2):** biên dạng hình thang có làm mềm đầu; P-A 5.0 s, P-B 4.4 s, P-C 3.4 s.
Cửa sổ thả nút `bottom − 2·guard − Δt` (0.66 s → 0.31 s); quy tắc `top ≥ Δt_cross + 0.25 s`.
Tối đa 3 piston liên tiếp; đối pha chỉ khi có bệ tĩnh xen giữa. **Chỉ nhảy khỏi piston
đang dừng đỉnh** — không bao giờ đứng trên piston đang hạ. Root bắt buộc **Rigidbody2D
kinematic + `MovePosition`** (collider teleport bằng transform = cơ chế chết im lặng).
Khoảng trống ≥ 3.2 m phía trên đỉnh piston. Va trần KHÔNG xử lý piston (thiết kế bỏ).

**Bệ điện (khu 3):** SAFE→WARN(4 Hz nhấp nháy)→LIVE. Lệch pha `3T/4` là lệch pha
**dễ nhất** (dùng làm cửa nghỉ kỹ thuật); lệch pha `T/4` trên E-C không tồn tại
(`T/4 ≤ W(0)` là điều kiện tồn tại — validator phải chặn). Bệ nguồn tĩnh vẫn phải trừ
`t₂` khi tính cửa sổ. Sàn đấu 12 m (x ∈ [−6, +6]), lưới tầng 9 m.

## Quy cách import art

Point filter, no compression, no mipmap, **PPU 100**. Atlas nhân vật 1024×1024 lưới
4×4 (ô 256), 16 frame, pivot chân (baseline y=230 trong ô), facing RIGHT.
Nền parallax 1200×900/lớp. Kiểm tra baseline lệch giữa các frame trước khi nối animation.
