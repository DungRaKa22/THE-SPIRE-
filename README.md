# JumpDummy — Jump Lab

**Màn leo cao mới:** mở **JumpDummy → Open Neon Ascent** rồi Play. Màn có 35 bệ điều hòa, bảng LED neon và ống khói, chia thành 5 khu cao khoảng 65 mét. Mở màn cũ bằng **JumpDummy → Open Cyberpunk Rooftops**. Xem [hướng dẫn asset và animation](Docs/CyberpunkArt.md).

Prototype đầu tiên: Unity 6000.5.7f1, 2D, bàn phím.

## Chơi thử

1. Đợi Unity biên dịch script.
2. Chọn **JumpDummy > Open Jump Lab** trên thanh menu. Scene được tạo tự động khi import lần đầu nếu chưa tồn tại.
3. Nhấn **Play**, click vào cửa sổ Game để nhận bàn phím.

- **A/D hoặc ←/→**: đi, chọn hướng khi tích lực.
- **Giữ Space**: tích lực, tối đa 0,85 giây. Nhân vật dừng đi khi tích lực.
- **Thả Space**: nhảy. Giữ hướng ở thời điểm thả; không giữ hướng thì nhảy thẳng đứng.
- Không đổi hướng trên không. Chạm tường khi đang bay làm bật ngược, chạm trần ngắt đà lên.
- **R**: trở lại điểm bắt đầu, đặt lại số lần nhảy.

Màn Cyberpunk có vòng chơi hoàn chỉnh: Enter/Space bắt đầu hoặc tiếp tục, N tạo lượt mới, Esc/P tạm dừng. Game tự lưu thời gian, số lần nhảy và vị trí an toàn cao nhất; rơi khỏi màn sẽ trở lại vị trí đó. Đạt beacon trên mái nhà để hoàn thành và lưu thời gian tốt nhất.

Coin: mỗi nhóm đủ 5 bục theo số thứ tự tên bục chọn ngẫu nhiên 1 bục để sinh 1 coin vàng, ở vị trí ngẫu nhiên trên mặt bục. Chạm coin cộng 10 điểm, hiển thị ở SCORE. Tường, sàn và trần không được tính là bục. Nhóm cuối chưa đủ 5 bục không sinh coin (màn 8 bục hiện tại có 1 coin). Lượt mới đổi vị trí và đặt điểm về 0; tiếp tục lượt cũ giữ vị trí coin, điểm và trạng thái đã nhặt. Rơi xuống không làm coin xuất hiện lại.

Neon Ascent có 7 coin mỗi lượt (tối đa 70 điểm) và lưu tiến trình riêng với màn cũ. Đường leo lấy cảm hứng bố cục 5 ảnh tham khảo: khối kiến trúc hai bên, bệ lệch trái/phải, mái nghỉ rộng và bệ hẹp đan xen. Hình ảnh và địa hình được dựng mới theo phong cách cyberpunk. Camera tiếp tục bám độ cao; các khu đổi từ xanh tối ở hẻm sang tím sáng ở đỉnh.

Chỉnh trực tiếp các đối tượng trong **Neon Ascent Platforms** ở scene `Assets/Scenes/NeonAscent.unity`. Ba prefab bệ tái sử dụng nằm trong `Assets/Prefabs/Cyberpunk/ClimbProps`; ảnh gốc và thiết lập import nằm trong `Assets/Art/Cyberpunk/ClimbProps`. Số đầu tên bệ xác định thứ tự chia nhóm coin. `JumpDummy → Build Windows Playtest` ưu tiên màn Neon Ascent.

## Chỉnh cảm giác nhảy

Chọn **Dummy** trong scene, chỉnh component **Dummy Controller**: Walk Speed, Charge Duration, Minimum/Maximum Jump Speed, Horizontal Jump Speed, Wall Bounce Retention và Gravity Scale. Thay đổi trong Play Mode không được giữ sau khi dừng.

Scene có 8 bục, hai tường và một trần thấp. Camera theo chiều cao, thanh trên đầu hiển thị lực tích. Đây là phòng thử cơ chế, chưa có lưu game, menu, checkpoint hoặc kết thúc màn.

## Kiểm tra thủ công

- So sánh nhấn nhanh và giữ Space hơn một giây.
- Đứng yên nhảy thẳng, sau đó nhảy sang trái/phải.
- Khi đang bay, thử đổi hướng và nhấn Space lại: quỹ đạo không đổi.
- Nhảy vào tường, trần thấp bên phải và mép bục; kiểm tra không kẹt hoặc xuyên collider.
- Rơi từ tầng trên xuống, đáp lại và nhảy tiếp.
- Thử Game view 16:9, 30/60/120 FPS trước khi cân bằng màn chính.

Batch smoke check: `-executeMethod JumpDummy.Editor.JumpDummyPrototypeChecks.ValidateBatch` sau khi scene đã được tạo. Kiểm tra này xác nhận logic tích lực/đầu vào với collider thật; vẫn cần chơi thử va chạm và cảm giác điều khiển.
