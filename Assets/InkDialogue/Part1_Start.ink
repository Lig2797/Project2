=== start ===

Haizzz, lại một ngày làm việc bận rộn khác #speaker:Alex #background: Think #portrait:Alex_Think #layout:Right #audio:animal_crossing_mid

Ngày nào mình cũng phải đối mặt với hàng đống công việc kể từ khi mình sống một mình hơn 2 năm nay. Các ca làm thêm giờ đang làm kiệt quệ mình. Đôi lúc mình tự hỏi liệu mục đích sống của mình là gì? #speaker:Alex #background: Think #portrait:Alex_Think #layout:Right #audio:animal_crossing_mid

"~~~ Ping pong" #speaker:??? #background: Say #portrait:Alex_Surprise #layout:Left #audio:animal_crossing_mid

"Giao thư đây!!!" #speaker:??? #background: Say #portrait:Alex_Surprise #layout:Left #audio:animal_crossing_mid

Hửm? Lạ nhỉ, ai lại đi gửi thư vào thời này chứ? #speaker:Alex #background: Think #portrait:Alex_Surprise2 #layout:Right #audio:animal_crossing_mid

Nếu là thư của bố mẹ thì chắc chắn họ đã liên lạc trước với mình rồi qua điện thoại rồi. Thôi để ra xem thử. #speaker:Alex #background: Think #portrait:Alex_Surprise2 #layout:Right #audio:animal_crossing_mid

-> DONE

=== grandpa_letter ===

~ AddItem("Letter")
Ô! Là thư của ông nội gửi, để đọc thử nào. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
~ AddItem("Key")
Bên trong còn có thêm 1 chiếc chìa khóa. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
~ AddItem("Axe")
~ AddItem("Hoe")
~ AddItem("WaterCan")
~ AddItem("Potato Seed")
~ AddItem("Strawberry Seed")
~ AddItem("Sword")
Còn có cả những đồ nghề làm vườn nữa. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> read_letter

=== read_letter ===

+ [Đọc thư kỹ càng]
    Bức thư có vẻ quan trọng mình cần phải đọc kỹ càng mới được. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

    -> DONE
    
+ [Bỏ qua thư]
    Thôi để lát nữa mình sẽ đọc vậy. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> DONE

=== prepare_departure ===

~ RemoveItem("Letter", 1)
Mình cũng rất nhớ ông. Có vẻ cơ hội để mình thay đổi bản thân đã đến rồi. Một cuộc sống ở thôn quê có vẻ sẽ hợp với mình hơn thành thị xô bồ. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

Quyết định đã được đưa ra, mình cần chuẩn bị lên đường đến làng Eloria. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

+ [Chuẩn bị hành lý kỹ càng] 
~ CompletedFirstCutscene()
    Mình cần thu dọn đồ đạc và sắp xếp những vật dụng cần thiết. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    
    -> DONE

+ [Đi ngay lập tức, không chờ đợi]
~ CompletedFirstCutscene()
    Không chần chừ thêm nữa, lên đường thôi. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

    -> DONE

=== to_eloria ===
~ CompletedSecondCutscene()
Thật là một chuyến đi dài mệt mỏi, phải nhanh chóng về nhà nghỉ ngơi mới được.
#speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> DONE

=== read_instructions ===

Theo như bảng thì bên phải là làng Eloria, vậy bên trái chắc chắn là nhà ông nội rồi.
#speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> DONE
    
=== eloria_village ===

Ồ! Cảnh vật quê đã thay đổi mấy so với trước kia nhỉ. Mình tự hỏi đã bao lâu rồi kể từ khi gia đình mình rời quê lên thành phố nhỉ? #speaker:Alex #background: Think #portrait:Alex_Surprise #layout:Right #audio:animal_crossing_mid

Có 2 chiếc chìa khóa, có vẻ chiếc chìa nhỏ này dùng để mở cửa ngôi nhà rồi. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> DONE

=== House_1 ===
~ RemoveItem("Key", 1)
Ngôi nhà này trông thật cũ kĩ.
#speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
Mình quá mệt để dọn dẹp rồi, làm một giấc rồi tính tiếp vậy.
#speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
-> DONE

=== noises_1 ===

Tiếng động gì mà ồn vậy ta. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
~ CompletedThirdCutscene()
Phải ra ngoài xem thử mới được. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> DONE

=== noise_2 ===

Haizzz~~~ Tại sao Lyria lại cần nhiều gỗ đến như vậy chứ. Thật là....
#speaker:??? #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid

Giọng nói phát ra từ đằng sau nhà mình nên ra xem thử thế nào.
#speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> DONE

=== meet_josh ===
Này cậu gì ơi. #speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
Khoan đã là Josh đây mà. #speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
Ô! Alex? #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
Cậu về lúc nào mà không nói cho tớ biết. #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
Tớ mới về cách đây không lâu. Cậu dạo này thế nào. #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
Cậu nhìn mà không biết sao? Cầm lấy cây rìu này và phụ tớ chặt gỗ mau.#speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
~ AddItem("Axe")
Này tớ vừa mới về thôi mà Josh #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
Josh!!! #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
Chờ tớ với Josh. #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
-> DONE
