# 02 — Backlog & lộ trình

Trạng thái: `[ ]` chưa làm · `[~]` đang làm · `[x]` xong (đã qua cổng).
Tầng: **A** = cần suy luận, giao Antigravity/Codex · **B** = khối lượng, giao Freebuff.
Cổng: `jc` = `Tools/jumpcheck.py` · `ac` = `Tools/apicheck.py` (chưa có, T-004) ·
`u` = cổng Unity của bạn · `-` = review mắt thường.

## M0 — Team OS & cổng kiểm (làm ngay)

| ID | Việc | Tầng | Chủ | Deliverable | Cổng |
|---|---|---|---|---|---|
| T-001 | `[x]` Bộ luật nhóm + pipeline + backlog | — | Arena | `Docs/team/**` | - |
| T-002 | `[x]` Cổng kiểm ngân sách nhảy | — | Arena | `Tools/jumpcheck.py`, `Docs/TheSpireJumpCheck.txt` | jc |
| T-003 | `[ ]` Chuẩn hoá **quy ước bảng bệ v2**: cột `p_min` (đủ cả 2 điều kiện) + cột `p_trần` (giới hạn trần/che khuất) | A | Antigravity | mục mới trong `Docs/memory/01-Architecture.md` §quy cách bảng | jc --strict |
| T-004 | `[ ]` Cổng API: index symbol C# trong `Assets/**`, chặn tham chiếu hàm không tồn tại | B | Freebuff | `Tools/apicheck.py` | tự chạy trên repo hiện tại |
| T-005 | `[ ]` Issue template + label + board GitHub | — | Arena | `.github/`, labels | - |
| T-006 | `[ ]` Dọn repo: bỏ 5 file âm thanh trùng `Assets/Audio` ↔ `Assets/Resources`, untrack `Assets/_Recovery` | B | Freebuff | `.gitignore`, xoá trùng | u (Unity mở sạch) |
| T-007 | `[ ]` Sửa 4 lỗi hình học bảng khu 1 (011, 012 RISKY; 021, 026) | B | Freebuff | `Docs/TheSpire-Level-Sector1.md` | jc GEOM=0 |
| T-008 | `[ ]` Chốt 7 quyết định mở (xem PR mô tả) | — | Bạn + Arena | ghi vào `Docs/memory/00` | - |

> T-003 là ticket quan trọng nhất của M0: hiện 19 dòng ở khu 2 ghi `p` theo **độ cao** trong khi
> tài liệu tuyên bố `p` là "mức tối thiểu cần". Nếu không chốt quy ước trước, mỗi agent sẽ điền
> bảng khu 4–8 theo một kiểu khác nhau và validator sẽ nổ hàng loạt.

## M1 — Hoàn tất thiết kế 8 khu (đúng quyết định "tài liệu trước, code sau")

Mỗi khu 3 bước, **không song song hai agent trên cùng một file**:
`A1` khung mechanic + toán (Antigravity) → `B` điền bảng 40–45 bệ (Freebuff) → `A2` soát + Arena chạy jc.

| ID | Việc | Tầng | Chủ | Cổng |
|---|---|---|---|---|
| T-101 | `[ ]` Khu 4 Machine Age — băng chuyền ("kiểm soát vị trí tích lực") | A1 A2 / B | Antigravity / Freebuff | jc + bảng §1.5b |
| T-102 | `[ ]` Khu 5 Digital Revolution — bệ bật/tắt theo mẫu | A1 A2 / B | Antigravity / Freebuff | jc + bảng §1.5b |
| T-103 | `[ ]` Khu 6 Neon Megacity — booster + drone (cao trào 105 m) | A1 A2 / B | Antigravity / Freebuff | jc + bảng §1.5b |
| T-104 | `[ ]` Khu 7 The Singularity — vùng trọng lực (đổi trục độ khó) | A1 A2 / B | Antigravity / Freebuff | jc + bảng §1.5b |
| T-105 | `[ ]` Khu 8 The Sky — đoạn leo ngắn + 2 ending | A / B | Antigravity / Freebuff | jc |
| T-106 | `[ ]` Bảng tra "cửa sổ hành động" hợp nhất khu 2–8 (§1.5b bắt buộc) | A | Antigravity | - |
| T-107 | `[ ]` Gộp chỉ mục bệ toàn tháp (~330 bệ) + chốt luật coin | B | Freebuff | jc COIN |
| T-108 | `[ ]` Cập nhật `Docs/memory/01,02` sau mỗi khu | — | Arena | - |

