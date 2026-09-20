# 06 — Sổ tay vận hành: luồng làm việc & đồng bộ GitHub ↔ máy

> Đọc file này khi bạn muốn biết **chính xác chuyện gì xảy ra ở đâu, ai gõ lệnh gì, file đi từ đâu
> tới đâu**. Các file khác trong `Docs/team/` nói *luật*; file này nói *thao tác*.

---

## 1. Bốn nơi chứa file (và không nơi nào là bản phụ)

```
┌─────────────────────────────┐        ┌──────────────────────────────────────┐
│ 1. GITHUB (nguồn chân lý)   │        │ 2. MÁY BẠN — nơi DUY NHẤT có Unity   │
│ DungRaKa22/THE-SPIRE-       │        │ C:\git\THE-SPIRE-                    │
│  • main        = đã kiểm    │◄──────►│  • làm việc trên main                │
│  • arena/01a0bf86-the-spire │  pull  │  • khi test PR: đổi sang nhánh tạm   │
│    = nhánh tích hợp         │  push  │  • Library/, Temp/ (không commit)    │
│  • Issues = ticket          │        │  • chạy cổng Unity                   │
│  • PR = chỗ review + merge  │        └──────────────────────────────────────┘
└─────────────────────────────┘                        ▲
        ▲                ▲                             │ bạn dán kết quả cổng
        │ push           │ push (chỉ tài liệu/PR)      │
        │                │                             │
┌───────┴──────────────┐ │        ┌───────────────────┴──────────────────────┐
│ 3. SÀN ARENA (tôi)   │ │        │ 4. BẢN SAO CỦA AGENT (Antigravity/Codex/ │
│ /home/user/THE-SPIRE-│ │        │    Freebuff) — chỉ ĐỌC, không ghi được   │
│  • sửa file, chạy    │ │        │  • git clone repo public (không cần token)│
│    cổng script       │ └───────►│  • viết file mới vào T-0xx/files/        │
│  • gh pr create/merge│          │  • chạy python Tools/*.py tại máy nó     │
└──────────────────────┘          └──────────────────────────────────────────┘
                                                     │
                       bạn copy thư mục T-0xx  ───────┘
                       vào  C:\git\inbox\T-0xx\
                                                     │
                                                     ▼
                                    tôi ghép vào repo bằng Tools/inbox.py
```

**Bốn nơi, một chiều dữ liệu:**

| Nơi | Chứa gì | Ai ghi | Đồng bộ bằng |
|---|---|---|---|
| **GitHub** | Lịch sử, nhánh, issue, PR | Arena (push + PR), bạn (merge nếu muốn) | `git push` / `git pull` |
| **Máy bạn (Unity)** | Bản chạy thật, Library, cấu hình Unity | Bạn | `git pull` (nhận), **không push** (ruleset D6 chặn) |
| **Sàn Arena** | Bản nháp tích hợp, chạy cổng script | Arena | `git push` nhánh arena, `gh pr` |
| **Máy agent** | Bản đọc repo + thư mục nộp bài | Agent | `git clone`/`git fetch` (đọc), copy tay qua `inbox` |

> **Repo đang PUBLIC** ⇒ agent `git clone` được không cần token ⇒ chúng luôn đọc đúng bản mới nhất,
> và **không cần bạn gửi file cho chúng**. Chiều ngược lại (agent → repo) là chỗ **duy nhất** phải
> đi bằng tay, qua thư mục `inbox`.

---

## 2. Hai nhánh, và lý do chỉ có hai

| Nhánh | Nghĩa | Ai đẩy vào | Unity mở nhánh nào |
|---|---|---|---|
| `main` | **Trạng thái đã kiểm chứng.** Chỉ nhận thay đổi đã qua cổng (script hoặc Unity) | Chỉ qua merge PR | ✅ mặc định |
| `arena/01a0bf86-the-spire` | **Nhánh tích hợp.** Mọi thứ Arena làm đều đẩy lên đây | Arena | Chỉ khi test một PR code |

