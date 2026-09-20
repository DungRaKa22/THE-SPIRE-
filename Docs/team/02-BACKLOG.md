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

## Quyết định còn lại (D1–D7)

| # | Câu hỏi | Trạng thái |
|---|---|---|
| D1 | Cách ghi cột `p` | ✅ đã chốt trong T-003: `p_min` đầy đủ + thêm `p_max` |
| D2 | Luật coin | ✅ **chốt 20/09: bỏ hoàn toàn cơ chế coin** (chỉ là demo, game không có điểm số). Đã dọn khỏi mọi tài liệu/prompt/công cụ; code cũ xoá ở **T-210** |
| D3 | Bật code M2 ngay? | ✅ **chốt 20/09: KHÔNG** — thiết kế đủ 8 khu rồi mới code (đúng `memory/00`, chủ dự án xác nhận lại) |
| D4 | Nhịp cổng Unity | ⏳ **chờ chủ dự án** — xem `04-UNITY-GATE.md`, 3 lựa chọn: mỗi PR code / theo lô ≥ 3 PR / cửa sổ cố định hằng tuần |
| D5 | Mỗi agent có PAT riêng? | ✅ không — chỉ Arena push |
| D6 | Ruleset chặn push thẳng vào `main`? | ⏳ **chờ chủ dự án** — bật/tắt, Arena không có quyền tạo |
| D7 | Issues làm board chính thức? | ✅ đã chốt (kèm hạn chế quyền ở `01-PIPELINE.md` §2) |

> **Hệ quả của D2 và D3 với lộ trình:** M1 (thiết kế 8 khu) là milestone duy nhất chạy được ngay;
> M2–M5 đóng cho tới khi đủ 8 doc khu. T-210 (xoá code coin) thuộc M2, không làm sớm hơn.
> Việc bỏ coin **không** làm mất số bệ: số vẫn là khoá thứ tự tuyến leo cho validator/builder.

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
| T-108 | — | Gộp chỉ mục bệ toàn tháp (~330) — chỉ mục tuyến leo, không coin | B | Freebuff | `[ ]` |

## M2 — Code mechanic (chờ D3)

| Ticket | Issue | Việc | Tầng | Chủ | Trạng thái |
|---|---|---|---|---|---|
| T-201 | [#9](https://github.com/DungRaKa22/THE-SPIRE-/issues/9) | `MovingPiston` + menu check | A | Codex | `[!]` chờ D3 + T-004 |
| T-202 | — | `ConveyorBelt` | A | Codex | `[ ]` |
| T-203 | — | `TogglePlatform` | B | Freebuff | `[ ]` |
| T-204 | — | `JumpBooster` + `DronePlatform` | B | Freebuff | `[ ]` |
| T-205 | — | `GravityZone` | A | Codex | `[ ]` |
| T-206 | — | `EndingChoice` + mảnh lore | B | Freebuff | `[ ]` |
| T-207 | — | `GameBalance` + `SpirePresentation` (bỏ tên Cyberpunk, bỏ SCORE khỏi HUD) | A | Codex | `[ ]` |
| T-210 | — | **Xoá code coin**: `PlatformCoins.cs`, `PlatformCoinPickup.cs`, phần coin trong `GameSession` (Score, CoinSeed, Coins, SaveCoinProgress) và `NeonAscentChecks` | B | Freebuff | `[!]` chờ M2 |

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
| **Chủ dự án** | trả lời **D4** (nhịp cổng Unity) và **D6** (ruleset bảo vệ `main`) + kết nối lại GitHub (tuỳ chọn) | trả lời trong PR #1 | — |
