# KHU 2 — AGE OF STEAM (63 – 131 m)

> Bố cục tầng chi tiết cho khu có **piston di chuyển**. Khác mọi khu tĩnh, ở đây bài toán
> không còn là quỹ đạo parabol thuần mà là **quỹ đạo + thời điểm**: cùng một cú nhảy có thể
> tới đích hoặc trượt hoàn toàn tuỳ lúc thả nút.
>
> Khu 2 dùng các bệ đánh số **41–83** (tiếp số khu 1 — xem §8 để biết vì sao không đánh lại từ 01).

---

## 1. Mục tiêu thiết kế

| Giai đoạn | Tầng | Dạy gì |
|---|---|---|
| Giới thiệu | 8 | Piston là gì, leo lên nó, nhịp chậm |
| Luyện | 9 | Chuỗi piston cùng pha, "thang máy" |
| Kết hợp | 10 | Ống hơi đường tắt + piston |
| Cao trào | 11 | Buồng máy thẳng đứng, piston nhanh |
| Đổi nhịp | 12 | Đối pha — phá thói quen |
| Căng | 13 | Piston nhanh liên hoàn, bệ hẹp |
| Nghỉ | 14 | Mái nghỉ + chuyển khu |

Trục độ khó: **Δy 0.4–1.9 m** (rộng hơn khu 1 vì piston cho không chiều cao), nhưng
**cửa sổ thời gian** siết dần từ 1.20 s → 0.85 s.

---

## 2. Ba profile piston

Piston dùng biên dạng **hình thang có làm mềm hai đầu** (smoothstep) — vận tốc bằng 0 ở hai
điểm dừng, nên cú đáp không bao giờ bị "hất" bởi bệ đang tăng tốc.

```
y(t) = lo + A · s(t)
```

| Pha | Khoảng thời gian | `s` |
|---|---|---|
| Đi lên | `[0, rise)` | `0 → 1` (smoothstep) |
| **Dừng đỉnh** | `[rise, rise+top)` | `1` |
| Đi xuống | `[rise+top, rise+top+desc)` | `1 → 0` (smoothstep) |
| **Dừng đáy** | `[rise+top+desc, T)` | `0` |

| Profile | Biên độ A | rise | **top** | desc | **bottom** | Chu kỳ T | Dùng ở |
|---|---:|---:|---:|---:|---:|---:|---|
| **P-A** (dạy) | 2.2 m | 1.30 | **1.20** | 1.30 | **1.20** | 5.00 s | Tầng 8 |
| **P-B** (chuẩn) | 2.2 m | 1.10 | **1.00** | 1.10 | **1.20** | 4.40 s | Tầng 9–12, 14 |
| **P-C** (nhanh) | 1.8 m | 0.85 | **0.85** | 0.85 | **0.85** | 3.40 s | Tầng 11, 13 |

> Chu kỳ lấy số tròn để người chơi **đếm nhịp được**. P-A 5.0 s là nhịp thở; P-C 3.4 s đã là nhịp gấp.

---

## 3. Toán thời gian — cửa sổ đáp

Đây là phần quyết định khu này có công bằng hay không.

### 3.1 Leo lên piston (mount): bệ tĩnh → đáy piston

Người chơi thả nút tại `t_r`. Quỹ đạo chạm mức đáy của piston sau `Δt = t₂(Δy, p)`.

Điều kiện đáp hợp lệ:

```
t_r + Δt ∈ [ t_bot + guard ,  t_bot + bottom − guard ]
guard = 0.10 s   (biên cho việc phát hiện tiếp đất thực tế)
```

Vì `Δt` phụ thuộc mức tích lực người chơi chọn, khoảng `Δt` khả dụng làm hẹp cửa sổ:

