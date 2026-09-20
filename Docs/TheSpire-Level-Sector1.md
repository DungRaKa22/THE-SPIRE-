# KHU 1 — FORGOTTEN KINGDOM (0 – 63 m)

> Thiết kế bố cục tầng chi tiết. Mọi liên kết bệ đều được kiểm tra bằng công thức quỹ đạo thật,
> không ước lượng bằng mắt. Đây là khu dạy học: **không có mechanic động**, độ khó đến từ bố cục.
>
> Khu 1 dùng các bệ đánh số **01–40**. Các khối không đứng được (tường, sàn, trần, trang trí)
> **không được đánh số** — quy ước này bắt buộc để `PlatformCoins` không nhận nhầm vào nhóm coin.

---

## 1. Ngân sách nhảy — bảng tra để thiết kế

Từ hằng số đã khóa trong `Docs/TheSpire-Remake-Plan.md` §1.2
(`g_eff = 29.43`, `v(p) = 4 + 9p`, ngang `6 × Lerp(0.4, 1, p)`):

| Đại lượng | Công thức | Giá trị |
|---|---|---:|
| Chiều cao đỉnh **H** | `maxJump² / (2·g_eff)` | **2.87 m** |
| Tầm xa ngang **D** (về cùng độ cao) | `horiz · 2·v/g` | **5.30 m** |
| **Trần thiết kế Δy** | `0.92 · H` | **2.64 m** |

Với cú nhảy lên `Δy`, tầm xa khả dụng dùng **nhánh rơi xuống** (đúng cách Jump King vận hành —
người chơi bay qua đỉnh rồi hạ xuống bệ đích):

```
t₂(Δy, p) = ( v + √(v² − 2·g_eff·Δy) ) / g_eff
D(Δy, p)  = horiz(p) · t₂
Hợp lệ khi:  Δy ≤ 0.92 · H(p)   VÀ   |Δx| ≤ 0.85 · D(Δy, p)
```

**Bảng tra nhanh** (Δx tối đa = `0.85 · D`):

| Δy (m) | p tối thiểu | @p=0.75 | @p=0.85 | @p=0.90 |
|---:|---:|---:|---:|---:|
| 1.1 | 0.49 | 2.63 | 3.17 | 3.44 |
| 1.3 | 0.57 | 2.50 | 3.05 | — |
| 1.5 | 0.64 | 2.35 | 2.92 | 3.21 |
| 1.7 | 0.71 | 2.16 | 2.78 | 3.08 |
| 1.9 | 0.78 | — | 2.61 | 2.93 |

> Nhắc lại hệ quả then chốt: lực nhảy là **bậc hai** theo thời gian giữ. Nửa tích lực chỉ được
> **1/4 chiều cao**. Vì vậy khoảng cách bệ **không được** thiết kế theo tỷ lệ tuyến tính.

---

## 2. Từ điển cú nhảy khu 1

| Loại | Δy | Δx | p cần | Dùng ở |
|---|---|---|---|---|
| **J1 thấp** | 1.1–1.4 | 1.3–1.8 | 0.49–0.61 | Tầng 1–2 |
| **J2 vừa** | 1.5–1.7 | 1.6–2.4 | 0.64–0.78 | Toàn khu |
| **J3 cao** | 1.8–1.9 | 1.3–2.0 | 0.75–0.78 | Chỉ tầng 7 |

Khu 1 **không dùng** J4 (Δy ≥ 2.3) — để dành từ khu 4 trở lên.

---

## 3. Bố cục 40 bệ

Sàn đấu **12 m** (x ∈ [−6, +6]); tường trong tại ±6.0, dày 0.5. Lưới tầng tham chiếu **9 m**.
`Δx` đo từ **mép gần của bệ nguồn → tâm bệ đích**. `p` là mức tích lực tối thiểu cần.

### Tầng 1 — Chợ cũ (0 – 8.10 m)

