# 02 — Tiến độ (cập nhật: 20/09/2026)

## Đã xong

### Tài liệu thiết kế
- ✅ Kế hoạch tổng `TheSpire-Remake-Plan.md` — thông số khóa, 525 m, lộ trình,
  §1.5b "cửa sổ thời gian" làm thước đo chung, quyết định đã chốt ghi ở đầu file.
- ✅ Bộ prompt art `TheSpire-ArtPrompts.md` (atlas Climber 16 frame, prop 8 khu,
  nền parallax, biểu tượng mặt trời, HUD + quy cách import).
- ✅ Khu 1 Forgotten Kingdom — 40 bệ (01–40), 3 loại nhảy J1–J3, bài học trần thấp
  `Machicolation` (cửa sổ p ∈ [0.64, 0.85]), 2 liên kết vượt p=0.80 (bệ 16, 28).
- ✅ Khu 2 Age of Steam — 43 bệ (41–83), 3 profile piston + 5 quy tắc pha,
  đường tắt Vent Updraft ngoài đường tới hạn.
- ✅ Khu 3 Electric Age — 42 bệ (84–125), 3 mạch E-A/E-B/E-C, quy tắc tồn tại
  `T/4 ≤ W(0)`, các "cửa nghỉ 270°".

### Code (đã sửa lỗi im lặng)
- ✅ Lỗi #2: `LastImpulseTime` + `ApplyImpulse` làm mốc xung chung; `CyberRunnerVisual`
  đọc mốc mới → cú hất điện phát animation Bounce đúng.
- ✅ Lỗi #1: `ElectricPlatform` poll bằng `Physics2D.OverlapBox` mỗi FixedUpdate
  thay vì chờ `OnCollisionEnter2D` (bị `if (Grounded) return;` chặn im lặng).
  `ElectricCircuit` đồng hồ chung + `RequireComponent` khung vỉa.
- ✅ Lỗi piston: `IMovingSurface` + `CheckGrounded` đo đất theo vận tốc tương đối —
  mở đường cho khu 2. Doc khu 2 mục 5 đánh dấu ĐÃ SỬA kèm yêu cầu kinematic+MovePosition.
- ✅ Biên dịch Unity thật pass 0 lỗi (3 file mới vào csproj).

### Art
- 🟡 GPT đã sinh v1: `Docs/ArtPreviews/` có ClimberAtlas-v1, ForgottenKingdomPlatforms-v1,
  ForgottenKingdomDecor-v1 (kèm prompt của từng ảnh). **Chưa import vào Assets/Art,
  chưa kiểm baseline, chưa có phản hồi người dùng về chất lượng.**

## Chưa làm

- ⏳ **Chạy play-check khu điện trong Play Mode** (menu JumpDummy → Run Electric
  Platform Checks) — chỉ người dùng làm được; kết quả về
  `Docs/TheSpireElectricValidation.txt`. Cho tới khi đó cơ chế hất điện chỉ được
  chứng minh bằng compile.
- ⏳ Thiết kế khu 4 Machine Age (băng chuyền — trục "kiểm soát vị trí tích lực"),
  rồi 5, 6, 7, Sky. Sau đó gộp index bệ toàn tháp.
- ⏳ `MovingPiston` thật (kinematic, trapezoid) — prototype để chứng minh sửa lỗi
  Grounded chạy runtime.
- ⏳ `SectorBlueprint` + builder + `SpireChecks` (mở rộng từ NeonAscentChecks) +
  dựng `TheSpire.unity` — chặng code lớn, đợi đủ thiết kế 8 khu.
- ⏳ Import art khi người dùng chốt ảnh (cắt sprite, pivot, Animator).

## Bẫy đã phát hiện (đừng dẫm lại)

1. `OnCollisionEnter2D` mở đầu `if (Grounded) return;` → mọi logic "hất khi đang đứng"
   phải poll, không chờ event.
2. `GroundFilter.useTriggers = false` → bệ đứng được phải là collider thường, không
   phải trigger.
3. Điều kiện đất phải theo vận tốc tương đối; piston phải kinematic + MovePosition.
4. Thứ tự chạy FixedUpdate giữa bệ và người chơi không đảm bảo → `ApplyImpulse` tự
   đặt `ignoreGroundUntil` để không bị ghi đè velocity.
5. Cửa sổ pha khi bệ nguồn tĩnh vẫn phải trừ `t₂` (0.44–0.56 s) — sai chỗ này là
   thổi phồng cửa sổ gấp đôi ở cao trào.
6. Batch Unity trên máy này treo >5 phút — mọi kiểm chứng runtime qua người dùng.