| Δy | `t₂` @p=0.4 | `t₂` @p=0.8 | Độ trải |
|---:|---:|---:|---:|
| 0.6 m | 0.30 s | 0.55 s | 0.25 s |
| 0.9 m | 0.33 s | 0.67 s | 0.34 s |
| 1.2 m | 0.37 s | 0.74 s | 0.37 s |

**Cửa sổ thả nút thực tế:**

| Profile | `bottom` | Trừ guard | Trừ độ trải | **Cửa sổ** |
|---|---:|---:|---:|---:|
| P-A | 1.20 | −0.20 | −0.34 | **0.66 s** |
| P-B | 1.20 | −0.20 | −0.34 | **0.66 s** |
| P-C | 0.85 | −0.20 | −0.34 | **0.31 s** |

Kèm thời gian tích lực tối đa 0.85 s, người chơi phải **bắt đầu giữ nút sớm hơn** thời điểm
đáp khoảng 1.0–1.3 s. Piston phải **nhìn thấy được** từ chỗ đứng — đây là ràng buộc camera,
không phải ràng buộc vật lý.

### 3.2 Nhảy ngang giữa hai piston (cross): đỉnh → đỉnh

Điều kiện: hai piston **cùng pha**, người chơi thả trong lúc `top` và đáp trong lúc `top` của piston kia.

```
Δt_cross + guard ≤ top
```

| Δy | Δx | p | `Δt_cross` | `top` cần | P-B (1.00) | P-C (0.85) |
|---:|---:|---:|---:|---:|---|---|
| 1.50 | 2.20 | 0.75 | 0.54 s | ≥ 0.79 s | ✓ biên 0.46 | ✓ biên 0.31 |
| 1.60 | 1.80 | 0.70 | 0.52 s | ≥ 0.77 s | ✓ | ✓ |

**Quy tắc chốt:** `top ≥ Δt_cross + 0.25 s`. P-C là mức căng nhất còn công bằng.

### 3.3 Vì sao KHÔNG dùng việc "đứng chờ trên piston"

Người chơi có thể leo lên piston rồi đứng đó chờ. Điều này phải **được phép** (nó là kỹ năng
hợp lệ) nhưng **không được là bắt buộc**, vì nó biến thử thách thời gian thành thử thách kiên nhẫn.
Vì vậy mọi chuỗi piston đều có **bệ tĩnh nằm trong tầm một cú nhảy từ vị trí đáy** — lối thoát.

---

## 4. Quy tắc pha (phase)

Mọi piston chia sẻ **một đồng hồ chung** `t₀ = thời điểm bắt đầu lượt`. Đồng hồ dùng `Time.time`,
nên tự đóng băng khi `Time.timeScale = 0` (tạm dừng) — không cần xử lý riêng.

| # | Quy tắc | Lý do |
|---|---|---|
| 1 | Piston trong cùng chuỗi nhảy ngang **phải cùng pha** | Nếu không, cú đỉnh→đỉnh là bất khả thi |
| 2 | Đối pha (Δφ = 0.5) **chỉ** dùng khi có bệ tĩnh xen giữa | Người chơi quan sát lại từ đầu |
| 3 | Tối đa **3 piston liên tiếp** trước một bệ tĩnh | Giới hạn tải nhận thức |
| 4 | Đổi pha **chỉ** xảy ra ở ranh giới tầng | Tránh "bẫy" giữa chuỗi đang chạy |
| 5 | `StartNewRun` và `ContinueRun` đều đặt lại `t₀` | Nhịp quan hệ không đổi giữa các lượt |

Ranh giới pha trong khu 2:

| Tầng | Pha | Ghi chú |
|---|---|---|
| 8–11 | `φ = 0` | Toàn bộ cùng pha — dạy nhịp |
| 12 | `φ = 0.5` cho 65, 66; `φ = 0` cho 68 | **Phá thói quen** — người chơi phải đọc lại |
| 13 | `φ = 0` | Về nhịp thống nhất, căng bằng chu kỳ ngắn |
| 14 | `φ = 0` | Một piston cuối, dễ |