| # | Tên | x | y đỉnh | rộng | Δy | Δx | p |
|---|---|---:|---:|---:|---:|---:|---:|
| 01 | Ground | 0 | 0.00 | 12.0 | — | — | — |
| 02 | Broken Roof | −3.6 | 1.10 | 3.0 | 1.10 | 0.0 | 0.49 |
| 03 | Barrel Row | −0.4 | 2.30 | 2.8 | 1.20 | 1.70 | 0.53 |
| 04 | Cart Shed | 2.8 | 3.60 | 2.6 | 1.30 | 1.80 | 0.57 |
| 05 | Old Well | 0.2 | 5.00 | 2.4 | 1.40 | 1.30 | 0.61 |
| 06 | Roof Ridge | −2.8 | 6.50 | 3.4 | 1.50 | 1.80 | 0.64 |
| 07 | Belfry Stair | 0.6 | 8.10 | 2.6 | 1.60 | 1.70 | 0.68 |

Spawn tại **(−4.0, 0.65)** trên `01 Ground`. Cả tầng là J1: học nhảy ngắn, nhảy xa, đáp mép.
Không có rủi ro rơi vì sàn đất nằm dưới toàn bộ.

**Trang trí (không đánh số, không va chạm):** dây phơi, thùng gỗ vỡ, giếng cổ, đèn tường.

### Tầng 2 — Tháp chuông (8.10 – 17.40 m)

| # | Tên | x | y đỉnh | rộng | Δy | Δx | p |
|---|---|---:|---:|---:|---:|---:|---:|
| 08 | Bell Tower Ledge | −2.2 | 9.70 | 2.2 | 1.60 | 1.50 | 0.68 |
| 09 | Rope Bridge | 1.2 | 11.20 | 3.0 | 1.50 | 2.30 | 0.74 |
| 10 | Stone Cornice | −0.6 | 12.80 | 2.4 | 1.60 | 0.30 | 0.68 |
| 11 | Choir Loft | 2.4 | 14.30 | 2.4 | 1.50 | 1.50 | 0.65 |
| 12 | Knight Statue | −1.6 | 15.90 | 1.9 | 1.60 | 2.40 | 0.78 |
| 13 | Nave Console | 1.8 | 17.40 | 3.2 | 1.50 | 2.45 | 0.77 |

#### Bài học trần thấp — `Machicolation`

Thêm một khối **có va chạm**, tên `Machicolation`, **không đánh số**:

- x ∈ [−2.2, 0.7]; mặt dưới **y = 15.10**; dày 0.5 m

Đứng trên `10 Stone Cornice` (đỉnh 12.80) → đầu nhân vật 13.70 → còn **1.4 m khoảng thở** ✓

Cửa sổ kỹ năng của cú **10 → 11**:

| Điều kiện | Bất phương trình | Kết quả |
|---|---|---|
| Lên tới được 11 (đỉnh 14.30) | `Δy 1.50 ≤ 0.92·H(p)` | `p ≥ 0.644` |
| Không đập trần 15.10 | `h(p) ≤ 2.30` | `p ≤ 0.849` |
| **Cửa sổ hợp lệ** | | **p ∈ [0.64, 0.85]** |

Tích quá tay là đập trần và mất cú nhảy — đúng bài học Jump King, dạy bằng hình phạt tự nhiên
chứ không bằng hướng dẫn chữ.

### Tầng 3 — Mái thành (17.40 – 26.80 m)

| # | Tên | x | y đỉnh | rộng | Δy | Δx | p |
|---|---|---:|---:|---:|---:|---:|---:|
| 14 | Cloister Roof | −0.8 | 19.00 | 2.6 | 1.60 | 1.00 | 0.68 |
| 15 | Rope Pulley | 2.4 | 20.60 | 2.2 | 1.60 | 1.90 | 0.70 |
| 16 | Mill Wall | −1.4 | 22.10 | 2.4 | 1.50 | 2.70 | 0.81 |
| 17 | Watch Post | 1.6 | 23.70 | 2.0 | 1.60 | 1.80 | 0.70 |
| 18 | Gate Arch | −2.0 | 25.20 | 2.8 | 1.50 | 2.60 | 0.80 |
| 19 | **Rest Roof — Old Belfry** | 1.2 | 26.80 | **3.6** | 1.60 | 1.80 | 0.70 |

