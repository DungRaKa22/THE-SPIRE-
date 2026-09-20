# 04 — Cổng Unity (bạn là người duy nhất chạy được)

## 1. Vì sao phải gom lô

Không agent nào trong nhóm chạy được Unity: Arena không có Unity trong sandbox, các agent khác
chỉ làm repo. Đồng thời Unity trong repo này là **studio duy nhất** (Editor đang mở, biên dịch tự
động khi focus) và batch mode trên máy bạn mất > 5 phút. Vì vậy:

- **Một cửa sổ cổng = một buổi** (khuyến nghị: cuối tuần, hoặc khi có ≥ 3 PR xếp hàng).
- Giữa hai cửa sổ, các agent vẫn làm việc được: M1 (tài liệu) và các cổng script **không cần Unity**.
- Không merge PR code vào `main` trước khi qua cổng (trừ tài liệu thuần).

## 2. Checklist chạy cổng (copy vào issue cổng mỗi lần)

```
CỔNG UNITY #<n> — <ngày>
Bản build: main @ <sha ngắn>
PR xếp hàng: #<a> <tên>, #<b> <tên>, ...

[ ] 1. git checkout main && git pull   (KHÔNG chuyển nhánh nào khác ở bản sao này)
[ ] 2. Mở Unity, đợi biên dịch xong. Lọc Console: "error CS"   → phải 0 dòng
[ ] 3. Mở scene đang bị ảnh hưởng:  JumpDummy > Open Neon Ascent  (hoặc scene tương ứng)
[ ] 4. Play 60 giây: đi, tích lực, nhảy, rơi. Xem có lỗi runtime trong Console không
[ ] 5. Chạy menu kiểm tra của ticket:  JumpDummy > <tên menu>
[ ] 6. Đọc file kết quả được ghi vào Docs/<...>.txt, dán nguyên văn vào issue
[ ] 7. Nếu ticket có thay đổi hình ảnh: chụp 1 ảnh Game view 16:9, kéo vào issue

Kết quả: PASS / FAIL (dán log)  —  ai đó gắn nhãn gate:passed rồi merge
```

## 3. Cách báo cáo lại (ngắn, để không tốn token của cả nhóm)

Dán đúng 4 mục, không cần ảnh toàn màn hình:

```
CỔNG #<n>: <PASS|FAIL>
1. Biên dịch: 0 lỗi | <dán dòng lỗi đầu tiên>
2. Unity Console khi Play: sạch | <dán 5 dòng đầu>
3. Menu check: <tên> → <dán dòng kết luận trong Docs/*.txt>
4. Cảm giác chơi (1 câu, chỉ khi có mechanic mới): <...>
```

Mục 4 là thứ **chỉ bạn đánh giá được** — không script nào thay được. Một câu ngắn, ví dụ:
"piston đi lên mượt, nhưng đứng trên đỉnh thấy hơi trượt".

## 4. Việc bạn KHÔNG phải làm

- Không sửa số liệu trong doc khu (mở issue, có cổng `jc` lo).
- Không rebase nhánh của agent (người mở PR tự làm).
- Không đọc diff dài: PR phải có mục "Chưa kiểm chứng" để bạn biết đúng chỗ cần thử.
- Không phải nhớ luật — checklist mục 2 dán lại được mỗi lần.

## 5. Ngoại lệ cổng Unity (được merge trước)

- Tài liệu `.md` thuần trong `Docs/` (kể cả khu 1–3 số liệu, vì có `jc`).
- `Tools/*.py` (chạy bằng Python, có self-check).
- `.gitignore`, `.github/**`.

Mọi thứ đụng `Assets/**` đều phải qua cổng. Trường hợp T-006 (dọn file âm thanh trùng) **phải**
qua cổng vì Unity sẽ nhập lại tài nguyên.