---

## 5. ✅ Rủi ro #1 (ĐÃ SỬA) — `Grounded` sẽ hỏng trên piston đi lên

Đây là lỗi **sẽ** xảy ra và làm hỏng toàn bộ khu 2 nếu không xử lý trước. **Đã sửa ở tầng code** —
`Assets/Scripts/MovingSurface.cs` + `DummyController.CheckGrounded()`.

`DummyController.FixedUpdate` xác định đứng vững bằng:

```csharp
if (Time.time >= ignoreGroundUntil && body.linearVelocity.y <= 0.1f)
    ... cast xuống ...
```

Piston P-B đi lên đạt vận tốc đỉnh ≈ **3.0 m/s** (`1.5 × A/rise`). Khi đẩy nhân vật lên,
`body.linearVelocity.y` trở thành **+3.0** → điều kiện `<= 0.1f` sai → `Grounded = false`
→ `CancelCharge()` chạy mỗi frame.

**Hệ quả:** người chơi **không thể tích lực khi đang đứng trên piston đi lên**. Cả khu 2 chết.

### Đã sửa như sau

1. **`IMovingSurface`** (file mới `Assets/Scripts/MovingSurface.cs`) — piston / băng chuyền / drone
   báo `SurfaceVelocity`, vận tốc mặt bề tại thời điểm hiện tại.
2. **`DummyController.CurrentSurface`** — nền tảng đang đứng, đọc từ kết quả ground cast của
   **chính frame đó** (không dùng hit của frame trước, tránh lệch pha một step vật lý).
3. Điều kiện đứng vững là **vận tốc tương đối**:
   `body.linearVelocity.y − maxUpward ≤ 0.1f`, với `maxUpward` = vận tốc lên nhanh nhất
   trong các bề mặt dưới chân. Trên sàn tĩnh `maxUpward = 0` → hành vi giữ nguyên như cũ.
4. `SetRunState`, `ApplyImpulse`, cú nhảy và `CheckGrounded` đầu mỗi frame đều quản lý/
   xoá `CurrentSurface` đúng chỗ.

### Yêu cầu bắt buộc khi dựng piston

Root piston phải mang **Rigidbody2D kinematic** và di chuyển bằng **`MovePosition`**. Collider
tĩnh chỉ teleport bằng `transform` sẽ không hiện ra trong ground cast của nhân vật — toàn bộ
cơ chế này vô hiệu mà không có lỗi nào báo ra.

### Va trần (rủi ro #3)

Chỉ vận tốc của cú nhảy bị triệt tiêu khi đập trần (hành vi gốc giữ nguyên). Trường hợp piston
đẩy nhân vật vào trần được loại bằng quy tắc khoảng trống **3.2 m** (mục dưới) — không xử lý
bằng vật lý, không thêm code riêng.

### Rủi ro #2 — Đi xuống thì mất tiếp xúc

Piston đi xuống nhanh hơn nhân vật rơi trong khoảnh khắc đầu (gia tốc rơi 29.43 m/s² so với
piston ~6 m/s² khởi đầu từ 0), nên nhân vật sẽ **nảy lên khỏi bệ** khi piston bắt đầu hạ.

**Không sửa bằng vật lý — sửa bằng thiết kế:** quy tắc cứng là **chỉ đi lên, không đi xuống**.
Mọi liên kết trong khu 2 đều có người chơi nhảy khỏi piston **trong lúc dừng đỉnh**.
Không có cú nhảy nào yêu cầu đứng trên piston đang hạ. Ghi rõ trong validator.

### Rủi ro #3 — Piston đẩy nhân vật vào trần

Mọi piston phải có khoảng trống ≥ **3.2 m** phía trên đỉnh (`A + H + biên`) để cú nhảy đầy
lực từ mặt piston không đập vào khối nào. Kiểm tra bằng validator.

---

## 6. Bố cục 43 bệ