## M2 — Code mechanic (⛔ chỉ bắt đầu khi bạn bật đèn xanh)

| ID | Việc | Tầng | Chủ | Cổng |
|---|---|---|---|---|
| T-201 | `[ ]` `MovingPiston` (kinematic + `MovePosition`, profile thang, `IMovingSurface`) | A | Codex | menu check mới + u |
| T-202 | `[ ]` `ConveyorBelt` (đổi điểm xuất phát tích lực, không đổi đường cong) | A | Codex | u |
| T-203 | `[ ]` `TogglePlatform` (viền đứt nét + chỗ quan sát an toàn) | B | Freebuff | u |
| T-204 | `[ ]` `JumpBooster` + `DronePlatform` | B | Freebuff | u |
| T-205 | `[ ]` `GravityZone` (chỉ giảm, có biên nhìn thấy) | A | Codex | u |
| T-206 | `[ ]` `EndingChoice` + mảnh lore | B | Freebuff | u |
| T-207 | `[ ]` `GameBalance` (khoá hằng số thành một chỗ) + `SpirePresentation` (thay `CyberpunkPresentation`) | A | Codex | u |
| T-208 | `[ ]` `EndingChoice`, checkpoint theo khu, kỷ lục theo khu | B | Freebuff | u |

## M3 — Dữ liệu & dựng scene (một người viết)

| ID | Việc | Tầng | Chủ | Cổng |
|---|---|---|---|---|
| T-301 | `[ ]` `SectorBlueprint` (ScriptableObject) theo §2.3 kế hoạch | A | Codex | ac |
| T-302 | `[ ]` `TheSpireBuilder` dựng từ blueprint, chạy lại được | A | Codex | u |
| T-303 | `[ ]` `SpireChecks` mở rộng từ `NeonAscentChecks`: mọi liên kết + ảnh cao trào mỗi khu | A | Codex | chạy batch |
| T-304 | `[ ]` Dựng `TheSpire.unity` + `SectorStreamer` + parallax | A | **Arena** | u |

## M4 — Kiểm chứng & tích hợp

| ID | Việc | Tầng | Chủ | Cổng |
|---|---|---|---|---|
| T-401 | `[ ]` Chạy `SpireChecks` toàn tháp, 100% liên kết tới được | A | Codex | batch |
| T-402 | `[ ]` 60 FPS, LOD collider, pooling coin/hạt | A | Codex | u |
| T-403 | `[ ]` Lưu/tiếp tục slot `Spire.v1`, 2 ending, ảnh chụp từng khu | B | Freebuff | u |
| T-404 | `[ ]` Playtest đầu–cuối, cân lại độ dài cú rơi | A | Antigravity | bạn |

## M5 — Art & âm thanh

| ID | Việc | Tầng | Chủ | Cổng |
|---|---|---|---|---|
| T-501 | `[ ]` Import `ClimberAtlas` (cắt 4×4, pivot chân, PPU 100), dựng Animator | B | Freebuff | u |
| T-502 | `[ ]` Prop 8 khu + kiểm baseline từng frame | B | Freebuff | u |
| T-503 | `[ ]` Nền parallax theo khu + đổi palette theo độ cao | B | Freebuff | u |
| T-504 | `[ ]` Ambience + nhạc theo khu, NPC, mảnh lore | B | Freebuff | u |
| T-505 | `[ ]` Build Windows cuối + playtest | A | Arena | bạn |

## Việc đang chờ quyết định của bạn (không ai được tự chốt)

1. `p` trong bảng bệ: sửa số cho khớp "mức tối thiểu đầy đủ", hay đổi nghĩa cột thành "mức theo độ cao"?
2. Luật coin khi có ~330 bệ: giữ 1 coin/5 bệ (~66 coin) hay 1 coin/8 bệ từ khu 4?
3. Có bật M2 (code mechanic) ngay hay giữ đúng "thiết kế xong 8 khu mới code"?
4. Cổng Unity mỗi tuần một lần, hay mỗi khi có ≥ 3 PR xếp hàng?