**Vì sao không có nhánh riêng cho từng agent:** agent **không có quyền push** (D5), nên nhánh riêng
của chúng là vô nghĩa. Thay vào đó, mỗi ticket = một **thư mục nộp bài**, và Arena có trách nhiệm
ghép. Đổi lại, quy tắc "một agent / một file / một lượt" (`00-CHARTER.md` §3) là **bắt buộc** —
không có nó thì Arena phải tự giải quyết conflict, và mỗi conflict là một lượt agent làm lại.

```
main ──●────────────────────────●───────────────●────────►   (chỉ merge qua PR)
        \                      ▲               ▲
         \  PR #1 (tài liệu)   │ PR #2 (code)  │
          \                    │               │
arena ─────●───●───●─────●─────●───●───●───────●──────────►
           T-003 T-009 T-101         T-004 T-201
           (commit nhỏ, đẩy lên liên tục — PR cập nhật theo)
```

---

## 3. LUỒNG A — ticket tài liệu (không cần Unity) · ví dụ T-009

Đây là luồng sẽ chạy nhiều nhất trong M1 (thiết kế 8 khu).

| Bước | Ở đâu | Ai | Việc + lệnh |
|---|---|---|---|
| 1 | GitHub | Arena | Mở issue `#4 T-009` (đã có). Trạng thái ghi ở `Docs/team/02-BACKLOG.md` |
| 2 | Chat của bạn | **Bạn** | Dán prompt khởi động (`05-KICKOFF-PROMPTS.md`) + số ticket cho agent. Hết |
| 3 | Máy agent | Agent | `git clone https://github.com/DungRaKa22/THE-SPIRE-.git` (chỉ để đọc) |
| 4 | Máy agent | Agent | Làm việc: sửa file → ghi vào `T-009/files/<đường dẫn y như trong repo>`; chạy `python3 Tools/jumpcheck.py` tại máy nó |
| 5 | Máy agent | Agent | Viết `T-009/report.md`: **dòng đầu `baseline: <sha 7 ký tự>`** rồi 4 mục (Đã làm / Cổng đã chạy / Chưa kiểm chứng / Token) |
| 6 | Máy bạn | **Bạn** | Copy cả thư mục `T-009/` vào `C:\git\inbox\T-009\` |
| 7 | Sàn Arena | Arena | `python3 Tools/inbox.py T-009 --inbox ~/inbox` → xem diff (dry run) → thêm `--write` |
| 8 | Sàn Arena | Arena | `git add -A && git commit -m "T-009: …" && git push` → `gh pr create` |
| 9 | GitHub | Antigravity | Review đối kháng, comment thẳng vào PR (ticket `T-003R` kiểu này) |
| 10 | GitHub | Arena | Sửa theo review (commit tiếp lên cùng nhánh → PR tự cập nhật) → `gh pr merge` |
| 11 | Máy bạn | **Bạn** | `git checkout main && git pull` — xong. Tài liệu không cần mở Unity |

Bước 5–9 **không cần Unity**, không cần bạn làm gì ngoài copy thư mục.

---

## 4. LUỒNG B — ticket code (cần cổng Unity) · ví dụ T-201

Giống Luồng A, khác từ bước 9 trở đi — và đây là chỗ dễ làm sai nhất:

| Bước | Ở đâu | Ai | Việc + lệnh |
|---|---|---|---|
| 1–8 | giống trên | | Agent viết code → inbox → Arena ghép, push, mở PR |
| **9** | Sàn Arena | Arena | Chạy **cổng script**: `apicheck.py` (không bịa API) + `jumpcheck.py` |
| **10** | Sàn Arena | Arena | Báo cho bạn: "Lô cổng Unity gồm PR #N: T-201" + checklist gộp |
| **11** | Máy bạn | **Bạn** | **Đóng Unity** → `git fetch origin && git checkout -B gate origin/arena/01a0bf86-the-spire` → **mở lại Unity** |
| **12** | Máy bạn | **Bạn** | Chạy checklist: biên dịch 0 lỗi → Play 60 s → menu check → đọc `Docs/*Validation.txt` |
| **13** | Máy bạn | **Bạn** | Dán 4 dòng kết quả vào PR (biên dịch / Console khi Play / kết quả menu check / cảm giác 1 câu) |
| **14a** | GitHub | Arena | **PASS** → `gh pr merge` → bạn `git checkout main && git pull`, xoá nhánh tạm `gate` |
| **14b** | Sàn Arena | Arena | **FAIL** → Arena `git revert <sha>` commit gây lỗi trên nhánh arena (PR tự cập nhật, chỉ còn phần đạt). Ticket lỗi quay lại owner, để dành lô sau |

**Vì sao phải test trên nhánh `gate` chứ không phải `main`:** `main` là "đã kiểm chứng" — nếu bạn
merge trước rồi mới test, `main` có thể chứa code không biên dịch được, và lần mở Unity sau bạn sẽ
gặp lỗi cũ mà không biết từ đâu. Thứ tự đúng luôn là: **test trước, merge sau**.

> **Mẹo giảm số lần đổi nhánh:** vì Arena đẩy tất cả lên **một** nhánh `arena/…`, bạn chỉ cần một
> lần `git checkout -B gate origin/arena/01a0bf86-the-spire` cho cả lô, chạy hết checklist của mọi
> PR trong lô, rồi Arena merge từng PR.

---

## 5. Đồng bộ ở máy Unity — chi tiết cần nhớ

| Việc | Lệnh | Ghi chú |
|---|---|---|
| Nhận thay đổi đã merge | `git checkout main && git pull` | Làm trước khi mở Unity |
| Xem có gì mới | `git log --oneline -8` | |
| Test một PR code | `git fetch origin && git checkout -B gate origin/arena/01a0bf86-the-spire` | **Đóng Unity trước khi đổi nhánh** |
| Về lại bình thường | `git checkout main && git pull` | Rồi mở Unity |
| Xem mình đang ở nhánh nào | `git status -sb` | Phải là `main` khi không test |

**Luật an toàn trên máy Unity:**

1. **Không bao giờ sửa file trực tiếp trong repo Unity rồi commit.** Cổng luôn đi qua PR; sửa tay
   trên `main` sẽ bị ruleset chặn push và bạn mất công làm lại.
   - *Ngoại lệ duy nhất:* bạn muốn chỉnh nhanh một tham số để **thử cảm giác** — sửa trong
     Play Mode hoặc sửa xong `git checkout -- <file>` để bỏ. Muốn giữ thì mở ticket cho Arena.
2. **Đóng Unity trước khi đổi nhánh.** Unity tự nhập lại asset khi file đổi, nhưng đổi nhánh giữa
   lúc nó đang import dễ sinh trạng thái lạ trong `Library/`.
3. `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Build/` **không bao giờ commit** (đã có trong
   `.gitignore`). `Assets/_Recovery/` cũng sẽ được gỡ (ticket T-006).
4. Sau `git pull`, nếu Unity hỏi "upgrade project" thì bấm **No** — project version không đổi trong
   các ticket này.

---

## 6. Đồng bộ ở GitHub — chi tiết về issue, PR, merge

| Việc | Ai làm | Lệnh / đường dẫn |
|---|---|---|
| Tạo ticket | Arena | `gh issue create` (Arena tạo được, **không sửa/xoá/gắn nhãn được** — xem `01-PIPELINE.md` §2) |
| Trạng thái ticket | Arena | `Docs/team/02-BACKLOG.md` — bảng sống, vì Arena không đóng/comment issue được |
| Đóng ticket | **Bạn** | Bấm `Close issue` trên GitHub khi PR đã merge (hoặc nối lại GitHub cho Arena) |
| Mở PR | Arena | `gh pr create --base main --head arena/01a0bf86-the-spire` |
| Review | Antigravity | Comment trong PR (chỉ cần quyền đọc, repo public) |
| Merge | Arena | `gh pr merge` — **đã kiểm chứng: Arena có quyền merge** (thử trên PR draft trả về "still a draft", không phải 403) |
| Bảo vệ `main` | **Bạn** | Ruleset `main` (5 bước ở `01-PIPELINE.md` §2) |

**Một PR tại một thời điểm.** Cả dự án chỉ có một nhánh tích hợp. Muốn tách phần bị fail ra khỏi PR,
Arena dùng `git revert` (không tách nhánh mới, vì Arena chỉ được đẩy một nhánh).

---

## 7. Quy tắc `.meta` — nguồn rối loạn lớn nhất nếu không chốt

Unity cần mỗi asset có một file `.meta` chứa GUID. Nếu hai máy sinh `.meta` cho cùng một asset,
GUID khác nhau ⇒ Unity coi là hai asset ⇒ mất tham chiếu (prefab mất sprite, scene mất script).

| Loại file | Ai tạo `.meta` | Ghi chú |
|---|---|---|
| `.md`, `.txt` (tài liệu) | **Không ai** | Unity bỏ qua, không có meta |
| `.cs` mới | **Arena** khi ghép inbox | `Tools/inbox.py` tự sinh: `fileFormatVersion: 2` + `guid: <32 hex>` |
| `.cs` cũ bị sửa | Không đổi | Meta giữ nguyên |
| `.png`, `.prefab`, `.asset` mới | **Bạn** (mở Unity một lần) | Cần TextureImporter/importer settings của project; Arena không tự bịa |
| File do builder sinh trong scene | **Builder** (khi bạn chạy menu dựng) | Không commit tay |

**Luật vàng:** asset nhị phân mới ⇒ để Unity sinh meta ở máy bạn; file text (`.cs`, `.md`, `.py`)
⇒ Arena sinh meta và commit luôn. Nếu `git status` ở máy bạn hiện một loạt `.meta` mới sau `pull`,
nghĩa là Arena quên — mở issue, **đừng** commit chúng trên `main` (ruleset sẽ chặn).

---

## 8. Bảng lệnh nhanh theo vai

**Bạn (máy Unity):**
```powershell
cd C:\git\THE-SPIRE-
git status -sb                          # đang ở nhánh nào?
git checkout main && git pull           # nhận bản đã kiểm chứng
# test một PR code:
git fetch origin
git checkout -B gate origin/arena/01a0bf86-the-spire
# ... mở Unity, chạy checklist, dán kết quả vào PR ...
git checkout main
```
```powershell
# copy bài nộp của agent vào hộp thư:
Copy-Item -Recurse D:\Downloads\T-009 C:\git\inbox\
```

**Arena (sàn):**
```bash
python3 Tools/inbox.py T-009 --inbox ~/inbox            # xem diff, không sửa gì
python3 Tools/inbox.py T-009 --inbox ~/inbox --write    # ghép vào repo
python3 Tools/jumpcheck.py                              # cổng ngân sách nhảy
python3 Tools/apicheck.py                               # cổng API (khi đã có, T-004)
git add -A && git commit -m "T-009: …" && git push
gh pr create --base main --head arena/01a0bf86-the-spire
gh pr merge                                            # sau khi cổng pass
```

**Agent (máy nó, chỉ đọc + nộp thư mục):**
```bash
git clone https://github.com/DungRaKa22/THE-SPIRE-.git spire && cd spire
git log --oneline -1                     # ghi sha này vào report.md (baseline)
# ... làm việc, viết file vào T-0xx/files/ ...
python3 Tools/jumpcheck.py               # cổng phải xanh trước khi nộp
# KHÔNG commit, KHÔNG push — chỉ để lại thư mục T-0xx/ cho chủ dự án copy
```

---

## 9. Xử lý sự cố

| Hiện tượng | Nguyên nhân thường gặp | Cách xử lý |
|---|---|---|
| Arena báo `TỪ CHỐI: Assets/Scenes/…` | Agent nộp cả file scene | Đúng thiết kế. Scene do builder sinh (T-302/T-304); bảo agent chỉ nộp `.cs`/`.md` |
| `inbox: không tìm thấy thư mục ticket` | Sai đường dẫn hoặc thiếu `files/` | Dùng `--inbox <đường dẫn>`; thư mục phải có `files/` và `report.md` |
| Diff khổng lồ, hàng nghìn dòng "đổi" mà nội dung như nhau | Agent nộp file lưu bằng CRLF hoặc sai encoding | Bảo agent lưu UTF-8, LF; Arena có thể `--write` rồi `git diff -w` để kiểm |
| Agent sửa nhầm bản cũ (baseline xa) | Agent clone lâu, repo đã tiến | Arena ghép tay theo diff; lần sau nhắc agent `git pull` trước khi bắt đầu |
| Unity báo lỗi biên dịch sau `pull` | PR code lọt qua mà chưa qua cổng Unity | Đây là lỗi quy trình: báo Arena revert commit đó; kiểm lại xem PR có gắn `gate:unity` không |
| `git push` trên máy bạn bị từ chối | Ruleset D6 đang bật (đúng như thiết kế) | Không push từ máy Unity. Mọi thay đổi đi qua PR |
| Hai agent cùng sửa một file | Vi phạm luật sở hữu file | Arena chọn bản đúng, trả ticket còn lại; thêm dòng "xếp hàng" vào backlog |
| PR quá lớn, review mệt | Ticket gộp nhiều việc | Arena cắt nhỏ; mỗi PR ≤ 400 dòng, một ticket |
| `Tools/jumpcheck.py` báo `HARD=…` sau khi ghép | Dữ liệu mới vi phạm ngân sách nhảy | **Không merge.** Trả ticket cho owner kèm nguyên văn dòng `HARD` |

---

## 10. Một tuần thực tế trông như thế này

| Thời điểm | Việc | Ai |
|---|---|---|
| Thứ 2 | Arena mở PR #1 (tài liệu + Team OS), bạn bật ruleset, dán 3 prompt khởi động | Arena + bạn |
| Thứ 2–3 | Freebuff làm T-009, Antigravity làm T-003R + T-101, Codex làm T-004 (song song, khác file) | 3 agent |
| Thứ 3 | Bạn copy `T-009`, `T-004` vào `inbox`; Arena ghép, chạy cổng, push, cập nhật PR #1 | bạn + Arena |
| Thứ 4 | Antigravity review PR #1 (comment GitHub). Arena sửa theo review | Antigravity + Arena |
| Thứ 4 | Arena merge PR #1 (tài liệu ⇒ không cần Unity). Bạn `git checkout main && git pull` | Arena + bạn |
| Thứ 5–6 | T-101 xong → Arena mở PR #2; T-102 (bảng khu 4) bắt đầu ngay sau đó | Arena + Freebuff |
| Thứ 7 | **Cổng Unity theo lô** (nếu PR #2 có code; PR tài liệu thì không cần) | bạn |
| Chủ nhật | Arena cập nhật `02-BACKLOG.md` + `memory/02`, dọn `inbox` đã ghép | Arena |

---

## 11. Checklist dán tường

**Bạn, mỗi lần nhận bài nộp:**
```
[ ] Thư mục có files/ và report.md
[ ] Dòng đầu report.md có baseline: <sha>
[ ] report.md có mục "Cổng đã chạy" kèm output thật
[ ] Copy vào C:\git\inbox\T-0xx\
[ ] Báo Arena "có bài trong inbox T-0xx"
```

**Bạn, mỗi lần chạy cổng Unity (theo lô):**
```
[ ] Đóng Unity
[ ] git fetch origin && git checkout -B gate origin/arena/01a0bf86-the-spire
[ ] Mở Unity, đợi biên dịch, Console 0 lỗi
[ ] Play 60 giây mỗi scene bị ảnh hưởng
[ ] Chạy menu check của từng ticket trong lô
[ ] Dán 4 dòng kết quả vào PR
[ ] git checkout main && git pull   (sau khi Arena merge)
```

**Arena, mỗi lần ghép bài:**
```
[ ] python3 Tools/inbox.py T-0xx --inbox ~/inbox        (xem diff)
[ ] --write, kiểm git status, chạy jumpcheck/apicheck
[ ] commit "T-0xx: …" + push + cập nhật PR
[ ] Cập nhật Docs/team/02-BACKLOG.md (trạng thái ticket)
```
