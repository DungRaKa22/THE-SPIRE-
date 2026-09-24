# KHU 3 — ELECTRIC AGE (131 – 194 m)

> Thiết kế bố cục tầng chi tiết. Khu 2 dạy **nhịp** (bệ di chuyển theo chu kỳ); khu 3 dạy
> **đọc trạng thái** (bệ đứng yên, nhưng trạng thái an toàn / cảnh báo / nhiễm điện chạy theo chu kỳ).
> Đây là trục độ khó khác hẳn: người chơi không canh thời điểm bệ *đi qua*, mà canh thời điểm
> bệ *chuyển trạng thái*.
>
> Khu 3 dùng các bệ đánh số **84–125** (tiếp số khu 2 — xem §9 để biết vì sao không đánh lại từ 01).

---

## 1. Mục tiêu thiết kế

- **Không có bệ nào di chuyển.** Mọi bệ đứng yên tuyệt đối; chỉ *trạng thái* thay đổi.
  Nhờ vậy người chơi đọc được bố cục từ xa, và độ khó nằm hoàn toàn ở thời điểm.
- **Học: trạng thái là thứ phải đọc trước khi tích lực.** Khác khu 1–2, ở đây đứng yên cũng
  có thể mất cú nhảy.
- **Nguy hiểm không trừ máu, không giết.** Chạm bệ nhiễm điện → hất ngược lại (xem §2.4).
  Hậu quả là mất vị trí và có thể rơi — đúng tài liệu ý tưởng.
- **Không đoán mò.** Ba trạng thái phải phân biệt được bằng *hình dạng*, không cần âm thanh;
  và cảnh báo phải đủ dài để nhìn thấy.
- **Một mechanic chính (Electric Platform) + một biến thể (Electric Bounce)** — biến thể chỉ
  xuất hiện sau khi mechanic chính đã dạy xong.

---

## 2. Bốn profile

### 2.1 Mạch điện — đơn vị thiết kế, không phải từng bệ

Các bệ được nối vào **mạch** (`ElectricCircuit`). Một mạch có một chu kỳ `T` và một đồng hồ chung;
mỗi bệ chỉ khác nhau ở **lệch pha `φ`** so với mốc của mạch.

```
u(t) = (t + φ) mod T
SAFE     khi u < t_safe
WARN     khi u < t_safe + t_warn
LIVE     khi u ≤ u < T
```

Ba profile dùng trong khu 3:

| Profile | T | SAFE | WARN | LIVE | Tỷ lệ an toàn | Dùng ở |
|---|---:|---:|---:|---:|---:|---|
| **E-A** | 6.0 s | 4.0 s | 0.8 s | 1.2 s | 67 % | Tầng 15–16 (giới thiệu) |
| **E-B** | 4.6 s | 2.8 s | 0.6 s | 1.2 s | 61 % | Tầng 17–19 (thân khu) |
| **E-C** | 3.4 s | 1.9 s | 0.5 s | 1.0 s | 56 % | Tầng 20–21 (cao trào) |

> **Luật cứng:** `WARN ≥ 0.5 s`. Cảnh báo ngắn hơn thì người chơi không kịp nhìn thấy,
> và mechanic biến thành trò đoán. Khu 3 giữ đúng 0.5–0.8 s.

### 2.2 Đồng hồ chung (bắt buộc)

Cả khu dùng **một đồng hồ duy nhất** `Time.time` (thang thời gian có scale, để pause đóng băng
đúng lúc). Bệ **không** tự tích lũy `deltaTime` riêng — nếu mỗi bệ tự đếm, chỉ cần một frame
trôi là cả chuỗi lệch pha và mọi tính toán cửa sổ ở §3–§4 mất giá trị.

Hệ quả cần chấp nhận: pha tuyệt đối lúc người chơi tới là **không đoán trước được**.
Điều này là *có lợi* — nó buộc phải đọc trạng thái thay vì đếm nhịp. Toàn bộ §3–§4 vì vậy
được viết theo **thời gian tương đối `u` tính từ lúc tiếp đất**, không theo mốc tuyệt đối.

### 2.3 Bệ điện — ba trạng thái, hai nguy cơ khác nhau

| Trạng thái | Va chạm | Hình dạng / tín hiệu | Nguy cơ |
|---|---|---|---|
| **SAFE** | đặc, đứng được | viền liền, đèn nền dịu | không |
| **WARN** | đặc, đứng được | viền đứt, chớp 4 Hz, tia lửa nhỏ ở mép | **sắp** có điện — phải rời đi |
| **LIVE** | đặc, vẫn đứng được về mặt vật lý | hồ quang giữa hai cực, chữ "▲ HẤT" | **bị hất** nếu đang đứng trên |

