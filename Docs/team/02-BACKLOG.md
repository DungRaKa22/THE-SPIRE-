# 02 — Backlog & lộ trình (bảng trạng thái sống)

> **Đây là nguồn chân lý về trạng thái.** Issue trên GitHub là *yêu cầu gốc, bất biến* — Arena
> không sửa/đóng/gắn nhãn được (xem `01-PIPELINE.md` §2), nên tầng/owner/trạng thái ghi ở đây.
> Chủ dự án đóng issue khi ticket xong.

Trạng thái: `[ ]` chưa làm · `[~]` đang làm · `[x]` xong (đã qua cổng) · `[!]` đang bị chặn.
Tầng: **A** = cần suy luận (Antigravity/Codex) · **B** = khối lượng có script kiểm (Freebuff).
Cổng: `jc` = `Tools/jumpcheck.py` · `ac` = `Tools/apicheck.py` · `u` = cổng Unity của chủ dự án.

## Quyết định của chủ dự án (20/09/2026)

| # | Quyết định | Chọn |
|---|---|---|
| Q1 | Kênh điều phối | **GitHub Issues + PR** |
| Q2 | Quyền push | **Chỉ Arena push** — 3 agent nộp bài qua `~/inbox/T-0xx/` |
| Q3 | Ưu tiên | **Chất lượng trước** — mọi PR code có review đối kháng của Antigravity |
| Q4 | Khởi động | **M0 + M1 song song**, nhưng chốt quy ước bảng trước khi ai điền số |

## Việc còn phải chốt (D1–D7)

| # | Câu hỏi | Đề xuất của Arena |
|---|---|---|
| D1 | Cột `p`: sửa số theo mức tối thiểu đầy đủ, hay đổi nghĩa cột? | Đã chốt trong T-003: đổi thành `p_min` đầy đủ + thêm `p_max` |
| D2 | Luật coin khi ~330 bệ: 1/5 hay 1/8? | Giữ 1/5 cho khu 1–3, thử 1/8 từ khu 4 (đo bằng số coin/khu) |
| D3 | Bật M2 (code mechanic) ngay? | Nên bật T-201 sớm để chứng minh bản sửa `Grounded` chạy thật |
| D4 | Nhịp cổng Unity | Cửa sổ cố định mỗi tuần + mỗi khi có ≥ 3 PR xếp hàng |
| D5 | Mỗi agent có PAT riêng? | Không — theo Q2 |
| D6 | Ruleset chặn push thẳng vào `main`? | Nên bật (chủ dự án làm, Arena không có quyền) |
| D7 | Dùng Issues làm board chính thức? | Đã chốt — kèm hạn chế quyền ở `01-PIPELINE.md` §2 |

## M0 — Team OS & cổng kiểm