Sàn đấu **12 m** (x ∈ [−6, +6]). `Δy` cho piston = từ đỉnh bệ nguồn tới **mức đáy** piston.
`Δx` đo từ mép gần bệ nguồn → tâm bệ đích.

### Tầng 8 — Cửa vào lò hơi (63.0 – 71.6 m) · piston **P-A**

| # | Tên | Loại | x | rộng | y_lo | y_hi | Δy | Δx | p |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| 41 | Steam Threshold | tĩnh | 0.0 | 2.8 | — | 63.00 | 1.90 | 1.20 | 0.78 |
| 42 | Copper Pipe | tĩnh | 2.6 | 2.4 | — | 64.30 | 1.30 | 1.20 | 0.57 |
| 43 | Boiler Rim | tĩnh | −0.6 | 2.8 | — | 65.60 | 1.30 | 2.00 | 0.57 |
| 44 | Catwalk | tĩnh | 1.6 | 2.4 | — | 66.90 | 1.30 | 0.80 | 0.57 |
| 45 | **Piston A1** | P-A | −1.8 | 2.6 | 67.30 | 69.50 | 0.40 | 2.20 | 0.59 |
| 46 | **Piston A2** | P-A | 1.8 | 2.6 | 68.50 | 70.70 | 1.20 | 2.30 | 0.53 |
| 47 | Boiler Top | tĩnh | −1.2 | 3.2 | — | 71.60 | 0.90 | 1.70 | 0.40 |

`41` là bệ đáp từ khu 1 (nối tiếp `40 Summit Balcony`). Piston A1 là **cú mount đầu tiên**:
`Δy` chỉ 0.40 m nên cửa sổ thời gian rộng nhất toàn game (0.66 s), và bệ rộng 2.6 m.
Người chơi học "leo lên rồi đi theo nó lên" mà gần như không thể trượt.

`46` là **cú cross đầu tiên**: đỉnh 45 (69.50) → đỉnh 46 (70.70), `Δy` 1.20, cùng pha.
Cửa sổ 0.66 s ✓

### Tầng 9 — Thang máy piston (71.6 – 81.5 m) · **P-B, cùng pha**

| # | Tên | Loại | x | rộng | y_lo | y_hi | Δy | Δx | p |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| 48 | Piston B1 | P-B | 1.4 | 2.4 | 72.60 | 74.80 | 1.00 | 1.30 | 0.45 |
| 49 | Piston B2 | P-B | −1.6 | 2.4 | 74.10 | 76.30 | 1.50 | 1.80 | 0.65 |
| 50 | Piston B3 | P-B | 1.4 | 2.4 | 75.70 | 77.90 | 1.60 | 1.80 | 0.70 |
| 51 | Riser Cradle | tĩnh | −0.8 | 2.2 | — | 78.70 | 0.80 | 1.00 | 0.35 |
| 52 | Piston B4 | P-B φ0.5 | 1.6 | 2.2 | 79.30 | 81.50 | 0.60 | 1.30 | 0.25 |

Ba piston liên tiếp **cùng pha** = "thang máy": leo lên, chờ đỉnh, nhảy ngang, lặp lại.
Chu kỳ 4.4 s ngắn hơn P-A nên nhịp nhanh hơn rõ rệt.

