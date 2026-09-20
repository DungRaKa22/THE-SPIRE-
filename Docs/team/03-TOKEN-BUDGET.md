# 03 — Ngân sách token & định tuyến model

## 1. Vì sao phải có file này

Một agent làm việc theo vòng: mỗi bước nó **đọc lại ngữ cảnh**. Chi phí ≈ `số bước × kích thước
ngữ cảnh`. Vì vậy tiết kiệm token **không phải** là viết prompt ngắn, mà là:

1. **Giảm số bước** — spec rõ tới mức agent không phải hỏi lại hoặc mò.
2. **Giảm ngữ cảnh mỗi bước** — gói context chuẩn, không bao giờ dán cả file.
3. **Đổi việc kiểm tra sang script** — thay vì model đắt đọc lại để tìm lỗi.
4. **Đưa việc khối lượng cho model rẻ** — và dùng script làm lưới an toàn.

Giá tham chiếu (đơn vị tương đối, 1 **CU** = 1k token tầng A):
tầng A ≈ 10–15 CU cho cùng lượng token so với tầng B ⇒ **một ticket tầng B làm sai rồi sửa vẫn rẻ
hơn một lượt tầng A làm lại**. Ngược lại: một quyết định mechanic sai ở tầng B là mất 10–30 CU
để phát hiện và sửa. Đó là ranh giới phân tầng.

## 2. Bảng định tuyến

| Loại việc | Tầng | Lý do |
|---|---|---|
| Toán cửa sổ thời gian, pha, chu kỳ | **A** | Sai thì không cổng nào bắt được |
| Kiến trúc (blueprint, streamer, save) | **A** | Sai thì phải đập đi làm lại nhiều file |
| Truy lỗi "im lặng" (Grounded, collision, thứ tự FixedUpdate) | **A** | Đã có 6 bẫy trong `memory/02` — cần suy luận |
| Code runtime vật lý/va chạm | **A** | Unity không chạy được trong cổng script |
| Editor builder dựng scene | **A** | Ảnh hưởng toàn dự án |
| **Điền bảng 40–45 bệ theo spec có sẵn** | **B** | Cổng `jc` kiểm được 100% |
| Đồng bộ/format tài liệu, dịch, danh sách prefab | **B** | Không có logic ngầm |
| Refactor lặp (đổi tên, thêm `RequireComponent`) | **B** | `ac` + review mắt |
| Import/cắt sprite, gán prefab theo checklist | **B** | Có checklist + ảnh đối chiếu |
| Dọn repo, `.gitignore`, file trùng | **B** | Việc cơ học |
| **Chạy `Tools/*.py`, so số, viết báo cáo** | **C** | 0 token model |

Luật cứng: **tầng B không nhận ticket liên quan tới vật lý chưa có cổng**. Ticket vật lý luôn tầng A.

## 3. Gói context chuẩn (đã đo kích thước thật)

Baseline bắt buộc cho mọi agent, mọi ticket — **≈ 4,7k token**, dán một lần ở đầu phiên:

| File | ~token |
|---|---:|
| `Docs/memory/00-Project-Overview.md` | 1.056 |
| `Docs/memory/01-Architecture.md` | 1.383 |
| `Docs/memory/02-Progress.md` | 1.220 |
| `Docs/team/00-CHARTER.md` | ~1.000 |

Gói theo việc — **thêm đúng phần cần**:

| Việc | Thêm vào | Tổng |
|---|---|---:|
| Thiết kế khu 4–8 (tầng A1) | `Docs/TheSpire-Remake-Plan.md` §1.2–§1.5b (trích, ~2,5k) | ~7k |
| Điền bảng bệ khu N (tầng B) | **chỉ §1–§2 của doc khu đó** (~1,5k) + 3 dòng bảng mẫu | ~6k |
| Viết component C# | `DummyController.cs` (~3,5k) + mục tài liệu liên quan | ~9k |
| Truy lỗi runtime | file lỗi + `memory/02` mục "bẫy đã phát hiện" | ~5k |

**Tuyệt đối không đưa vào context** (vô ích và đắt):
`Assets/Scenes/*.unity` (1–2,6 MB YAML) · `Docs/ArtPreviews/*.png` · `Docs/The-Spire-Y-Tuong-Game.md`
(~13,7k token, chỉ dùng khi viết lore) · `Packages/packages-lock.json` · bất kỳ `.meta` ·
`Assets/Resources/**` · log build.

**Luật vàng:** không dán **chính cái bảng mà agent đang viết**. Chỉ đưa tiêu đề cột + 3 dòng mẫu.
Bảng khu 3 dài 12,6k token — đưa cả vào là phí 4 lượt làm việc.

## 4. Phản mẫu (và giá phải trả)

| Phản mẫu | Hệ quả |
|---|---|
| Để hai agent "trao đổi" để thống nhất | Gấp 10–50× một lần bàn giao bằng file |
| Dán cả repo / cả doc khu vào prompt | Ngữ cảnh phình mỗi bước, không thêm thông tin |
| Agent tự mò quy ước thay vì đọc `memory/01` | Sinh dữ liệu kiểu khác → cổng fail → sửa tốn gấp 3 |
| Giao tầng B việc "tự nghĩ ra cân bằng" | Không cổng nào bắt được cho tới khi người chơi thử |
| Review từng file bằng model đắt | Cổng script làm được, miễn phí |
| Ticket mơ hồ ("làm khu 4 cho tốt") | Agent hỏi lại 3–5 lượt = 5× chi phí |
| Sửa tài liệu người khác cho nhanh | Conflict + mất dấu vết, tốn hơn cả làm lại |

## 5. Ngân sách theo milestone (ước lượng, tính bằng CU)

| Milestone | Tầng A | Tầng B | Ghi chú |
|---|---:|---:|---|
| M0 Team OS | 3 | 4 | Đã phần lớn xong, chủ yếu là công cụ |
| M1 (1 khu) | 12 (A1+A2) | 6 (bảng bệ) | × 5 khu. **Đây là milestone tốn nhất** |
| M2 (1 component) | 10 | 3 | Chỉ khi đã có spec từ M1 |
| M3 dựng scene | 15 | 5 | Blueprint + builder + checks |
| M4 kiểm chứng | 8 | 6 | Phần lớn là chạy cổng + sửa |
| M5 art/âm | 3 | 20 | Tầng B làm hết phần cơ học |

Ngân sách giữ cho một khu M1 ≈ **18 CU**. Vượt 1,5× ⇒ dừng, xem lại spec (thường là spec thiếu,
không phải agent dở).

## 6. Cách đo (bắt buộc, để lần sau rẻ hơn)

Mỗi PR ghi 1 dòng cuối: `tokens: in≈X out≈Y (agent), CU≈Z`.
Arena ghi lại vào `Docs/team/02-BACKLOG.md` cuối mỗi milestone. Sau 2 milestone ta biết **loại
việc nào nên giao tầng nào**, thay vì đoán.
