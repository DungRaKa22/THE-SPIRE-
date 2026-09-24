# 00 — Hiến chương nhóm

## 1. Năm nguyên tắc bất di bất dịch

1. **Một người viết, một file.** Không hai agent cùng sửa một file. Muốn sửa file của người khác
   thì mở ticket, không tự sửa.
2. **Tài liệu là nguồn chân lý.** Code phải trích dẫn mục tài liệu nó hiện thực (ví dụ
   `Docs/TheSpire-Level-Sector2.md §3.1`). Không có mục tài liệu ⇒ viết tài liệu trước.
3. **Hằng số vật lý khóa cứng.** `gravityScale 3.0`, `chargeDuration 0.85`, jump 4→13,
   ngang `6 × Lerp(0.4,1,p)`, `wallBounceRetention 0.8`, không điều khiển trên không.
   Ai muốn đổi phải mở **ticket quyết định** và cả bạn + Arena đồng ý.
4. **Cổng script trước, review người sau.** Việc gì `Tools/*.py` kiểm được thì không tiêu token
   model để kiểm.
5. **`main` luôn mở được trong Unity.** Không merge khi chưa qua cổng Unity của bạn.

## 2. Vai trò

| Thành viên | Tầng | Sở trường giao | Không giao |
|---|---|---|---|
| **Arena** (trưởng nhóm & tích hợp) | — | Spec, issue/ticket, **người DUY NHẤT được push**, ghép bài từ inbox, chạy cổng, xử lý conflict, `Docs/memory/**`, `Tools/**`, `.github/**`, sinh scene từ dữ liệu | Không viết thay mechanic phức tạp |
| **Antigravity** (Gemini 3.x) | A — suy luận | Toán mechanic, bảng cửa sổ thời gian §1.5b, kiến trúc, truy lỗi "im lặng", **review đối kháng mọi PR code** | Điền bảng 40 dòng, doc formatting |
| **Codex** (GPT-6-class) | A — triển khai | Component runtime C# + editor tooling theo spec, diff nhỏ, menu check | Viết tài liệu thiết kế màn, sửa scene bằng tay |
| **Freebuff** (DeepSeek) | B — khối lượng | Điền bảng bệ theo spec, đồng bộ doc, danh sách prefab, refactor lặp, dịch/format | Quyết định mechanic, code vật lý chưa có test |
| **Bạn** (chủ dự án) | — | **Cổng Unity duy nhất**, duyệt ticket quyết định, chốt hướng | Không phải sửa lỗi đánh máy trong doc |

Nguyên tắc chọn tầng: **việc nào bước 4 (`Tools/*.py`) kiểm được thì giao tầng B; việc nào sai
thì không ai phát hiện được thì giao tầng A.**

## 3. Ma trận sở hữu file

| Đường dẫn | Người viết duy nhất | Ghi chú |
|---|---|---|
| `Docs/memory/**`, `Docs/team/**`, `Tools/**`, `.github/**` | **Arena** | Bộ nhớ + luật nhóm |
| `Docs/TheSpire-Level-Sector*.md` | **1 agent / 1 khu / 1 lượt** | Ghi rõ ai giữ khu nào trong ticket |
| `Docs/TheSpire-Remake-Plan.md`, `Docs/TheSpire-ArtPrompts.md` | Arena (sửa) hoặc agent được giao | Thay đổi lớn cần ticket |
| `Assets/Scripts/*.cs` (mechanic mới) | **Codex** | Freebuff chỉ refactor khi có ticket tầng B |
| `Assets/Scripts/Cyberpunk/*.cs` (trình bày) | Codex hoặc Arena | Đổi tên theo The Spire ở T-207 |
| `Assets/Editor/**` | **1 agent / 1 file / 1 lượt** | Builder ghi cả scene — cực kỳ cấm chồng |
| `Assets/Scenes/**.unity` | **Không ai sửa tay** | Sinh bằng builder từ dữ liệu (T-304) |
| `Assets/Prefabs/**`, `Assets/Art/**`, `Assets/*.meta` | Arena | Tạo file mới = tạo GUID mới, chỉ một người làm |
| `ProjectSettings/**`, `Packages/**` | Arena | Đổi phải có ticket quyết định |
| `Assets/Resources/**` | Arena | Đang trùng với `Assets/Audio/**` (T-006) |

## 4. Luật giao tiếp (đây là chỗ tiết kiệm token nhiều nhất)

- **Không chat LLM–LLM.** Mọi bàn giao đi qua file hoặc comment issue/PR. Hai model nói chuyện
  với nhau tốn gấp 10–50 lần một lần bàn giao bằng file, và không để lại dấu vết.
- **Thứ tự bàn giao chuẩn:** spec (file) → ticket (issue) → PR (code/data) → kết quả cổng (file)
  → review (comment PR) → kết quả Unity (`Docs/*Validation.txt`).
- **Trả lời ngắn:** trong PR chỉ viết 4 mục — Đã làm / Cổng chạy / Chưa kiểm chứng / Token đã dùng.
- **Hỏi khi mơ hồ, nhưng hỏi một lần:** gom câu hỏi vào issue, không hỏi từng câu.
- **Không tự đổi phạm vi:** phát hiện việc ngoài ticket ⇒ mở issue mới, đừng làm luôn.

## 5. Ai push, ai không

**Chỉ Arena push được** (nhánh `arena/01a0bf86-the-spire`). Ba agent ngoài **không commit, không
tạo nhánh, không push** — các em nộp thư mục `T-0xx/` gồm file hoàn chỉnh + `report.md`, chủ dự án
đưa vào `~/inbox/T-0xx/`, Arena ghép vào repo và mở PR. Chi tiết định dạng: `01-PIPELINE.md` §1.

Hệ quả cần nhớ: agent **không tự giải quyết conflict được** ⇒ luật "một agent / một file / một
lượt" ở §3 là bắt buộc, không phải khuyến nghị. Ticket đụng cùng file phải **xếp hàng**.

Antigravity review đối kháng **không cần quyền push** — chỉ cần đọc PR trên GitHub và comment
vào đó; Arena đọc bằng `gh pr view <n> --comments`.

## 6. Leo thang & dừng

| Tình huống | Luật |
|---|---|
| Cùng một cổng fail 2 lần | Dừng. Chuyển ticket lên tầng A, kèm **log lỗi nguyên bản** |
| Ticket vượt 2× ước lượng dòng code | Cắt nhỏ, mở ticket con |
| Agent nghĩ cần sửa file ngoài phạm vi | Dừng, mở issue cho Arena |
| Nghi ngờ tài liệu sai | Đừng sửa tài liệu người khác. Mở issue + ghi số liệu chứng minh |
| Bế tắc kỹ thuật > 1 lượt | Ghi 3 dòng "đã thử gì / kết quả / cần gì" rồi trả ticket |
