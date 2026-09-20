# 00 — Tổng quan dự án The Spire (đọc file này trước)

## Dự án là gì

Repo **JumpDummy** (Unity **6000.5.7f1**, 2D, bàn phím) đang được **remake** thành game
**THE SPIRE — Tháp Tiến Hóa**: platformer leo cao nhảy tích lực kiểu Jump King, đi qua
7 thời kỳ công nghệ + The Sky. Ý tưởng gốc đầy đủ nằm ở `Docs/The-Spire-Y-Tuong-Game.md`
(bản 1.0 — nguồn chân lý về lore, 7 khu, 2 kết thúc Ascend/Return).

Tham chiếu Jump King: `C:\Documents\game 3d\Jump-King-main\Jump-King-main` — 43 màn
1200×900 px, hình học tầng trong `LevelSetupFunction.js`, ảnh tham chiếu trong
`images/levelImages`. **Ảnh Jump King chỉ dùng tham chiếu bố cục, không đưa vào build.**

## Quyết định đã chốt (đừng hỏi lại)

1. **Hiện tại dừng ở tài liệu thiết kế** — người chơi quyết định khi nào code scene mới.
   Riêng sửa lỗi runtime trong code hiện có thì làm bình thường.
2. **Art: người dùng tự sinh bằng công cụ image-gen** (GPT), tôi viết prompt
   (`Docs/TheSpire-ArtPrompts.md`) và sẽ cắt sprite/pivot/Animator sau. Tôi không vẽ ảnh.
3. **Nhân vật mới — The Climber** (người nhặt phế liệu + Neural Jump Drive ở chân).
4. **Mục tiêu cuối: 1 scene duy nhất** `TheSpire.unity` (~525 m tháp, 8 khu), level dạng
   dữ liệu `SectorBlueprint`, khu cũ (NeonAscent, JumpLab…) giữ lại làm phòng thử.
5. Thiết kế đủ 8 khu bằng tài liệu trước, rồi mới dựng scene.

## Chỉ mục tài liệu

| File | Nội dung |
|---|---|
| `Docs/The-Spire-Y-Tuong-Game.md` | Ý tưởng gốc (lore, 7 khu, 2 ending) — nguồn chân lý |
| `Docs/TheSpire-Remake-Plan.md` | Kế hoạch tổng: thông số khóa, 8 khu, lộ trình 7 giai đoạn |
| `Docs/TheSpire-ArtPrompts.md` | Prompt sinh ảnh + quy cách import (Point filter, PPU 100) |
| `Docs/TheSpire-Level-Sector1.md` | Khu 1 Forgotten Kingdom — 40 bệ (01–40) |
| `Docs/TheSpire-Level-Sector2.md` | Khu 2 Age of Steam — 43 bệ (41–83), toán piston |
| `Docs/TheSpire-Level-Sector3.md` | Khu 3 Electric Age — 42 bệ (84–125), toán trạng thái |
| `Docs/ArtPreviews/` | Ảnh GPT đã sinh (v1) + prompt tương ứng của từng ảnh |

## Cách làm việc

- Trao đổi bằng **tiếng Việt**; code/comment bằng tiếng Anh.
- Mọi cú nhảy trong thiết kế phải **tính bằng công thức thật**, không ước lượng
  (xem `01-Architecture.md` mục ngân sách nhảy).
- Kiểm tra tự động theo convention menu **JumpDummy → …** trong `Assets/Editor`,
  kết quả ghi `Docs/*Validation.txt`. Batch mode Unity không dùng được trên máy này
  (khởi động >5 phút) — kiểm chứng runtime là việc của người dùng trong Play Mode.
- Editor Unity thường đang mở → biên dịch tự chạy khi focus lại; không chạy Unity
  batch song song, cẩn thận `Temp/UnityLockfile` sót sau khi kill tiến trình.

Chi tiết kỹ thuật: `01-Architecture.md`. Trạng thái công việc: `02-Progress.md`.