`51` là **bệ tĩnh thoát hiểm** — thoả quy tắc §3.3. `52` là piston đối pha đầu tiên nhưng có
bệ tĩnh 51 ngay trước, nên vẫn đọc được (đúng quy tắc pha #2).

### Tầng 10 — Ống hơi (81.5 – 89.2 m) · **P-B**

| # | Tên | Loại | x | rộng | y_lo | y_hi | Δy | Δx | p |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| 53 | Vent Catwalk | tĩnh | −1.2 | 3.4 | — | 82.40 | 0.90 | 1.70 | 0.40 |
| 54 | **Vent Updraft** | đường tắt | 1.6 | — | 82.40 | 86.00 | — | — | — |
| 55 | Pipe Bridge | tĩnh | 2.0 | 2.8 | — | 83.70 | 1.30 | 1.90 | 0.57 |
| 56 | Piston B5 | P-B | −1.4 | 2.4 | 84.60 | 86.80 | 0.90 | 2.00 | 0.40 |
| 57 | Piston B6 | P-B | 1.8 | 2.4 | 86.10 | 88.30 | 1.50 | 2.00 | 0.65 |
| 58 | **Rest Roof — Engine Deck** | tĩnh | −1.0 | 3.6 | — | 89.20 | 0.90 | 1.60 | 0.40 |

#### Đường tắt `54 Vent Updraft`

- Vùng kích hoạt: x ∈ [1.1, 2.1], y ∈ [82.40, 86.00]
- Kích hoạt khi nhân vật **đi vào từ dưới** và có vận tốc lên
- Xung lực **cố định** `+9.0 m/s` (tương đương `p ≈ 0.56`) — **không ngẫu nhiên, không biến thiên**
- Đồng hồ áp suất + tiếng rít báo trước **0.6 s**
- Đưa từ `53` tới thẳng `57` (bỏ qua `55`, `56`)

**Đây là đường tắt tùy chọn, KHÔNG nằm trên đường tới hạn.** Validator bỏ qua nó, và
`58 Rest Roof` vẫn tới được bằng đường thường. Đúng nguyên tắc "dùng cho đường tắt tùy chọn
trước khi đưa vào thử thách khó".

### Tầng 11 — Buồng máy thẳng đứng (89.2 – 98.7 m) · cao trào

| # | Tên | Loại | x | rộng | y_lo | y_hi | Δy | Δx | p |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| 59 | Piston B7 | P-B | 1.2 | 2.2 | 90.00 | 92.20 | 0.80 | 0.40 | 0.35 |
| 60 | Piston B8 | P-B | −1.8 | 2.2 | 91.50 | 93.70 | 1.50 | 1.90 | 0.65 |
| 61 | Flywheel Ledge | tĩnh | 1.4 | 2.0 | — | 94.60 | 0.90 | 2.10 | 0.40 |
| 62 | **Piston C1** | P-C | −1.6 | 2.0 | 95.40 | 97.20 | 0.80 | 2.00 | 0.55 |
| 63 | **Piston C2** | P-C | 1.6 | 2.0 | 96.90 | 98.70 | 1.50 | 2.20 | 0.65 |

Đây là cảnh chủ đạo: băng qua buồng máy thẳng đứng trên các đầu piston, bánh đà lớn ở nền.
Hai cú cuối chuyển sang **P-C (3.4 s)** — cửa sổ siết từ 0.66 xuống **0.31 s**.
Bệ hẹp dần còn 2.0 m.

### Tầng 12 — Đối pha (98.7 – 108.0 m)

| # | Tên | Loại | x | rộng | y_lo | y_hi | Δy | Δx | p |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| 64 | Gauge Panel | tĩnh | −1.2 | 2.4 | — | 99.60 | 0.90 | 1.80 | 0.40 |
| 65 | Piston B9 | P-B φ0.5 | 1.8 | 2.4 | 100.40 | 102.60 | 0.80 | 1.80 | 0.40 |
| 66 | Piston B10 | P-B φ0.5 | −1.6 | 2.4 | 101.90 | 104.10 | 1.50 | 2.20 | 0.65 |
| 67 | Relief Valve | tĩnh | 1.2 | 2.2 | — | 105.00 | 0.90 | 1.60 | 0.40 |
| 68 | Piston B11 | P-B φ0 | −1.8 | 2.2 | 105.80 | 108.00 | 0.80 | 1.90 | 0.50 |

`65` và `66` **cùng đối pha với nhau** nên cú đỉnh→đỉnh giữa chúng vẫn hợp lệ — nhưng lệch 180°
so với mọi thứ trước đó. Người chơi quen nhịp cũ sẽ trượt lần đầu. **Đây là mục đích.**

`68` quay lại `φ = 0`, có `67` tĩnh xen giữa (đúng quy tắc #2 và #3: tối đa 3 piston liên tiếp —
chuỗi này là 65, 66 rồi dừng, 68 đứng riêng).

### Tầng 13 — Lò cao (108.0 – 115.3 m) · **P-C**

| # | Tên | Loại | x | rộng | y_lo | y_hi | Δy | Δx | p |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| 69 | Firebox Ledge | tĩnh | 1.0 | 2.8 | — | 108.90 | 0.90 | 1.70 | 0.40 |
| 70 | Piston C3 | P-C | −2.0 | 1.9 | 109.60 | 111.40 | 0.70 | 1.90 | 0.55 |
| 71 | Piston C4 | P-C | 0.6 | 1.9 | 111.10 | 112.90 | 1.50 | 1.65 | 0.65 |
| 72 | Piston C5 | P-C | 2.8 | 1.9 | 112.60 | 114.40 | 1.50 | 1.25 | 0.65 |
| 73 | Hopper Shelf | tĩnh | 0.0 | 2.4 | — | 115.30 | 0.90 | 1.85 | 0.42 |

Ba piston nhanh liên hoàn — chuỗi dài nhất khu (đúng mức tối đa 3 của quy tắc #3),
bệ hẹp nhất khu (1.9 m), cửa sổ 0.31 s.

### Tầng 14 — Nghỉ & chuyển khu (115.3 – 131.0 m)

| # | Tên | Loại | x | rộng | y_lo | y_hi | Δy | Δx | p |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| 74 | Coal Chute | tĩnh | −2.2 | 2.4 | — | 116.30 | 1.00 | 1.00 | 0.45 |
| 75 | Chain Hoist | tĩnh | 0.8 | 2.6 | — | 117.60 | 1.30 | 1.80 | 0.57 |
| 76 | **Rest Roof — Boiler House** | tĩnh | −1.4 | 3.6 | — | 118.90 | 1.30 | 0.90 | 0.57 |
| 77 | Exhaust Shaft | tĩnh | 1.2 | 2.2 | — | 120.20 | 1.30 | 0.80 | 0.57 |
| 78 | Piston B12 | P-B | −1.6 | 2.2 | 121.10 | 123.30 | 0.90 | 1.70 | 0.40 |
| 79 | Gantry | tĩnh | 1.8 | 2.2 | — | 124.20 | 0.90 | 2.30 | 0.40 |
| 80 | Condenser Deck | tĩnh | −1.0 | 3.0 | — | 125.60 | 1.40 | 1.70 | 0.61 |
| 81 | Transition Arch | tĩnh | 0.6 | 2.6 | — | 127.00 | 1.40 | 0.10 | 0.61 |
| 82 | First Cable | tĩnh | −1.8 | 2.4 | — | 128.40 | 1.40 | 1.10 | 0.61 |
| 83 | Threshold of Wires | tĩnh | 1.0 | 3.0 | — | 129.80 | 1.40 | 1.60 | 0.61 |

Chuyển khu bắt đầu ngay tại `80 Condenser Deck`: ống đồng nhường chỗ cho dây dẫn,
ánh đèn điện đầu tiên xuất hiện giữa khói. **Ranh giới khu tại y = 131.0.**

---

## 7. Cấu trúc rơi

| Vị trí | Rơi xuống | Độ sâu | Loại |
|---|---|---|---|
| Trượt piston 45/46 | 44 (66.90) | 0.4–3.8 m | Bệ bắt rơi |
| Trượt chuỗi 48–50 | 47 (71.60) | 1.0–4.7 m | Ngắn |
| Trượt 52 | 51 (78.70) | 0.6–2.8 m | Bệ bắt rơi |
| Trượt chuỗi 56–57 | 55 (83.70) | 0.9–4.6 m | Ngắn |
| Trượt 59–63 | 58 (89.20) | 0.8–9.5 m | **Trung bình** |
| Trượt 65–66 | 64 (99.60) | 0.8–4.5 m | Ngắn |
| Trượt 70–72 | 69 (108.90) | 0.7–5.5 m | Trung bình |
| Sai từ tầng 13 | 58 (89.20) | 19.7 m | **Dài — 1 lần** |
| Rơi khỏi mép phải tầng 12 | 61 (94.60) | 4.4 m | Ngắn |

**Hai mái nghỉ thật** (`58` y 89.20 và `76` y 118.90) chia khu thành 3 phần, nên cú rơi dài nhất
không bao giờ vượt ~20 m. Phù hợp mức "2 khu" trong kế hoạch.

---

## 8. Đánh số bệ

### Vì sao tiếp số 41–83 chứ không đánh lại từ 01

Số bệ là **khoá thứ tự tuyến leo**: validator (`SpireChecks`) và builder dựng scene đọc thứ tự này
để kiểm và mọc liên kết theo đúng đường leo. Nếu mỗi khu đánh lại từ 01, hai bệ trùng số ở hai khu
khác nhau sẽ không phân biệt được.

**Vì vậy số phải là duy nhất và tăng dần theo độ cao trên toàn tháp.** Khu 1 = 01–40,
khu 2 = 41–83, khu 3 bắt đầu từ 84.

| Hạng mục | Giá trị |
|---|---|
| Bệ khu 2 | 41–83 (43 bệ) |
| Không đánh số | `Vent Updraft`, mọi khối trang trí, tường, trần |

> **Đã chốt 20/09/2026:** loại bỏ hoàn toàn cơ chế coin (chỉ là demo) — không nhóm 5 bệ, không
> điểm số. Số bệ giữ nguyên vì phục vụ thứ tự tuyến leo và validator.

---

## 9. Tiêu chí validator

`SpireChecks` phải xác nhận:

- [ ] Mọi liên kết **đường tới hạn** 41→83 tới được bằng mô phỏng vật lý thật.
- [ ] Với bệ piston: mô phỏng đặt piston ở **đúng vị trí pha tại thời điểm đáp** tính theo
      `t₀`, không phải đứng yên ở đáy.
- [ ] `Δy` mọi liên kết ≤ **2.64 m**.
- [ ] Cửa sổ thả nút mọi piston **≥ 0.30 s** (P-C là biên dưới).
- [ ] Mọi piston có ≥ **3.2 m** khoảng trống phía trên đỉnh.
- [ ] **Không** liên kết nào yêu cầu đứng trên piston đang hạ (§5, rủi ro #2).
- [ ] `Vent Updraft` bị **loại khỏi** kiểm tra đường tới hạn, nhưng tự nó kiểm tra riêng:
      vào từ dưới → ra ở đúng tầm `57`.
- [ ] Chuỗi piston liên tiếp tối đa = **3**.
- [ ] Mọi piston trong một chuỗi nhảy ngang có **cùng pha**.
- [ ] 43 bệ đánh số, ranh giới khu = 131.0.

---

## 10. Thứ tự triển khai đề xuất

| Bước | Việc | Vì sao trước |
|---:|---|---|
| 1 | Sửa `DummyController` cho nền tảng động (§5 rủi ro #1) | Không có bước này thì khu 2 không chơi được |
| 2 | Viết `MovingPiston` với biên dạng hình thang + đồng hồ chung | Nền tảng cho mọi tầng |
| 3 | Dựng **tầng 8** và playtest tay | Xác nhận cảm giác leo piston |
| 4 | Mở rộng validator cho liên kết phụ thuộc thời gian | Trước khi nhân bản ra 11 piston còn lại |
| 5 | Dựng tầng 9–14 | Chỉ sau khi nhịp đã đúng |