| Ticket | Issue | Việc | Tầng | Chủ | Trạng thái |
|---|---|---|---|---|---|
| T-001 | — | Bộ luật nhóm | — | Arena | `[x]` PR #1 |
| T-002 | — | `Tools/jumpcheck.py` + báo cáo | — | Arena | `[x]` PR #1 |
| T-003 | [#2](https://github.com/DungRaKa22/THE-SPIRE-/issues/2) | **Quy ước bảng bệ v2** (`p_min`, `p_max`, luật liên kết căng) | A | Arena | `[x]` PR #1 |
| T-003R | [#3](https://github.com/DungRaKa22/THE-SPIRE-/issues/3) | Review đối kháng quy ước v2 + công cụ | A | Antigravity | `[ ]` |
| T-009 | [#4](https://github.com/DungRaKa22/THE-SPIRE-/issues/4) | Chuyển bảng khu 1–3 sang v2 + sửa 4 lỗi hình học | B | Freebuff | `[ ]` |
| T-004 | [#5](https://github.com/DungRaKa22/THE-SPIRE-/issues/5) | `Tools/apicheck.py` | B | Codex | `[ ]` |
| T-006 | [#8](https://github.com/DungRaKa22/THE-SPIRE-/issues/8) | Dọn repo (âm thanh trùng, `_Recovery`) | B | Freebuff | `[ ]` |
| T-005 | — | Issue template + nhãn + board | — | Arena | `[~]` (thiếu quyền gắn nhãn) |

## M1 — Hoàn tất thiết kế 8 khu

Mỗi khu 3 nhịp: **A1** khung mechanic + toán (Antigravity) → **B** bảng 40–45 bệ (Freebuff) →
**A2** soát số liệu + Arena chạy `jc`. Một agent / một file / một lượt — ticket cùng file xếp hàng.

| Ticket | Issue | Việc | Tầng | Chủ | Trạng thái |
|---|---|---|---|---|---|
| T-101 | [#6](https://github.com/DungRaKa22/THE-SPIRE-/issues/6) | Khu 4 Machine Age — khung + toán băng chuyền | A1 | Antigravity | `[ ]` ← **bắt đầu ngay** |
| T-102 | [#7](https://github.com/DungRaKa22/THE-SPIRE-/issues/7) | Khu 4 Machine Age — bảng bệ | B | Freebuff | `[!]` chờ T-101 + T-009 |
| T-103 | — | Khu 5 Digital Revolution — khung + bảng | A1/B | Antigravity → Freebuff | `[ ]` xếp sau T-101 |
| T-104 | — | Khu 6 Neon Megacity (cao trào, booster + drone) | A1/B | Antigravity → Freebuff | `[ ]` |
| T-105 | — | Khu 7 The Singularity (vùng trọng lực) | A1/B | Antigravity → Freebuff | `[ ]` |
| T-106 | — | Khu 8 The Sky + 2 ending | A/B | Antigravity → Freebuff | `[ ]` |
| T-107 | — | Bảng tra "cửa sổ hành động" hợp nhất khu 2–8 (§1.5b) | A | Antigravity | `[ ]` |
| T-108 | — | Gộp chỉ mục bệ toàn tháp (~330) + chốt luật coin (D2) | B | Freebuff | `[ ]` |

## M2 — Code mechanic (chờ D3)

| Ticket | Issue | Việc | Tầng | Chủ | Trạng thái |
|---|---|---|---|---|---|
| T-201 | [#9](https://github.com/DungRaKa22/THE-SPIRE-/issues/9) | `MovingPiston` + menu check | A | Codex | `[!]` chờ D3 + T-004 |
| T-202 | — | `ConveyorBelt` | A | Codex | `[ ]` |
| T-203 | — | `TogglePlatform` | B | Freebuff | `[ ]` |
| T-204 | — | `JumpBooster` + `DronePlatform` | B | Freebuff | `[ ]` |
| T-205 | — | `GravityZone` | A | Codex | `[ ]` |
| T-206 | — | `EndingChoice` + mảnh lore | B | Freebuff | `[ ]` |
| T-207 | — | `GameBalance` + `SpirePresentation` (bỏ tên Cyberpunk) | A | Codex | `[ ]` |

## M3–M5 (chi tiết khi tới nơi)

- **M3:** `SectorBlueprint` (T-301) → `TheSpireBuilder` (T-302) → `SpireChecks` (T-303) →
  dựng `TheSpire.unity` + `SectorStreamer` (T-304, Arena).
- **M4:** chạy `SpireChecks` toàn tháp, 60 FPS, lưu/tiếp tục slot `Spire.v1`, playtest đầu–cuối.
- **M5:** import art (PPU 100, pivot chân), prop 8 khu, parallax, ambience + NPC + lore, build cuối.

## Lượt giao việc hiện tại (Arena giao 20/09/2026)

| Agent | Ticket | Nộp gì | Chặn bởi |
|---|---|---|---|
| **Antigravity** | T-003R (review quy ước + công cụ) | `T-003R/report.md` | — |
| **Antigravity** | T-101 (khu 4 khung + toán) | `T-101/files/Docs/TheSpire-Level-Sector4.md` | sau T-003R |
| **Freebuff** | T-009 (chuyển bảng khu 1–3 sang v2) | 3 file Sector + `report.md` | — |
| **Codex** | T-004 (`Tools/apicheck.py`) | `T-004/files/Tools/apicheck.py` + `report.md` | — |
| **Chủ dự án** | D2 · D3 · D4 · D6 + kết nối lại GitHub (tuỳ chọn) | trả lời trong PR #1 | — |