Hai cú căng nhất khu (**16** và **18**, p ≈ 0.80–0.81) rồi thưởng bằng mái nghỉ rộng 3.6 m.

### Tầng 4 — Giàn giáo (26.80 – 36.30 m)

| # | Tên | x | y đỉnh | rộng | Δy | Δx | p |
|---|---|---:|---:|---:|---:|---:|---:|
| 20 | Scaffold Plank | −1.0 | 28.40 | 2.2 | 1.60 | 0.40 | 0.68 |
| 21 | Hoist Beam | 1.8 | 30.00 | 1.9 | 1.60 | 2.10 | 0.75 |
| 22 | Cornice Ledge | −1.4 | 31.60 | 2.4 | 1.60 | 2.25 | 0.75 |
| 23 | Bell Rope Post | 0.8 | 33.20 | 2.0 | 1.60 | 1.00 | 0.68 |
| 24 | Gargoyle Perch | −2.6 | 34.80 | 1.7 | 1.60 | 2.40 | 0.78 |
| 25 | Nave Roof | 0.6 | 36.30 | 3.4 | 1.50 | 2.35 | 0.75 |

Bệ hẹp 1.7–2.0 m xuất hiện đều — dạy đáp mép chính xác.

### Tầng 5 — Hầm mộ (36.30 – 45.70 m)

| # | Tên | x | y đỉnh | rộng | Δy | Δx | p |
|---|---|---:|---:|---:|---:|---:|---:|
| 26 | Crypt Vault | −0.4 | 37.90 | 2.8 | 1.60 | 0.70 | 0.68 |
| 27 | Ossuary Shelf | 2.6 | 39.50 | 2.2 | 1.60 | 1.60 | 0.68 |
| 28 | Stone Coffin | −1.2 | 41.10 | 2.6 | 1.60 | 2.70 | 0.81 |
| 29 | Iron Gate Top | 1.4 | 42.60 | 1.9 | 1.50 | 1.30 | 0.65 |
| 30 | Reliquary | −1.6 | 44.20 | 2.4 | 1.60 | 2.05 | 0.72 |
| 31 | **Rest Roof — Bell Chamber** | 1.0 | 45.70 | **3.6** | 1.50 | 1.40 | 0.65 |

### Tầng 6 — Tháp chuông cao (45.70 – 53.80 m)

| # | Tên | x | y đỉnh | rộng | Δy | Δx | p |
|---|---|---:|---:|---:|---:|---:|---:|
| 32 | Bell Frame | −1.4 | 47.30 | 2.2 | 1.60 | 0.60 | 0.68 |
| 33 | Clapper Walk | 1.8 | 49.00 | 2.0 | 1.70 | 2.10 | 0.75 |
| 34 | Sound Window | −1.6 | 50.60 | 2.0 | 1.60 | 2.40 | 0.78 |
| 35 | Gargoyle Spout | 0.4 | 52.20 | 2.0 | 1.60 | 1.00 | 0.68 |
| 36 | Spire Foot | 2.6 | 53.80 | 2.4 | 1.60 | 1.20 | 0.68 |

### Tầng 7 — Chóp tháp (53.80 – 61.10 m)

| # | Tên | x | y đỉnh | rộng | Δy | Δx | p |
|---|---|---:|---:|---:|---:|---:|---:|
| 37 | Spire Step 1 | −0.6 | 55.60 | 2.2 | 1.80 | 2.00 | 0.75 |
| 38 | Spire Step 2 | 1.8 | 57.40 | 1.9 | 1.80 | 1.30 | 0.75 |
| 39 | Spire Step 3 | −1.0 | 59.20 | 1.8 | 1.80 | 1.85 | 0.75 |
| 40 | Summit Balcony | 1.2 | 61.10 | 3.6 | 1.90 | 1.30 | 0.78 |

