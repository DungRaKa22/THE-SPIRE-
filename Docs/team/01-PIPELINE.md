# 01 — Đường ống làm việc

Chốt với chủ dự án 20/09/2026: **kênh điều phối = GitHub Issues + PR**, **chỉ Arena có quyền
push**, **chất lượng trước**, **M0 và M1 chạy song song**.

```
1 SPEC      Tài liệu/mục kỹ thuật là nguồn chân lý (Antigravity hoặc Arena viết)
2 ISSUE     Arena mở issue = ticket, gán nhãn tầng/owner. Issue là bảng công việc DUY NHẤT
3 IMPLEMENT Agent sở hữu làm trong bản sao riêng, chạy cổng script tại máy mình
4 DELIVER   Agent nộp thư mục T-0xx/ (file hoàn chỉnh + report.md) → chủ dự án đưa vào ~/inbox/
5 INTEGRATE Arena copy vào repo, chạy cổng, commit "T-0xx: …", mở PR, cập nhật issue
6 REVIEW    Antigravity review đối kháng (comment thẳng vào PR trên GitHub) — bắt buộc với code
7 UNITY     Gom lô, chủ dự án chạy cổng Unity (Docs/team/04-UNITY-GATE.md)
8 MERGE     Arena merge, đóng issue, xoá inbox
```

Bước IMPLEMENT–DELIVER làm được ở bất kỳ máy nào, **không cần Unity**. Bước 7 là nút cổ chai duy nhất ⇒ gom lô.

## 2. Quyền của Arena trên GitHub (đo thực tế 20/09/2026)

| Việc | Được? | Ghi chú |
|---|---|---|
| `git push` nhánh `arena/01a0bf86-the-spire` | ✅ | Nhánh duy nhất được push |
| Tạo issue | ✅ | Issue = ticket, tạo một lần |
| Sửa / đóng / gắn nhãn / comment issue | ❌ | `Resource not accessible by integration` (403) |
| Tạo + sửa PR, gửi review trên PR | ✅ | Sửa body PR bằng `gh api -X PATCH /repos/.../pulls/<n>` |
| Merge PR | ⚠️ chưa xác nhận | Lỗi trả về là NOT_FOUND (không phải FORBIDDEN) ⇒ nhiều khả năng được |

**Hệ quả — cách vận hành board:**

1. **Issue là ticket bất biến.** Ai đọc cũng thấy yêu cầu gốc, nhưng **trạng thái sống** nằm ở
   `Docs/team/02-BACKLOG.md` (Arena sửa được). Đóng issue là việc của chủ dự án (hoặc bật lại
   quyền cho Arena).
2. Nhãn không gắn được ⇒ tầng/owner ghi thẳng trong **tiêu đề issue** (`T-009 [B/freebuff] — …`).
3. Muốn Arena tự đóng/comment/nhãn được thì **kết nối lại GitHub trong Arena** với quyền đầy đủ.
   Cho tới lúc đó mọi thứ vẫn chạy, chỉ thêm 1 bước tay cho chủ dự án.

### Bật ruleset bảo vệ `main` (chốt D6 = BẬT, chỉ chủ dự án làm được)

1. GitHub → repo `THE-SPIRE-` → **Settings** → **Rules** → **Rulesets** → **New branch ruleset**.
2. Name: `protect-main`; **Enforcement status: Active**.
3. **Target branches** → Add target → Include by pattern → `main`.
4. Tick **Require a pull request before merging** (đừng tick *Require approvals* — Arena không tự
   approve PR của chính mình, sẽ tự khoá mình).
5. Tick **Block force pushes** → **Create**.

Tác dụng: mọi thay đổi vào `main` phải đi qua PR; lịch sử không bị ghi đè; nhánh mà Unity của bạn
mở luôn là nhánh đã qua cổng. Arena push nhánh `arena/…` nên không bị ảnh hưởng.

## 3. Kênh nộp bài (vì chỉ Arena push được)

Agent **không** commit, **không** tạo nhánh, **không** push. Nộp bài = một thư mục:

```
T-0xx/
├── report.md                      # 4 mục theo mẫu dưới
└── files/                         # file HOÀN CHỈNH, giữ nguyên đường dẫn tương đối trong repo
    ├── Docs/TheSpire-Level-Sector4.md
    └── Tools/…
```

Luật nộp:
- **File hoàn chỉnh**, không phải diff từng dòng (tránh lệch whitespace khi ghép).
- **Ghi rõ baseline**: dòng đầu `report.md` là `baseline: <sha 7 ký tự>` của commit agent đã lấy làm
  gốc. Vì nộp file nguyên khối, nếu repo đã đổi sau sha đó thì Arena phải ghép tay — nên luôn
  `git pull` trước khi bắt đầu, và nộp sớm.
