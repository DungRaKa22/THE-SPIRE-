# 01 — Đường ống làm việc

## 1. Vòng đời một ticket

```
1 SPEC      Antigravity/Arena viết hoặc cập nhật tài liệu (file .md) — không code ở bước này
2 TICKET    Arena tạo issue: mục tiêu 1 câu, deliverable, mục tài liệu tham chiếu, cổng, non-goals
3 IMPLEMENT Agent sở hữu làm trên nhánh riêng, chạy cổng script tại máy mình
4 GATE      python3 Tools/jumpcheck.py  (và các cổng khác tuỳ ticket) — phải xanh mới mở PR
5 REVIEW    Arena đọc PR: tính đúng, phạm vi, trích dẫn tài liệu, có mục "Chưa kiểm chứng" không
6 UNITY     Gom vào cửa sổ cổng tuần (Docs/team/04-UNITY-GATE.md) — chỉ bạn chạy được
7 MERGE     Arena merge sau khi có nhãn gate:passed
```

Bước 3–4 làm được ở bất kỳ máy nào (không cần Unity). Bước 6 là **nút cổ chai duy nhất** ⇒ gom lô.

## 2. Nhánh & PR

| Loại | Tên nhánh | Ví dụ |
|---|---|---|
| Tài liệu/thiết kế | `spec/T-102-sector5` | 1 khu = 1 nhánh |
| Code runtime | `code/T-201-movingpiston` | 1 component = 1 nhánh |
| Sửa lỗi | `fix/T-003-table-convention` | |
| Arena (bắt buộc của phiên này) | `arena/01a0bf86-the-spire` | Arena chỉ push được nhánh này |

Luật PR:
- **1 ticket = 1 PR.** ≤ 400 dòng đổi (không tính bảng số liệu dài trong doc).
- Trước khi mở PR: `git fetch origin && git rebase origin/main`. Nhánh sống ≤ 5 ngày.
- **Cấm trong PR:** `Assets/Scenes/**.unity`, `ProjectSettings/**`, `Packages/**`, `.meta` mới do
  agent tự tạo, file > 1 MB. Cần thì mở ticket riêng cho Arena.
- Mọi PR phải điền `.github/PULL_REQUEST_TEMPLATE.md`, gồm mục **"Chưa kiểm chứng"** — thiếu mục
  này trả lại không cần đọc tiếp.
- Nhãn: `tier:A` `tier:B` `owner:antigravity` `owner:codex` `owner:freebuff` `sector:4`
  `gate:script` `gate:unity` `gate:passed` `decision`.

## 3. Định nghĩa SẴN SÀNG (DoR) — ticket chưa đủ 6 mục thì không bắt đầu

1. Mục tiêu một câu, đo được.
2. Đường dẫn deliverable cụ thể (`Docs/...`, `Assets/...`).
3. Mục tài liệu tham chiếu (số mục, không phải "đọc file X").
4. Cổng kiểm: script nào / menu Unity nào.
5. Non-goals: ít nhất 1 dòng "không làm gì".
6. Phụ thuộc đã merge xong + ước lượng ≤ 400 dòng.

## 4. Định nghĩa XONG (DoD)

- [ ] Cổng script xanh, dán **nguyên văn** dòng kết luận vào PR.
- [ ] Đúng phạm vi file (không sửa file của người khác).
- [ ] Code: chỉ dùng API **đã tồn tại** trong repo (liệt kê trong ticket), không bịa hàm.
- [ ] Nếu đổi luật/số liệu khóa → đã cập nhật `Docs/memory/**` và có nhãn `decision`.
- [ ] Mục "Chưa kiểm chứng" ghi rõ cái gì phải chờ cổng Unity.
- [ ] Không còn TODO không có ticket.

## 5. Rubric review (5 câu, không cần đọc dài)

1. PR có **hiện thực đúng mục tài liệu** đã trích không?
2. Có **bịa API** hoặc tự nghĩ ra tham số không?
3. Có **phá hằng số khóa** không?
4. Cổng script có thật sự chạy (có output) hay chỉ nói "đã chạy"?
5. Phạm vi có **vượt ticket** không (đặc biệt: file scene, file của agent khác)?

## 6. Đồng bộ git (chống conflict — nguồn tốn token lớn nhất)

- Bắt đầu: `git fetch origin && git rebase origin/main`.
- Trước PR: rebase lại lần nữa. Sau khi merge: xoá nhánh.
- **Bản sao Unity của bạn giữ ở `main`.** Các agent làm trên clone riêng, không mở Unity trong
  lúc làm việc (tránh Library nặng + tránh `.meta` sinh loạn). Agent nào cần Unity thì chỉ để
  chạy cổng (trong `04-UNITY-GATE.md`).
- Conflict xảy ra ⇒ **người mở PR tự giải quyết**, không nhờ Arena rebase (Arena chỉ push được
  nhánh `arena/01a0bf86-the-spire`).
- Nếu hai PR cùng sửa một file: Arena merge cái cũ trước, cái sau tự rebase lại.

## 7. Ai làm gì khi `main` hỏng

1. Người phát hiện mở issue `main-broken`, dán log/kết quả.
2. Arena tạo nhánh `arena/01a0bf86-the-spire` sửa nhanh hoặc revert PR gây lỗi.
3. Trong lúc đó **mọi ticket mới tạm dừng**, không ai merge thêm.
