=== start ===

// Alex nhận được thư của ông nội
"Ông nội đã gửi cho tôi một lá thư cùng chiếc chìa khóa cũ. Tôi sẽ làm gì bây giờ?" #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

+ [Đọc lại thư kỹ càng] 
    "Tôi đọc lại thư và cảm thấy quyết tâm trở về ngôi làng." #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> prepare_departure

+ [Bỏ qua thư, chưa chắc chắn]
    "Tôi chưa chắc mình có nên về không, nhưng có cảm giác không thể từ chối." #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> prepare_departure

=== prepare_departure ===
"Quyết định đã được đưa ra, tôi chuẩn bị lên đường đến làng Eloria." #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

+ [Chuẩn bị hành lý kỹ càng] 
    "Tôi thu dọn đồ đạc và sắp xếp những vật dụng cần thiết." #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> arrival

+ [Đi ngay lập tức, không chờ đợi]
    "Tôi không chần chừ thêm nữa, lên đường ngay." #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> arrival

=== arrival ===
"Tôi đã đến làng Eloria. Không khí nơi đây thật trong lành và thân thiện." #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

+ [Đi tìm Josh - bạn thân lâu năm]
    -> meet_josh

+ [Khám phá làng một mình trước]
    -> explore_village

=== meet_josh ===
Alex! Cậu đến rồi à? Thật vui khi gặp lại cậu. #speaker:Josh #background: Say #portrait:Josh_Smile #layout:Left #audio:animal_crossing_mid
+ [Chào Josh, rất vui được gặp lại cậu] 
    "Tôi mỉm cười và nói với Josh rằng tôi rất vui." #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> josh_dialogue

+ [Hỏi Josh về cuộc sống ở làng] 
    "Tôi hỏi Josh cuộc sống ở đây thế nào." #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> josh_dialogue

=== josh_dialogue ===
Làng vẫn thế, nhưng có nhiều điều kỳ lạ đang xảy ra. Tao sẽ giúp mày thích nghi. #speaker:Josh #background: Say #portrait:Josh_Smile #layout:Left #audio:animal_crossing_mid
+ [Cảm ơn Josh, tôi sẽ cố gắng thích nghi nhanh] 
    "Tôi biết ơn sự giúp đỡ của Josh." #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> end_part1

+ [Tôi còn bỡ ngỡ, cần thời gian] 
    "Tôi hơi lo lắng, nhưng sẽ cố gắng từng bước." #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> end_part1

=== explore_village ===
"Tôi đi dạo quanh làng, quan sát mọi thứ xung quanh."
+ [Gặp Lyria - cô gái mê thảo dược] 
    -> meet_lyria

+ [Quay lại tìm Josh] 
    -> meet_josh

=== meet_lyria ===
Lyria: "Chào Alex, tôi nghe ông nội bạn là một người đặc biệt."
+ [Hỏi Lyria về phép màu của làng]
    "Tôi tò mò về những điều kỳ diệu ở đây."
    -> lyria_magic

+ [Nói lời chào và rút lui nhẹ nhàng]
    "Tôi cảm thấy có nhiều việc cần làm nên tạm biệt Lyria."
    -> explore_village

=== lyria_magic ===
Lyria: "Làng này là một Tâm Linh Địa, có nhiều năng lượng kỳ diệu. Nhưng bóng tối cũng đang rình rập."
+ [Hỏi thêm về bóng tối]
    -> dark_warning

+ [Cảm ơn và hứa sẽ cẩn thận]
    -> explore_village

=== dark_warning ===
Lyria: "Vùng Đất Bóng Tối đang dần thức tỉnh, cần cảnh giác vào ban đêm."
+ [Đồng ý và hứa sẽ giúp bảo vệ làng]
    "Tôi quyết tâm bảo vệ nơi đây."
    -> end_part1

+ [Tôi sẽ tìm hiểu thêm một mình]
    -> explore_village

=== end_part1 ===
"Tôi đã có những bước đầu tiên trong cuộc hành trình ở Eloria."
-> DONE
