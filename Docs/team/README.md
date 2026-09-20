# THE SPIRE — Team OS (4 agent cùng một repo)

> Thư mục này là **luật vận hành** cho nhóm: 3 AI agent ngoài (Antigravity / Codex / Freebuff)
> + Arena (trưởng nhóm) + bạn (chủ dự án và là **cổng Unity duy nhất**).
> Mục tiêu: nhiều agent chạy song song mà **không đụng file của nhau**, **không tốn token vào việc
> đã có script kiểm được**, và **không bao giờ để `main` hỏng**.

## Đọc gì trước khi làm bất cứ việc gì

| Thứ tự | File | Vì sao |
|---|---|---|
| 1 | `Docs/memory/00-Project-Overview.md` | Dự án là gì, quyết định đã chốt (đừng hỏi lại) |
| 2 | `Docs/memory/01-Architecture.md` | **Hằng số khóa** + ngân sách nhảy + bẫy đã phát hiện |
| 3 | `Docs/memory/02-Progress.md` | Đang ở đâu, cái gì chưa làm |
| 4 | `Docs/team/00-CHARTER.md` | Vai trò, quyền sửa file của từng agent |
| 5 | `Docs/team/01-PIPELINE.md` | Vòng đời một ticket, DoR/DoD, luật PR |

## Một ticket đi qua 6 bước

```
SPEC (Tier A)  →  TICKET  →  IMPLEMENT (owner)  →  GATE (script)  →  REVIEW (Arena)  →  UNITY GATE (bạn)  →  MERGE
   Antigravity      Arena        Codex/Freebuff      jumpcheck.py        PR review        mở Unity 1 lần/tuần
```

Bước nào tốn token nhất thì bước đó phải có script kiểm trước. Bước 4 chạy **miễn phí** và đã
chứng minh giá trị: nó tìm ra 30 vấn đề trong tài liệu khu 1–3 mà không dùng token nào (xem
`Docs/TheSpireJumpCheck.txt`).

## Chỉ mục

| File | Nội dung |
|---|---|
| `00-CHARTER.md` | Vai trò 4 agent, ma trận sở hữu file, luật giao tiếp, quy tắc leo thang |
| `01-PIPELINE.md` | Vòng đời ticket, nhánh/PR, DoR/DoD, rubric review, đồng bộ git |
| `02-BACKLOG.md` | M0–M5 với mã ticket, tầng model, cổng kiểm, phụ thuộc |
| `03-TOKEN-BUDGET.md` | Định tuyến model theo loại việc, gói context chuẩn, phản mẫu, cách đo |
| `04-UNITY-GATE.md` | Checklist cổng Unity gom lô + cách báo cáo kết quả về repo |
| `05-KICKOFF-PROMPTS.md` | Prompt sẵn dán cho từng agent (copy nguyên khối) |

## Công cụ đã có

| Công cụ | Chạy | Việc |
|---|---|---|
| `Tools/jumpcheck.py` | `python3 Tools/jumpcheck.py [--strict]` | Kiểm mọi liên kết bệ trong doc khu 1–3 bằng công thức khóa; ghi `Docs/TheSpireJumpCheck.txt` |

## Ba câu hỏi mỗi ticket phải trả lời được

1. Việc này thuộc **tầng nào** (A: cần suy luận / B: khối lượng / C: script kiểm được)?
2. Cái gì **chứng minh nó đúng** mà không cần LLM đọc lại?
3. Agent nào **sở hữu file** này, và có ai khác đang cầm cùng file không?