- Chỉ nộp file thuộc phạm vi ticket. Nộp thừa file = Arena trả lại.
- Chủ dự án lưu thư mục vào `~/inbox/T-0xx/` — **ngoài repo**, không commit vào Git.
- `report.md` đúng 4 mục: **Đã làm** · **Cổng đã chạy (dán nguyên văn output)** ·
  **Chưa kiểm chứng** · **Token đã dùng**.

Arena nhận → copy vào repo → chạy cổng → commit `T-0xx: <mô tả>` → mở PR → gán issue.

## 4. PR của Arena (nhánh duy nhất được push)

- Nhánh: `arena/01a0bf86-the-spire`. **Một PR tại một thời điểm**, merge xong mới mở PR kế tiếp
  (nhánh này là nhánh tích hợp cuộn).
- PR body luôn ghi: danh sách ticket trong PR, cổng đã chạy, cái gì chưa kiểm chứng, issue liên quan.
- **Cấm trong PR:** `ProjectSettings/**`, `Packages/**`, file > 1 MB, `.meta` sinh tự động
  (Arena tạo file mới thì Unity sinh `.meta` khi chủ dự án mở — ghi rõ ở mục "Chưa kiểm chứng").
- `Assets/Scenes/**.unity` chỉ được đổi bởi builder (T-302/T-304), không sửa tay.

## 5. Định nghĩa SẴN SÀNG (DoR) — issue chưa đủ 6 mục thì không giao

1. Mục tiêu một câu, đo được.
2. Đường dẫn deliverable cụ thể.
3. Mục tài liệu tham chiếu (số mục, không phải "đọc file X").
4. Cổng kiểm: script nào / menu Unity nào.
5. Non-goals: ≥ 1 dòng "không làm gì".
6. Phụ thuộc đã merge + ước lượng ≤ 400 dòng.

## 6. Định nghĩa XONG (DoD)

- [ ] Cổng script xanh, output dán **nguyên văn** trong `report.md` và trong PR.
- [ ] Đúng phạm vi file; không sửa file của agent khác.
- [ ] Code: chỉ dùng API **đã tồn tại** trong repo (liệt kê trong ticket), không bịa hàm.
- [ ] **Antigravity đã review đối kháng** (code + tài liệu số liệu) và comment trong PR.
- [ ] Nếu đổi luật/số liệu khoá → đã cập nhật `Docs/memory/**`, issue có nhãn `decision`.
- [ ] Mục "Chưa kiểm chứng" nêu rõ phần phải chờ cổng Unity.
- [ ] Không còn TODO không có issue.

## 7. Review đối kháng (chất lượng trước)

| Loại PR | Người review | Cách review |
|---|---|---|
| Tài liệu **có số liệu** (bảng bệ, toán cửa sổ) | Cổng script **+** Antigravity | Script kiểm số; Antigravity kiểm *giả định* sau con số |
| Code C# / editor tool | **Antigravity bắt buộc** | Comment trực tiếp vào PR trên GitHub |
| Tài liệu chữ, dọn repo | Arena | Đọc mắt |

Antigravity review **không cần quyền push** — chỉ cần đọc được repo/PR trên GitHub. Arena đọc
kết quả bằng `gh pr view <n> --comments` và chỉ merge khi mọi câu hỏi review đã được trả lời.

Câu hỏi review chuẩn (5 câu, không cần đọc dài):
1. PR có hiện thực đúng mục tài liệu đã trích không?
2. Có bịa API hoặc tự nghĩ ra tham số không?
3. Có phá hằng số khoá không?
4. Cổng script có chạy thật (có output) hay chỉ nói "đã chạy"?
5. Phạm vi có vượt ticket không (đặc biệt file scene, file của agent khác)?

## 8. Đồng bộ & chống conflict (nguồn tốn token lớn nhất)

- **Bản sao Unity của chủ dự án giữ ở `main`**, chỉ `git pull` khi chạy cổng.
- Agent làm trong **bản sao riêng, không mở Unity** (không cần `Library/`, tránh `.meta` sinh loạn).
  Vì agent nộp file hoàn chỉnh nên bản sao của họ luôn có thể bị xoá/tạo lại — không mất công.
- **Một agent / một file / một lượt.** Đây là luật quan trọng nhất khi chỉ có một người ghép bài:
  conflict ở bước 5 do Arena xử lý, và cứ mỗi conflict là một lượt làm lại tốn token của agent.
- Ticket đụng cùng file ⇒ **xếp hàng**, không chạy song song.

## 9. Khi `main` hỏng

1. Người phát hiện mở issue `main-broken` + dán log.
2. Arena sửa nhanh trên nhánh của mình hoặc revert commit gây lỗi.
3. Trong lúc đó mọi ticket mới dừng, không merge thêm.