Lần đầu khu này dùng J3. Cú cuối **rộng rãi có chủ đích** — đáp xuống sân thượng phải là
phần thưởng, không phải cú khó nhất.

**Cửa chuyển khu** tại **y = 63.0**: vòm đá bị ống đồng xuyên qua — chuyển sang Age of Steam.

---

## 4. Cấu trúc rơi

| Vị trí | Rơi xuống | Độ sâu | Loại |
|---|---|---|---|
| Mép trái 40 | 39 (59.20) | 1.9 m | Bệ bắt rơi |
| Mép phải 40 | 36 (53.80) | 7.3 m | Ngắn |
| Mép trái 19 | 18 (25.20) | 1.6 m | Bệ bắt rơi |
| Trượt 16 | 15 (20.60) | 1.5 m | Ngắn |
| Mép phải 31 | 29 (42.60) | 3.1 m | Ngắn |
| Trượt liên hoàn tầng 5–6 | 31 (45.70) | 3–10 m | Trung bình |
| Sai từ tầng 7 xuống thẳng | 31 → 19 | 34.3 m | **Dài — duy nhất 1 lần, cuối khu** |

Nguyên tắc: **mỗi tầng có ít nhất một bệ bắt rơi**, và chỉ duy nhất cú rơi dài ở tầng 7 mới thực
sự đưa về giữa khu. Khớp yêu cầu "không phải mọi sai sót đều đưa về đáy".

---

## 5. Tích hợp hệ thống hiện có

| Hạng mục | Giá trị |
|---|---|
| Bệ đánh số | 01–40 → `40 / 5 = 8` nhóm coin → **8 coin = 80 điểm** |
| Quy ước tên | `"NN Tên"` — `PlatformCoins` đọc `name.Split(' ')[0]` |
| Không đánh số | `Ground`, `Left wall`, `Right wall`, `Machicolation`, mọi trang trí |
| `summitHeight` khu 1 | **61.10 m** (HUD báo vượt khu) |
| Ranh giới khu | **63.0 m** |
| Số liên kết có `p > 0.80` | **2** (16 và 28) |

### Ghi chú sửa kế hoạch tổng

Bảng §1.4 của kế hoạch ghi mật độ bệ nghỉ khu 1 là **1/5**. Thiết kế thực tế cho ra:
**2 mái nghỉ (3.6 m) trên 40 bệ ≈ 1/20**, cộng thêm 3 bệ mở rộng 3.2–3.4 m.

Đề xuất sửa cột "Nghỉ" trong kế hoạch từ **tỷ lệ** thành **số tuyệt đối**:

| Khu | Mái nghỉ (≥3.4 m) | Bệ mở rộng (2.8–3.4 m) |
|---|---:|---:|
| 1 Forgotten Kingdom | 2 | 3 |

Lý do: một tỷ lệ 1/5 sẽ cho 8 mái nghỉ trong khu 1 — quá nhiều, làm mất sức nặng của khu.
Con số thiết kế thật cần dùng là số tuyệt đối.

---

## 6. Tiêu chí validator

`SpireChecks` phải xác nhận được cho khu này:

- [ ] Mọi liên kết 01→40 tới được bằng **mô phỏng vật lý thật** (không chỉ công thức).
- [ ] Cột `p` ở trên khớp với `p` mà validator tìm ra (sai số ≤ 0.05).
- [ ] `Δy` mọi liên kết ≤ **2.64 m**.
- [ ] Cú **10 → 11** thất bại khi `p > 0.85` (đập `Machicolation`) — kiểm tra âm để chắc
      bài học trần thấp thực sự hoạt động.
- [ ] Tổng 40 bệ đánh số, 8 coin, `summitHeight = 61.10`.
- [ ] Mọi `SpriteRenderer` trong khu có sprite hợp lệ.
