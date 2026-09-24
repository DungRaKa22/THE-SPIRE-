# 02 — Tiến độ (cập nhật: 20/09/2026)

## Đã xong

### Vận hành nhóm (mới 20/09/2026)
- ✅ **Sổ tay vận hành `Docs/team/06-WORKFLOW.md`** — 4 nơi chứa file (GitHub / máy Unity / sàn
  Arena / máy agent), hai nhánh, luồng A (tài liệu, không cần Unity) và luồng B (code, cổng Unity
  theo lô), quy tắc `.meta`, bảng lệnh theo vai, xử lý sự cố, kế hoạch một tuần, checklist dán tường.
- ✅ `Tools/inbox.py` — ghép bài nộp của agent: dry-run mặc định, chặn `Assets/Scenes/**`,
  `ProjectSettings/**`, file ngoài phạm vi; tự sinh `.meta` cho `.cs` mới; tự chạy `jumpcheck.py`.
- ✅ **Repo đang PUBLIC** (agent clone đọc không cần token) và **Arena merge được PR** (kiểm chứng
  bằng lỗi "still a draft" chứ không phải 403).
- ✅ `Docs/team/` — hiến chương 4 agent, ma trận sở hữu file, pipeline 6 bước, DoR/DoD,
  định tuyến model theo tầng A/B, quy trình cổng Unity gom lô, prompt khởi động cho 3 agent.
- ✅ **Bảng công việc = GitHub Issues** (8 issue: #2 T-003, #3 T-003R, #4 T-009, #5 T-004,
  #6 T-101, #7 T-102, #8 T-006, #9 T-201). **Hạn chế quyền đã đo:** Arena push + tạo issue +
  tạo/sửa PR + review PR được; **không** comment/đóng/gắn nhãn issue (403) ⇒ trạng thái sống ghi ở
  `Docs/team/02-BACKLOG.md`, chủ dự án đóng issue. Muốn tự động hoá hoàn toàn thì kết nối lại GitHub.
- ✅ **Quy ước bảng bệ v2 (T-003)** — chốt trong `Docs/memory/01-Architecture.md`: cột `p` cũ đổi
  thành `p_min` (mức tối thiểu đủ **cả** độ cao và tầm xa), thêm `p_max` (giới hạn trần/che khuất),
  luật "liên kết căng" khi `p_max − p_min < 0.10`, kèm 2 ví dụ tính tay khớp công cụ.
- ✅ `Tools/jumpcheck.py` — cổng kiểm ngân sách nhảy chạy bằng Python (0 token), ghi
  `Docs/TheSpireJumpCheck.txt`. Chạy lần đầu trên khu 1–3 (125 bệ) đã phát hiện:
  **HARD=0** (không có liên kết nào bất khả thi — tin tốt), **GEOM=4** lỗi hình học bảng ở khu 1
  (2 dòng ghi Δx **thấp hơn** thực tế ⇒ bệ khó hơn tài liệu: `011 Choir Loft`, `012 Knight Statue`),
  **CONV=19** dòng ghi `p` theo mức tối thiểu **chỉ độ cao** trong khi tài liệu tuyên bố `p` là
  mức tối thiểu đầy đủ (khu 2: 17/48 dòng), **WEAK=7** dòng không khớp cách tính nào.

### Quyết định của chủ dự án (20/09/2026)
- ✅ **Bỏ hoàn toàn cơ chế coin** (chỉ là demo, THE SPIRE không có điểm số). Đã dọn coin khỏi:
  `Docs/TheSpire-Level-Sector1/2/3.md`, `Docs/TheSpire-Remake-Plan.md` (§2.3 blueprint, §2.6, §5),
  `Docs/TheSpire-ArtPrompts.md` (HUD), `README.md`, `Docs/memory/*`. Còn lại phải xoá trong code:
  `PlatformCoins.cs`, `PlatformCoinPickup.cs`, SCORE trong `CyberpunkPresentation`, phần coin trong
  `GameSession` và `NeonAscentChecks` → **ticket T-210**, để lại cho giai đoạn code.
- ✅ **Chưa code gì mới cho tới khi thiết kế đủ 8 khu** (M2 vẫn đóng).
- ✅ **Cổng Unity theo lô (D4)**: gom các PR đụng `Assets/**` rồi chủ dự án mở Unity một lần;
  PR tài liệu/tool không cần cổng.
- ✅ **Bật ruleset bảo vệ `main` (D6)**: chủ dự án tạo (Arena không có quyền), hướng dẫn ở
  `Docs/team/01-PIPELINE.md` §2.
- ✅ T-005 một phần: board Issues đã tạo (#2–#9); không gắn được nhãn (403) ⇒ ghi nhãn trong
  `Docs/team/02-BACKLOG.md`.

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

- ⏳ **Ticket T-003 (tầng A):** chốt **quy ước bảng bệ v2** — định nghĩa cột `p` và cách ghi giới
  hạn trần/che khuất. Phải xong **trước khi** ai đó điền bảng khu 4–8, nếu không mỗi agent sẽ
  theo một quy ước khác nhau. Ticket T-007 (tầng B) sửa 4 lỗi hình học khu 1.
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
