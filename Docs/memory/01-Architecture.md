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