Điểm quan trọng: bệ **không bao giờ mất va chạm**. Nó chỉ hất. Nếu tắt collider, người chơi
rơi xuyên qua và toàn bộ mô hình thời gian ở §3 sụp (xem §5 rủi ro #2).

### 2.4 Vận tốc hất — cố định, có hướng

```
dir_in  = sign(vx) nếu |vx| ≥ 0.5, ngược lại dùng Facing
v_hất    = (−dir_in × 1.5 , +7.5)      // m/s
```

- Hất **ngược hướng vừa đi tới** → người chơi hiểu ngay "bị đẩy lùi".
- Độ cao bật lên `7.5² / 58.86 = 0.956 m`; trôi ngang `1.5 × 0.51 = 0.76 m`.
- Đủ để đẩy khỏi mép một bệ rộng 2.2 m nếu đứng gần mép, nhưng **không** đủ để ném xuống
  nếu đứng giữa bệ. Trừng phạt nhẹ, đọc được, không ngẫu nhiên.
- Trong lúc LIVE kéo dài 1.0–1.2 s, người chơi có thể bị hất 2 lần liên tiếp (mỗi lần 0.51 s).
  Đây là *cảm giác điện giật* mong muốn, và nó **có biên** vì LIVE hữu hạn.
- Hất tự động hủy tích lực: `DummyController` gọi `CancelCharge()` mỗi frame khi `!Grounded`
  (đã đọc code, dòng `if (!Grounded) CancelCharge();`) → không cần code thêm.

---

## 3. Toán trạng thái

### 3.1 Mô hình

Đứng trên bệ nguồn `S`, tiếp đất ở thời điểm quy ước `u = 0` (tính theo pha của `S`).
Người chơi cần:

| Bước | Thời lượng | Ghi chú |
|---|---|---|
| Ổn định sau tiếp đất | `t_settle = 0.15 s` | không thể bắt đầu tích lực sớm hơn |
| Tích lực | `t_c(p) = 0.85 · p` | `p` = mức tích lực, tính từ bảng §3.3 |
| Bay | `t₂(Δy, p)` | nhánh **rơi xuống** qua đỉnh — xem `Docs/TheSpire-Level-Sector1.md` §1 |
| Biên an toàn khi thả | `margin = 0.10 s` | thả muộn hơn thì bệ đã nhiễm điện |

Điều kiện hợp lệ:

```
(1) u ∈ [ t_settle , t_safe(S) − margin − t_c ]          rời bệ nguồn khi còn SAFE
(2) A(u) = ( u + t_c + t₂ + Δφ ) mod T  ≤  t_safe(D) − t_settle   tiếp đất khi bệ đích SAFE
    với Δφ = φ_D − φ_S
```

`W` = bề rộng cửa sổ tiếp đất hợp lệ (đơn vị: giây, đo trong không gian `u`).
`U_max = t_safe(S) − margin − t_c` = biên trên do điều kiện (1).

### 3.2 Nghiệm tổng quát — hai nhánh

Giải (2) cho `u`, tách theo việc `u + t_c + t₂ + Δφ` có vượt `T` hay không:

| Nhánh | Điều kiện | Cửa sổ `u` | Tồn tại khi |
|---|---|---|---|
| **Không vòng** | `u ≤ t_safe(D) − t_settle − t_c − t₂ − Δφ` | `[t_settle, min(U_max, t_safe − t_settle − t_c − t₂ − Δφ)]` | `W(Δφ = 0) ≥ Δφ` |
| **Vòng** | `u ≥ T − t_c − t₂ − Δφ` | `[max(t_settle, T − t_c − t₂ − Δφ), U_max]` | `U_max ≥ T − t_c − t₂ − Δφ` |

**Hệ quả 1 — cửa sổ rộng nhất không nằm ở `Δφ = 0`.** Khi `Δφ ≥ T − t_c − t₂ − t_settle`
(nhánh vòng), cửa sổ bằng `U_max − t_settle = t_safe − margin − t_c − t_settle` — tức là
**luôn rộng hơn `Δφ = 0`**, vì bệ đích vừa tắt điện đúng lúc người chơi tới và còn nguyên
cả chu kỳ an toàn phía trước.

**Hệ quả 2 — `Δφ = T/4` có thể *không tồn tại*.** Trên mạch ngắn (E-C, `T = 3.4 s`), thời gian
bay `t₂ ≈ 0.44–0.50 s` cộng thời gian tích lực `≈ 0.55–0.68 s` đã xấp xỉ `T/4 = 0.85 s`,
nên cả hai nhánh đều rỗng. **Lệch pha 90° trên E-C là bất khả thi về mặt toán học, không phải
"khó"** — validator phải chặn cấu hình này (xem §10).

### 3.3 Bảng tra nhanh `t_c + t₂`

`t_c = 0.85p`; `t₂ = (v + √(v² − 58.86·Δy)) / 29.43` với `v = 4 + 9p`.
Cột `p_min = (8·√Δy − 4) / 9` (từ luật `Δy ≤ 0.92·H`, xem khu 1 §1).

| Δy (m) | `p_min` | p dùng | `t_c` | `t₂` | `t_c + t₂` |
|---:|---:|---:|---:|---:|---:|
| 1.30 | 0.569 | 0.60 | 0.510 | 0.436 | **0.946** |
| 1.40 | 0.607 | 0.62 | 0.527 | 0.430 | **0.957** |
| 1.45 | 0.626 | 0.66 | 0.561 | 0.462 | **1.023** |
| 1.45 | 0.626 | 0.68 | 0.578 | 0.484 | **1.062** |
| 1.45 | 0.626 | 0.72 | 0.612 | 0.524 | **1.136** |
| 1.50 | 0.644 | 0.68 | 0.578 | 0.471 | **1.049** |
| 1.50 | 0.644 | 0.72 | 0.612 | 0.514 | **1.126** |
| 1.60 | 0.680 | 0.72 | 0.612 | 0.490 | **1.102** |
| 1.65 | 0.697 | 0.74 | 0.629 | 0.500 | **1.129** |
| 0.90 | 0.398 | 0.65 | 0.553 | 0.560 | **1.113** |

> So sánh với khu 2: ở đây **thời gian bay ngắn hơn** (`t₂ ≈ 0.43–0.56 s` so với 0.45–0.65 s của
> piston) nhưng **thời gian tích lực dài hơn** vì khu 3 đẩy `Δy` lên 1.45–1.65 m. Tổng gần bằng nhau.

---

## 4. Bảng cửa sổ theo lệch pha

Mọi giá trị dưới đây tính bằng công thức §3.2, cho cặp `(Δy, p)` đại diện của từng mạch.

### 4.1 `W` (giây) — bảng then chốt của khu

| `Δφ` | Ý nghĩa | E-A (T 6.0) | E-B (T 4.6) | E-C (T 3.4) |
|---|---|---:|---:|---:|
| `0` (0°) | cùng pha | 2.638 | 1.451 | 0.551 |
| `T/4` (90°) | lệch một phần tư | 1.138 | 0.301 | **∅ không dùng được** |
| `T/2` (180°) | đối pha | 1.384 | 0.871 | 0.571 |
| `3T/4` (270°) | lệch ba phần tư | 2.884 | 1.972 | 1.072 |
| *nguồn tĩnh* | chỉ ràng buộc đích | 3.41 | 2.18 | 1.28 |

*(Cặp tham chiếu: E-A `Δy 1.45 / p 0.68`; E-B `Δy 1.50 / p 0.68`; E-C `Δy 1.50 / p 0.68`.)

Dòng **nguồn tĩnh** là trường hợp bệ nguồn không bao giờ chuyển trạng thái: chỉ còn ràng buộc (2),
nên cửa sổ đo trong **không gian thời điểm thả nút** và bằng `t_safe(D) − t_settle − t₂`.
Chú ý **vẫn phải trừ `t₂`** — người chơi rời bệ sớm bao nhiêu cũng được, nhưng lúc *tiếp đất*
mới là lúc bệ đích phải còn SAFE. Đây là lỗi dễ mắc nhất khi tính tay: lấy nguyên `t_safe − t_settle`
sẽ thổi phồng cửa sổ thêm đúng một khoảng bằng thời gian bay.*

### 4.2 Bốn luật rút ra

1. **`Δφ = 3T/4` là lệch pha *dễ nhất*, không phải khó nhất.** Người chơi phải chờ một chút
   (bệ đích đang ở cuối LIVE) rồi được thưởng cửa sổ gần gấp đôi cùng pha. Đây là công cụ
   để làm dịu một chuỗi dài mà không phải hạ `Δy`.
2. **`Δφ = T/4` là lệch pha khó nhất** — buộc nhảy gần như ngay sau khi tiếp đất.
   Điều kiện tồn tại gọn lại thành: **`T/4 ≤ W(Δφ = 0)`** (vì `W` khi cùng pha bằng đúng
   `t_safe − 2·t_settle − t_c − t₂`). Với E-B: `1.15 ≤ 1.451` ✓. Với E-C: `0.85 ≤ 0.551` ✗ → **cấm**.
   Kiểm tra được bằng một dòng, không cần thử từng liên kết.
3. **Đối pha (`T/2`) buộc chờ muộn.** Cửa sổ không bắt đầu ở `u = 0.15` mà ở
   `u = T − t_c − t₂ − T/2`; với E-B là `u ≈ 1.25 s`. Đây là bài học "đứng yên chờ" —
   trái ngược hoàn toàn với khu 2, nơi cùng pha là dễ nhất.
4. **Nguồn tĩnh → bệ điện luôn dễ hơn bệ điện → bệ điện.** Bệ nguồn không bao giờ khóa
   người chơi, nên cửa sổ bằng `t_safe(D) − t_settle − t₂` — vẫn rộng hơn cùng pha ở mọi mạch,
   vì mất ràng buộc (1) mà chỉ giữ ràng buộc (2).
   Đây là hình mẫu để giới thiệu mỗi mạch mới: **luôn đặt một bệ tĩnh ngay trước đoạn điện đầu tiên.**

### 4.3 Thứ tự giới thiệu trong khu (đúng luật §4.2)

| Bậc | Nội dung | `W` | Tầng |
|---|---|---:|---|
| Học | nguồn tĩnh → E-A | 3.41 | 15 |
| Học | E-A → E-A, cùng pha | 2.64 | 15–16 |
| Luyện | E-A → E-A, `T/4` (khó nhất của E-A) | 1.14 | 16 |
| Luyện | E-A → E-A, `3T/4` (thưởng) | 2.86 | 16 |
| Kết hợp | mạch nhanh hơn E-B, cùng pha | 1.45 | 17–18 |
| Kết hợp | E-B đối pha — **chờ muộn** | 0.87 | 17, 19 |
| Kết hợp | nguồn tĩnh → E-B (mạch nhanh hơn) | 2.22 | 18 |
| Cao trào | E-C cùng pha | 0.55 | 20 |
| Cao trào | E-C `3T/4` (thưởng giữa cao trào) | 1.07 | 20 |
| Cao trào | E-C đối pha | 0.57–0.60 | 21 |

---

## 5. ⚠️ Rủi ro triển khai

Đây là phần quan trọng nhất của tài liệu này. Mỗi mục đều đã đối chiếu với code hiện có.

> **Trạng thái code — rủi ro #1, #2, #3, #4, #5 đã được bịt:**
>
> | File | Việc đã làm |
> |---|---|
> | `Assets/Scripts/DummyController.cs` | Thêm `ApplyImpulse(Vector2)` (hất, hủy tích lực, đặt `ignoreGroundUntil`) và mốc dùng chung `LastImpulseTime` |
> | `Assets/Scripts/Cyberpunk/CyberRunnerVisual.cs` | Đọc `LastImpulseTime` thay cho `LastWallBounceTime` → animation `Bounce` phát cho cả hất điện |
> | `Assets/Scripts/ElectricCircuit.cs` | MỚI: đồng hồ dùng chung + ba preset E-A / E-B / E-C + `OffsetForImpendingLive` |
> | `Assets/Scripts/ElectricPlatform.cs` | MỚI: ba trạng thái, poll `FixedUpdate`, không bao giờ tắt collider |
> | `Assets/Editor/ElectricPlatformChecks.cs` | MỚI: play-check chứng minh cả hai lỗi không tái phát |
>
> Ba rủi ro còn lại không phụ thuộc vào việc người dựng có nhớ hay không:
> #3 được ép bằng **API** (`ApplyImpulse` là đường duy nhất để hất), #4 được ép bằng **cấu trúc**
> (`ElectricPlatform` bắt buộc có `ElectricCircuit`), #6 sẽ được ép bằng **builder đặt số tự động**
> + validator (chưa viết).
>
> Đã biên dịch sạch bằng Unity 6000.5.7f1 ở chế độ batch. Kiểm chứng hành vi cần **Play Mode**:
> mở scene có nhân vật → menu *JumpDummy → Run Electric Platform Checks* → kết quả ghi vào
> `Docs/TheSpireElectricValidation.txt` (cùng quy ước với 4 check đang có).

### Rủi ro #1 — `OnCollisionEnter2D` **không** bắn khi người chơi đang đứng sẵn

`DummyController.OnCollisionEnter2D` mở đầu bằng `if (Grounded) return;`.
Nghĩa là: người chơi **đang đứng** trên bệ khi bệ chuyển sang LIVE sẽ **không** sinh va chạm mới,
và nếu `ElectricPlatform` chờ `OnCollisionEnter2D` để hất thì **sẽ không bao giờ hất** —
mechanic mất hoàn toàn, nhưng bug này im lặng (không log, không crash).

**Bắt buộc:** bệ phải **tự hỏi mỗi `FixedUpdate`** khi `LIVE`:

```
Physics2D.OverlapBox(topCenter, new Vector2(width, 0.14f), 0f, filter) → lấy player
nếu player.Grounded và player.transform.position.y ≥ top − 0.05  → hất, rồi đặt cooldown 0.20 s
```

Kiểm tra thêm `player.Grounded` (`DummyController.Grounded` là `public`) để không hất khi
người chơi chỉ bay ngang qua. Cooldown 0.20 s tránh hất 2 lần trong cùng một lần tiếp xúc.

### Rủi ro #2 — `Grounded` dùng raycast **không** tính trigger

Trong `DummyController.FixedUpdate`:

```
groundFilter = new ContactFilter2D { useTriggers = false };
shape.Cast(Vector2.down, groundFilter, groundHits, 0.045f)
```

Hai hệ quả:

- Bệ điện **phải là collider thường** (`isTrigger = false`). Nếu làm bệ bằng trigger (ý tưởng
  "vùng điện" rất tự nhiên khi nghĩ về điện), người chơi sẽ **rơi xuyên** và không bao giờ đứng được.
- **Không bao giờ** đặt trạng thái bằng cách `collider.enabled = false` cho LIVE. Việc tắt
  collider làm người chơi rơi xuống, mất `Grounded`, mất tích lực, và phá vỡ toàn bộ mô hình §3.

### Rủi ro #3 — Hất trong callback va chạm có thể bị ghi đè

`DummyController.OnCollisionEnter2D` kết thúc bằng `body.linearVelocity = velocity;`.
Nếu `ElectricPlatform` đặt vận tốc hất bên trong một callback va chạm, thứ tự thực thi giữa
hai `OnCollisionEnter2D` là không đảm bảo — và nhánh chạm trần (`normal.y < -0.7f`) của người
chơi sẽ **zero hóa `velocity.y`**, xóa luôn cú hất.

**Bắt buộc:** đặt vận tốc hất trong `FixedUpdate` của bệ (hoặc bơm vào một hàng đợi cho
`DummyController` tiêu thụ trong `FixedUpdate` của nó). Sau khi hất, người chơi không còn
`Grounded` nên nhánh `if (Grounded)` của controller không ghi đè lại.

### Rủi ro #4 — Đồng hồ tách rời

Nếu mỗi bệ tự tích lũy pha riêng, sai số một frame trên mỗi bệ sẽ làm lệch pha tương đối giữa
các bệ trong cùng mạch. Vì mọi cửa sổ ở §4 tính bằng **hiệu pha**, lệch 0.05 s là đủ để biến
cửa sổ 0.30 s thành cửa sổ âm.

**Bắt buộc:** một `ElectricCircuit` giữ pha; bệ chỉ khai báo `offset`. Không `accumulator += dt`
ở cấp bệ. Dùng `Time.time` (đã scale) để pause đóng băng đúng — **không** dùng `unscaledTime`.

### Rủi ro #5 — Animation `Bounce` không chạy cho cú hất điện

`CyberRunnerVisual` chọn trạng thái `"Bounce"` theo
`Time.time - player.LastWallBounceTime < 0.16f`, mà `LastWallBounceTime` **chỉ** được đặt trong
nhánh va chạm tường của `DummyController`. Cú hất điện và thanh dẫn nảy không đi qua nhánh đó,
nên nếu không sửa, phản hồi thị giác quan trọng nhất của khu sẽ **không bao giờ phát**.

**Bắt buộc:** thêm một mốc dùng chung (ví dụ `LastImpulseTime`) được đặt cho cả va chạm tường,
hất điện và thanh dẫn nảy; `CyberRunnerVisual` đọc mốc đó. Giữ nguyên chữ ký `Bounce` để
không phải sửa Animator.

### Rủi ro #6 — quy ước tên bệ

Validator và builder lọc bệ theo số ở **token đầu tiên** của tên:

```
int.TryParse(p.name.Split(' ')[0], out _)  &&  !p.isTrigger  &&  p.enabled
```

→ **số phải là token đầu tiên.** `"86 Coil Deck A"` ✓ an toàn; `"E-A 86 Coil Deck"` ✗ sẽ bị
bỏ qua (không tính là bệ) và làm lệch **toàn bộ** thứ tự tuyến leo.
Tên bệ điện **không** được chứa tiền tố chữ. Đưa vào validator (§10).

---

## 6. Bố cục 42 bệ

Sàn đấu **12 m** (x ∈ [−6, +6]); tường trong tại ±6.0, dày 0.5, cao từ 131.0 tới 194.0.
Bệ đứng yên hoàn toàn: chỉ có `y_đỉnh`, không có `y_lo`/`y_hi`.
`Δy` = đỉnh bệ nguồn → đỉnh bệ đích. `Δx` = mép gần của bệ nguồn → **tâm** bệ đích.

Ký hiệu cột **φ**: lệch pha theo độ (và giây trong ngoặc).

### Tầng 15 — Cổng vào nhà máy điện (131.0 – 140.0) · mạch **E-A**

| # | Tên | Loại | x | rộng | y | φ | Δy | Δx | p | W |
|---|---|---:|---:|---:|---:|---|---:|---:|---:|---:|
| 84 | Power Threshold | tĩnh | −0.4 | 3.0 | 131.20 | — | 1.40 | 0.10 | 0.61 | — |
| 85 | Cable Spool | tĩnh | 2.4 | 2.6 | 132.50 | — | 1.30 | 1.30 | 0.60 | — |
| 86 | Coil Deck A | E-A | −0.2 | 2.8 | 133.80 | 0° (0.00) | 1.30 | 1.30 | 0.60 | 3.41 |
| 87 | Coil Deck B | E-A | 2.6 | 2.6 | 135.10 | 0° (0.00) | 1.30 | 1.40 | 0.60 | 2.75 |
| 88 | Meter Shelf | tĩnh | 0.0 | 2.6 | 136.50 | — | 1.40 | 1.30 | 0.62 | — |
| 89 | **Rest Roof — Relay House** | tĩnh | 2.2 | 3.6 | 137.90 | — | 1.40 | 0.90 | 0.62 | — |

`84` là bệ đáp từ khu 2 (nối tiếp `83 Threshold of Wires`, y 129.80, x 1.0, rộng 3.0).
Hai bệ điện đầu tiên **cùng pha, kề bệ tĩnh hai bên**: cú vào `85 → 86` có nguồn tĩnh nên cửa sổ
3.41 s, cú `86 → 87` là cú điện → điện đầu tiên với cửa sổ 2.75 s — dài gấp 5 lần cao trào
của khu. Người chơi học câu duy nhất cần học: *nhìn viền bệ trước khi tích lực*.

Khối trang trí có va chạm `Insulator Mast` (không đánh số): x ∈ [−5.6, −3.4], mặt dưới y = **137.4**,
đỉnh 138.0 — tạo một chỗ trú hợp lệ cho cú nhảy 88 → 89 nếu người chơi tích quá tay.

### Tầng 16 — Sân máy biến áp (140.0 – 149.3) · **E-A**, học lệch pha

| # | Tên | Loại | x | rộng | y | φ | Δy | Δx | p | W |
|---|---|---:|---:|---:|---:|---|---:|---:|---:|---:|
| 90 | Slot Panel | tĩnh | −0.6 | 2.8 | 139.30 | — | 1.40 | 1.00 | 0.62 | — |
| 91 | Transformer Base | tĩnh | 2.0 | 2.8 | 140.70 | — | 1.40 | 1.20 | 0.62 | — |
| 92 | Bus Bar A | E-A | −0.8 | 2.6 | 142.10 | 0° (0.00) | 1.40 | 1.40 | 0.62 | 2.74 |
| 93 | Bus Bar B | E-A | 1.6 | 2.6 | 143.50 | 0° (0.00) | 1.40 | 1.10 | 0.62 | 2.74 |
| 94 | Regulator Deck | E-A | −1.4 | 2.4 | 144.95 | **+90° (1.50)** | 1.45 | 1.70 | 0.68 | **1.14** |
| 95 | Regulator Deck B | E-A | 1.4 | 2.4 | 146.40 | **+90° (1.50)** | 1.45 | 1.60 | 0.68 | 2.64 |
| 96 | Arc Terminal | E-A | −1.0 | 2.6 | 147.85 | 0° (0.00) | 1.45 | 1.20 | 0.66 | 2.86 |
| 97 | Insulator Stack | tĩnh | 1.2 | 2.4 | 149.30 | — | 1.45 | 0.90 | 0.66 | — |

`93 → 94` là **cú lệch pha 90° đầu tiên** của cả dự án: cửa sổ rơi từ 2.74 xuống **1.14 s** mà
bố cục không hề thay đổi. Đây là minh chứng cho luận điểm chính của khu: *độ khó nằm ở thời điểm,
không ở hình học.* Người chơi quen "cứ nhảy là được" sẽ trượt lần đầu.

`94 → 95` cùng pha trở lại (thở ra), `95 → 96` lệch `3T/4` — **thưởng**: cửa sổ 2.86 s,
rộng hơn cả cùng pha. Dạy luật §4.2 mục 1 ngay sau khi dạy cái khó.

### Tầng 17 — Dãy cầu dao (149.3 – 158.1) · mạch **E-B**

| # | Tên | Loại | x | rộng | y | φ | Δy | Δx | p | W |
|---|---|---:|---:|---:|---:|---|---:|---:|---:|---:|
| 98 | Breaker Row | E-B | −1.6 | 2.6 | 150.75 | 0° (0.00) | 1.45 | 1.60 | 0.66 | 2.19 |
| 99 | Breaker Row B | E-B | 1.0 | 2.4 | 152.25 | 0° (0.00) | 1.50 | 1.30 | 0.68 | 1.45 |
| 100 | Feeder Box | E-B | −1.2 | 2.4 | 153.75 | 0° (0.00) | 1.50 | 1.00 | 0.68 | 1.45 |
| 101 | Breaker Row C | E-B | 1.4 | 2.4 | 155.20 | **+180° (2.30)** | 1.45 | 1.40 | 0.66 | **0.87** |
| 102 | Feeder Box B | E-B | −0.8 | 2.2 | 156.65 | **+180° (2.30)** | 1.45 | 1.00 | 0.66 | 1.48 |
| 103 | Gauge Wall | tĩnh | 1.8 | 2.4 | 158.10 | — | 1.45 | 1.50 | 0.66 | — |

`97` (bệ tĩnh) đứng ngay trước `98` → bậc "học" của E-B đúng luật §4.2 mục 4: nguồn tĩnh,
cửa sổ 2.19 s, người chơi quan sát trọn một chu kỳ E-B rồi mới nhảy.

`100 → 101` là **cú đối pha đầu tiên**: cửa sổ không mở ở `u = 0.15` mà ở `u ≈ 1.25 s`.
Người chơi phải **đứng chờ gần hết pha an toàn rồi mới nhảy** — ngược lại hoàn toàn với phản xạ
đã hình thành ở khu 2. Cửa sổ 0.87 s, không tàn nhẫn, nhưng đủ để trượt nếu nhảy sớm.

### Tầng 18 — Băng qua chuỗi sứ (158.1 – 167.8) · E-B + **thanh dẫn nảy R1**

| # | Tên | Loại | x | rộng | y | φ | Δy | Δx | p | W |
|---|---|---:|---:|---:|---:|---|---:|---:|---:|---:|
| 104 | Arcing Horn | E-B | −1.0 | 2.4 | 159.60 | 0° (0.00) | 1.50 | 1.60 | 0.68 | 1.45 |
| 105 | Cable Head | E-B | 1.6 | 2.4 | 161.10 | 0° (0.00) | 1.50 | 1.40 | 0.68 | 1.45 |
| 106 | **Bounce Rail R1** | nảy | 0.0 | 2.2 | 162.55 | luôn bật | 1.45 | 0.40 | 0.65 | — |
| 107 | Rail Landing Deck | tĩnh | −3.08 | 2.8 | 164.95 | — | **2.40** | **3.08** | nảy | — |
| 108 | Condenser Coil | E-B | −0.4 | 2.4 | 166.35 | **+270° (3.45)** | 1.40 | 1.28 | 0.62 | 2.22 |
| 109 | **Rest Roof — Relay Yard** | tĩnh | −2.6 | 3.6 | 167.80 | — | 1.45 | 1.00 | 0.66 | — |

`105 → 106`: vào thanh dẫn nảy hướng sang trái (người chơi đi sang trái), `Δx` chỉ 0.40 m nên
tiếp xúc gần như chắc chắn. `106 → 107` là cú phóng của thanh dẫn: `Δy 2.40`, `Δx 3.08`,
**thuộc hình học đảm bảo** — xem §7.

`107` là bệ tĩnh → `108` có cửa sổ 2.22 s (rộng nhất trong E-B), và lệch pha `+270°` của `108`
không còn là ràng buộc: đứng trên bệ tĩnh thì người chơi chỉ cần chờ đúng lúc. Đây là
"cửa nghỉ kỹ thuật": sau cú phóng căng thẳng, người chơi được thưởng một cú dễ.

### Tầng 19 — Buồng tụ & thanh dẫn nảy (167.8 – 177.6) · E-B + **R2**

| # | Tên | Loại | x | rộng | y | φ | Δy | Δx | p | W |
|---|---|---:|---:|---:|---:|---|---:|---:|---:|---:|
| 110 | Capacitor Bank | E-B | 0.6 | 2.4 | 169.25 | 0° (0.00) | 1.45 | 1.40 | 0.66 | 1.48 |
| 111 | Capacitor Bank B | E-B | −1.6 | 2.4 | 170.75 | **+180° (2.30)** | 1.50 | 1.00 | 0.68 | **0.87** |
| 112 | Discharge Rod | E-B | 1.4 | 2.2 | 172.25 | 0° (0.00) | 1.50 | 1.80 | 0.72 | **0.91** |
| 113 | Surge Arrester | E-B | −1.4 | 2.2 | 173.70 | 0° (0.00) | 1.45 | 1.80 | 0.72 | 1.36 |
| 114 | **Bounce Rail R2** | nảy | 0.6 | 2.2 | 175.15 | luôn bật | 1.45 | 0.90 | 0.66 | — |
| 115 | Rail Landing Deck B | tĩnh | 3.68 | 2.8 | 177.55 | — | **2.40** | **3.08** | nảy | — |

Hai cú liên tiếp quanh `111` đều **đối pha** (`110 → 111` và `111 → 112`), nhưng lần này
`112` rộng 2.2 m và `p` phải lên 0.72 → cửa sổ 0.91 s. Đây là lần thứ hai gặp đối pha
(cố ý lặp để thành kỹ năng, không phải trò may rủi).

`R2` phóng sang **phải** (ngược `R1`) — buộc người chơi đọc lại ký hiệu hướng trên thanh dẫn
thay vì nhớ "cứ phóng sang trái".

### Tầng 20 — Tháp tải điện (177.6 – 185.1) · chuyển sang **E-C**

| # | Tên | Loại | x | rộng | y | φ | Δy | Δx | p | W |
|---|---|---:|---:|---:|---:|---|---:|---:|---:|---:|
| 116 | Tower Foot | E-C | 0.4 | 2.4 | 179.05 | 0° (0.00) | 1.50 | 1.88 | 0.72 | 1.24 |
| 117 | Tower Ring A | E-C | −1.8 | 2.4 | 180.55 | 0° (0.00) | 1.50 | 1.00 | 0.68 | 0.55 |
| 118 | Tower Ring B | E-C | 0.8 | 2.2 | 182.05 | **+270° (2.55)** | 1.50 | 1.40 | 0.68 | **1.07** |
| 119 | Tower Ring C | E-C | −1.4 | 2.2 | 183.55 | **+180° (1.70)** | 1.50 | 1.10 | 0.68 | **1.07** |
| 120 | Tower Ring D | E-C | 1.0 | 2.2 | 185.05 | 0° (0.00) | 1.50 | 1.30 | 0.68 | 0.57 |

`115` (bệ tĩnh, kết cấu lưới thép) → `116` là **bậc học của E-C**: nguồn tĩnh nên cửa sổ
`1.9 − 0.15 − 0.514 = 1.24 s` — vẫn rộng gấp 2.3 lần cửa sổ cùng pha của E-C (0.55 s),
và vẫn hẹp hơn **mọi** cửa sổ của E-B. Đó chính là tín hiệu "khu đã vào đoạn cuối".

`117 → 118 → 119` là chuỗi lệch `3T/4` rồi `3T/4` so với `0°`: sau khi đã quen E-C ở cửa sổ
0.55 s, người chơi nhận được hai cửa sổ 1.07 s — nhưng **phải chờ**, vì bệ đích đang ở cuối LIVE.
Chuỗi này dạy luật §4.2 mục 1 ở mạch nhanh nhất, nơi nó khó nhận ra nhất.

### Tầng 21 — Đỉnh biến áp (185.1 – 194.0) · **E-C** cao trào

| # | Tên | Loại | x | rộng | y | φ | Δy | Δx | p | W |
|---|---|---:|---:|---:|---:|---|---:|---:|---:|---:|
| 121 | Corona Ring | E-C | −1.2 | 2.2 | 186.65 | 0° (0.00) | 1.60 | 1.10 | 0.72 | **0.50** |
| 122 | Corona Ring B | E-C | 1.6 | 2.2 | 188.25 | 0° (0.00) | 1.60 | 1.70 | 0.72 | **0.50** |
| 123 | Surge Tower | E-C | −0.8 | 2.2 | 189.90 | **+180° (1.70)** | 1.65 | 1.30 | 0.74 | **0.60** |
| 124 | Last Wire | E-C | 1.4 | 2.2 | 191.50 | 0° (0.00) | 1.60 | 1.10 | 0.72 | **0.59** |
| 125 | **Rest Roof — High Tension Deck** | tĩnh | −1.6 | 3.6 | 192.40 | — | 0.90 | 1.90 | 0.65 | — |

Cao trào đúng nghĩa: bệ hẹp nhất khu (2.2 m), `Δy` cao nhất (1.65 m), `W` nhỏ nhất (0.50 s),
`p` cao nhất (0.74 — chỉ còn 0.11 trước trần 0.85 của luật tay người). Trần `1.65/0.92` → không
cú nào vượt `2.64 m` ✓.

`125` kết khu ở y **192.40**, ranh giới khu **y = 194.0** — chừa 1.6 m cho đoạn chuyển sang
khu 4 (Machine Age). Khối trang trí có va chạm `Busbar Gantry` (không đánh số): mặt dưới y = 194.6,
x ∈ [−2.0, 1.2] — vừa là trần của cú cuối, vừa là mép dưới của khu 4.

**Tổng: 84 → 125, đúng 42 bệ.**

---

## 7. Thanh dẫn nảy (Electric Bounce) — hình học đảm bảo

Biến thể theo tài liệu ý tưởng: bề mặt nảy với **lực cố định**, không ngẫu nhiên.
Đây không phải bệ điện — nó **luôn ở trạng thái nảy**, không có chu kỳ.

### 7.1 Xung lực

```
dir = sign(vx) nếu |vx| ≥ 0.5, ngược lại dùng Facing      // hướng vào
v_nảy = ( dir × 5.4 , +12.6 )                              // đặt, KHÔNG cộng thêm
```

Đặt `vx` chứ không cộng vào vận tốc cũ là điều kiện để cú nảy **hoàn toàn xác định** —
nếu cộng, tầm xa phụ thuộc cú nhảy trước, và không thể kiểm chứng bằng validator.

**Chỉ kích hoạt khi tiếp xúc từ trên** (`normal.y > 0.7`). Chạm cạnh bên thì chỉ là va chạm thường,
nên thanh dẫn không bao giờ là bẫy.

### 7.2 Bảng tầm với của thanh dẫn (`v₀ = 12.6`, `vx = 5.4`)

| Δy (m) | `t₂` | Δx (m) | Ghi chú |
|---:|---:|---:|---|
| 0.00 | 0.856 | **4.62** | tầm xa nhất |
| 0.50 | 0.815 | 4.40 | |
| 1.00 | 0.768 | 4.15 | |
| 1.50 | 0.714 | 3.85 | |
| 2.00 | 0.646 | 3.49 | |
| **2.40** | **0.570** | **3.08** | **giá trị dùng ở khu 3** |
| 2.50 | 0.544 | 2.94 | |
| 2.69 | 0.450 | 2.43 | sát đỉnh (97 % trần) |

Đỉnh `12.6² / 58.86 = 2.697 m`. **Trần thiết kế `Δy ≤ 2.45 m`** (91 % đỉnh) — để chừa biên cho
biến thể `Energy Rail` ở khu 6 dùng lại cùng quy ước nhưng cao hơn.

So sánh có chủ đích: đỉnh thanh dẫn `2.697 m` **cao hơn** cú nhảy `p = 0.85` (`2.31 m`)
nhưng **thấp hơn** trần lý thuyết `2.87 m`. Tầm xa `4.62 m` > `4.32 m` của `p = 0.85`.
Nghĩa là thanh dẫn là "chiêu mạnh", nhưng không bao giờ thay thế được cú nhảy cực đại —
đúng nguyên tắc "booster không được phá vỡ ngân sách nhảy".

### 7.3 Luật band — vì sao cú phóng không thể trượt

Người chơi có thể tiếp xúc thanh dẫn ở **bất kỳ điểm nào** trên mặt nó, và điểm xuất phát
quyết định điểm đáp. Nếu thanh dẫn rộng 2.2 m thì điểm đáp trải trên một **band** rộng đúng 2.2 m:

```
band = [ x_rail − w/2 + Δx_reach , x_rail + w/2 + Δx_reach ]
```

**Luật band:** bệ đích phải **chứa toàn bộ band**, với biên ≥ 0.30 m mỗi phía:

```
x_đích = x_rail + Δx_reach            (tâm bệ đích)
w_đích ≥ w_rail + 0.60
```

Kiểm tra hai thanh dẫn của khu 3:

| | `x_rail` | `w_rail` | hướng | band | bệ đích | tâm | rộng | biên | ✓ |
|---|---:|---:|---|---|---|---:|---:|---:|---|
| **R1** | 0.0 | 2.2 | trái | `[−4.18, −1.98]` | 107 | −3.08 | 2.8 | 0.30 | ✓ |
| **R2** | 0.6 | 2.2 | phải | `[2.58, 4.78]` | 115 | 3.68 | 2.8 | 0.30 | ✓ |

Vì band luôn nằm trong bệ đích, **cú phóng thanh dẫn không thể trượt**. Đây là lý do thanh dẫn
khác hẳn bệ tăng lực của tài liệu ý tưởng (`Jump Booster`, nơi "vị trí tiếp xúc quyết định quỹ đạo"):
thanh dẫn **không** phụ thuộc vị trí tiếp xúc theo trục ngang, chỉ phụ thuộc hướng vào.

### 7.4 Hai luật ràng buộc vị trí (dễ vi phạm nhất khi dựng)

1. **`|x_rail| ≤ 1.35 m`.** Với `Δx_reach = 3.08 m` và bệ đích rộng 2.8 m, nếu thanh dẫn lệch
   quá 1.35 m khỏi trục thì bệ đích tràn ra ngoài sàn đấu ±6 m. Bản nháp đầu tiên của khu này
   đặt `R2` tại `x = −2.6` → bệ đích rơi vào `[−7.08, −4.28]`, **ngoài sàn**. Validator phải chặn.
2. **Khoảng trống phía trên ≥ 3.2 m** và **phía trước ≥ 4.6 m** tính từ mép gần theo hướng phóng.
   Đỉnh bay là 2.697 m; chạm trần giữa đường thì cú phóng hụt và người chơi rơi — mà rơi thì
   lại chạm chính thanh dẫn, tạo vòng lặp phóng.

---

## 8. Cấu trúc rơi

| Vị trí trượt | Rơi xuống | Độ sâu | Loại |
|---|---|---|---|
| 86–88 (lần đầu gặp bệ điện) | 85 (132.50) | 1.3–4.0 m | **Bệ bắt rơi** |
| 92–95 (nhánh lệch pha) | 91 (140.70) | 1.4–5.7 m | Ngắn |
| 96–97 | 94 (144.95) | 1.5–2.9 m | Bệ bắt rơi |
| 98–100 | 97 (149.30) | 1.5–4.5 m | Ngắn |
| 101–103 | 100 (153.75) | 1.5–4.4 m | Ngắn |
| 104–106 | 103 (158.10) | 1.5–4.5 m | Ngắn |
| 108–109 | 106 → **phóng lại** | — | Vòng, không mất tiến trình |
| 110–113 | 109 (167.80) | 1.5–5.9 m | Ngắn → **trung bình** |
| 116–120 | 115 (177.55) | 1.5–7.5 m | Trung bình |
| 121–124 | 120 (185.05) | 0.9–6.5 m | Trung bình |
| Sai cả tầng 21 | 115 (177.55) | 13.9 m | **Trung bình — 1 lần** |
| Sai cả tầng 20–21 | 108 (166.35) | 25.2 m | **Dài — 1 lần** |

**Ba mái nghỉ thật** — `89` (137.90), `109` (167.80), `125` (192.40) — chia khu thành bốn phần,
nên cú rơi dài nhất bên trong khu là **25.2 m**, tương đương "2 khu" theo kế hoạch §1.5.

Không có sàn riêng ở `y = 131`; miệng khu 3 **mở xuống khu 2**, nên rơi lệch trục sẽ rơi vào
dãy bệ bắt rơi 74–80 của khu 2 (116–126 m). Một lần rơi tệ nhất từ tầng 21 xuống `80 Condenser Deck`
(125.60) là **65.9 m** — đúng mức "dài, chỉ ở đoạn đã cho đủ thông tin".

---

## 9. Đánh số bệ

Số **duy nhất và tăng dần theo độ cao trên toàn tháp** (khu 1 = 01–40, khu 2 = 41–83,
**khu 3 = 84–125**) để thứ tự tuyến leo là duy nhất trên toàn tháp (xem §5 rủi ro #6).

| Hạng mục | Giá trị |
|---|---|
| Bệ khu 3 | 84–125 (42 bệ) |
| Không đánh số | `Insulator Mast`, `Busbar Gantry`, mọi trang trí, tường, trần |
| `summitHeight` khu 3 | **194.0 m** (HUD báo vượt khu) |

> **Đã chốt 20/09/2026:** bỏ hoàn toàn cơ chế coin — không nhóm 5 bệ, không điểm số.
> Số bệ giữ nguyên vì phục vụ thứ tự tuyến leo và validator.

---

## 10. Tiêu chí validator

`SpireChecks` phải xác nhận với khu 3:

- [ ] Mọi liên kết **đường tới hạn** 84→125 tới được bằng mô phỏng vật lý thật.
- [ ] `Δy` mọi liên kết ≤ **2.64 m** (khu 3 dùng tối đa 1.65 m).
- [ ] Mọi liên kết **bệ điện → bệ điện**: `W` (theo §3.2) **≥ 0.30 s**, không nhánh nào rỗng.
- [ ] **Không** cấu hình `Δφ = T/4` nào vi phạm `T/4 ≤ W(Δφ = 0)` (tức là cấm `T/4` trên E-C,
      vì `0.85 > 0.551`).
- [ ] Mọi mạch có `WARN ≥ 0.5 s`.
- [ ] Mọi bệ điện có **collider thường** (`isTrigger == false`) và **không** đổi `enabled`.
- [ ] Không liên kết nào yêu cầu **đứng trên bệ LIVE** hoặc chờ qua một pha LIVE trên bệ nguồn.
- [ ] Mọi thanh dẫn nảy: `|x| ≤ 1.35`, `Δy ≤ 2.45`, band chứa trong bệ đích với biên ≥ 0.30 m,
      khoảng trống trên ≥ 3.2 m, trước ≥ 4.6 m.
- [ ] Mọi thanh dẫn nảy vào được từ **một** phía, và hướng phóng khớp ký hiệu hiển thị.
- [ ] Mọi bệ điện đứng **ngay sau một bệ tĩnh** ở lần xuất hiện đầu tiên của mỗi mạch.
- [ ] Tên mọi bệ bắt đầu bằng số (`^\d+ `) — kiểm tra riêng để không phá thứ tự tuyến leo.
- [ ] 42 bệ đánh số, ranh giới khu = **194.0**.

---

## 11. Thứ tự triển khai đề xuất

| Bước | Việc | Vì sao trước |
|---:|---|---|
| 1 | `ElectricCircuit` + đồng hồ chung, không có gameplay | Mọi thứ khác phụ thuộc nó; pha sai thì mọi cửa sổ vô nghĩa |
| 2 | `ElectricPlatform` với **poll `FixedUpdate`** (§5 rủi ro #1) + hất | Đây là bug im lặng; làm trước khi dựng màn |
| 3 | `LastImpulseTime` dùng chung cho va chạm tường + hất điện (§5 rủi ro #5) | Không có thì phản hồi thị giác không phát |
| 4 | Dựng **tầng 15** (6 bệ, E-A) và playtest tay | Bậc học: nếu người chơi không hiểu "nhìn viền bệ", cả khu hỏng |
| 5 | Mở rộng validator cho `W` theo `Δφ` trước khi dựng tiếp | Khu này có ~30 liên kết phụ thuộc trạng thái |
| 6 | `ElectricBounce` + luật band, dựng tầng 18 | Biến thể chỉ vào sau khi mechanic chính đã đứng vững |
| 7 | Dựng tầng 16–17 | Bậc luyện và kết hợp |
| 8 | Dựng tầng 18–21 | Cao trào + chuyển khu |
| 9 | Đồng bộ dải đèn thành phố ở nền theo `ElectricCircuit` | Đọc trạng thái bằng nền, đúng cảnh chủ đạo của tài liệu ý tưởng |

**Đã xong (bước 1–3).** `ElectricCircuit`, `ElectricPlatform` (poll) và mốc `LastImpulseTime`
đều đã có trong code và biên dịch sạch. Bước 4–9 vẫn còn nguyên.

> Ghi chú kiểm chứng: chạy Play Mode ở chế độ batch (`-executeMethod` + `EnterPlaymode`) đã được
> thử và **không khả thi trên máy này** — Unity mất hơn 5 phút chỉ cho khởi động và retry
> `cdp.cloud.unity3d.com`. Vì vậy check giữ đúng hình dạng menu-driven như 4 check đang có,
> không thêm đường headless chưa kiểm chứng được.
