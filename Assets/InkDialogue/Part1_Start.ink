=== start ===
"Chương 1: Chuyến đi theo lời kêu gọi."
//
->DONE
//"Quest đầu tiên đi chào mọi người trong thị trấn 0/5"
=== meet_josh ===
"Tôi có nghe thị trưởng nói sắp tới sẽ có thêm người đến thị trấn để định cư. Hóa ra là cậu Alex, lâu rồi không gặp, cậu nhìn chín chắn hẳn ra nhỉ."
#speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

+ [Hỏi nghi vấn?] 
    "Tôi có quen cậu à?" #speaker:Alex #background: Say #portrait:Alex_Default_1 #layout:Right #audio:animal_crossing_mid
    -> josh_dialogue

+ [Bối rối]
    "Tôi không nhớ cậu lắm, không biết tôi quen cậu như thế nào?" #speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> josh_dialogue


=== josh_dialogue ===
"Đã hơn 10 năm rồi nên chắc cậu quên về người bạn thân hồi nhỏ rồi nhỉ. Tớ là Josh, bạn thân nhất của cậu, ông của cậu đã nhờ tớ viết thư cho cậu đấy." #speaker:Josh #background: Say #portrait:Josh_Default_1 #layout:Left #audio:animal_crossing_mid
"Thôi cậu về đây thì mừng cho ông của cậu rồi. Tớ sẽ hỗ trợ cậu thích nghi với thị trấn nhanh nhất có thể. Nếu có gì không hiểu cứ việc hỏi tớ."
#speaker:Josh #background: Say #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid
 + [Nãy cậu có bảo là "sẽ có thêm người mới chuyển tới", tức là có người chuyển tới trước rồi à?]
    -> about_new_person
 + [Cảm ơn cậu, tớ sẽ hỏi cậu sau.]
 -> DONE
=== about_new_person ===
"À là tuần trước vừa có một cô gái chuyển đến ở. Cô ấy sống ở thành thị hiện đại nhưng cô ấy mong muốn tìm kiếm và giúp đỡ thị trấn phát triển hơn nên cổ đã chọn chuyển đến đây để sống."
#speaker:Josh #background: Say #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid
"Cô ấy có đem theo cả mớ linh kiện và thiết bị máy móc nữa. Cậu có thể gặp cô ta tại nhà ở phía sau thị trấn đấy."
#speaker:Josh #background: Say #portrait:Josh_Default_1 #layout:Left #audio:animal_crossing_mid
->end_josh

=== meet_Tori ===
// Alex đến trước cửa nhà của Tori và gõ
// Tori sẽ không ra khỏi nhà tới khi Alex gõ gửa

"Xin chào, ai ở ngoài đấy vậy, tôi đang bận tay một xíu. Nếu có việc gấp cần gặp mặt thì hãy đá mạnh vào chân cửa."
#speaker:Tori #background: Say #portrait: Tori_Default #layout:Left #audio:animal_crossing_mid
+ [Tôi thử đá vào cánh cửa như lời nói ra.]
-> Kick
+ [Mình sẽ giới thiệu bản thân qua cánh cửa vậy. Mình không muốn làm thiệt hại tài sản của người khác đâu]
-> introduce_door

=== Kick ===
"Wow!!! Đá thật kìa, ha ha ha. Tôi đùa thôi, xin hãy cứ mở cửa đi vào như bình thường, cửa không có khóa đâu."
#speaker:Tori #background: Say #portrait: Tori_Surprise #layout:Left #audio:animal_crossing_mid


-> get_in
=== introduce_door ===
"Tôi là Alex, mới chuyển tới thị trấn nên tôi đang đi chào hỏi mọi người trong thị trấn."
#speaker:Alex #background: Say #portrait: Alex_Default #layout:Left #audio:animal_crossing_mid
"Ồ ra vậy, nãy tôi đùa thôi cửa không khóa đâu, cậu cứ mở cửa vào đi."
#speaker:Tori #background: Say #portrait: Tori_Default #layout:Left #audio:animal_crossing_mid
-> get_in

=== get_in ===
// Alex đi vào bên trong thấy Tori đang đứng đợi sẵn
"Xin chào, tôi sẽ giới thiệu lại, tôi là Alex. Tôi vừa chuyển từ thành phố về quê sống, ở căn nhà ngoài rừng của thị trấn này. Hân hạnh được làm quen."
#speaker:Alex #background: Say #portrait: Alex_Default #layout:Left #audio:animal_crossing_mid
"Tôi là kỹ sư máy móc nên là nếu có việc cần hỗ trợ sửa chữa cứ tìm tới tôi, cùng là người mới nên mong cậu sẽ chiếu cố nhé."
#speaker:Tori #background: Say #portrait: Tori_Smile #layout:Left #audio:animal_crossing_mid
-> DONE

=== meet_lyria ===
"Xin chào, tới là Alex, vừa chuyển từ thành phố về quê sống, ở căn nhà ngoài rừng của thị trấn này. Hân hạnh được làm quen."
#speaker:Alex #background: Say #portrait: Alex_Default #layout:Left #audio:animal_crossing_mid
"Ô xin chào cậu, cậu là cháu của ông Gran nhỉ. Hân hạnh được gặp mặt."
#speaker:Lyria #background: Say #portrait: Lyria_Default #layout:Left #audio:animal_crossing_mid
" Xin tự giới thiệu tớ là chuyên gia về nghiên cứu thảo dược và văn tự cổ đại. Ông của cậu rất hay giảng cho tớ về các văn tự cổ đại đấy."
#speaker:Lyria #background: Say #portrait: Lyria_Smile #layout:Left #audio:animal_crossing_mid

+ [Ông của tôi biết về văn tự cổ đại sao?]
    -> ask_about_scroll

+ [Nghe có vẻ hay nhỉ, tớ sẽ tìm cậu sau nếu tớ cần tìm hiểu thêm về thảo dược.]
-> end_lyria
=== ask_about_scroll ===
"Lúc nhỏ tớ có vô tình tìm được các cuốn sách ghi chép theo một ngôn ngữ kỳ lạ. Dù có đi hỏi mọi người về các cuốn sách ấy thì họ chỉ nói là sách từ mấy đời trước để lại và được bảo tồn trong thư viện."
#speaker:Lyria #background: Say #portrait: Lyria_Default #layout:Left #audio:animal_crossing_mid
"May mắn là ông của cậu, người duy nhất trong thị trấn có kiến thức về các cuốn sách này. Nhưng ông ấy đã lớn tuổi nên cũng không nhớ rõ hết nên tớ chỉ nhờ ông ấy giúp dịch cơ bản các văn tự."
#speaker:Lyria #background: Say #portrait: Lyria_Smile #layout:Left #audio:animal_crossing_mid
"Từ đó tớ có đam mê về nghiên cứu và điều chế thảo dược."
#speaker:Lyria #background: Say #portrait: Lyria_Default #layout:Left #audio:animal_crossing_mid
->end_lyria

=== meet_manu ===
"A, Alex! Em về rồi đấy à. Vậy là ông Gran sẽ không phải bận tâm về khu vườn của mình nữa rồi."
#speaker:Manu #background: Say #portrait: Manu_Smile #layout:Left #audio:animal_crossing_mid
+[Vâng, chào chị. Chị có vẻ thân với ông của em lắm ạ?]
-> manu_dialogue
+[ Cảm ơn chị đã quan tâm đến ông của em.]
->manu_dialogue 

=== manu_dialogue ===
"Ông của em là người được cả thị trấn kính trọng mà. Người dân ai cũng quý mến ông ấy cả. Nhất là mấy năm gần đây sức khỏe của ông ấy đang yếu đi."
#speaker:Manu #background: Say #portrait: Manu_Default_1 #layout:Left #audio:animal_crossing_mid
"Chị thì thỉnh thoảng vẫn hay qua phụ ông ấy dọn dẹp nhà cửa. Mà thôi giờ cứ để ông ấy an tâm đi chữa bệnh là mọi người mừng rồi. Vậy nên việc nhà cửa, nhờ em trông coi chăm sóc nhé. Chị cũng trở lại công việc của chị đây."
#speaker:Manu #background: Say #portrait: Manu_Defualt #layout:Left #audio:animal_crossing_mid
"Vâng ạ."
#speaker:Alex #background: Say #portrait: Alex_Smile #layout:Left #audio:animal_crossing_mid
->DONE
=== meet_bill ===
"Ô hô, Chào cậu bé. Cháu là người nối dõi của ông Gran đúng chứ? Đã lớn đến chừng này rồi à. Ta cũng không ngờ đấy, thoáng cái -....."
#speaker:Bill #background: Say #portrait: Bill_Smile #layout:Left #audio:animal_crossing_mid
+[Tiếp tục lắng nghe]
->bill_dialogue
+["Vâng chào chú, cháu là Alex mới chuyển tới ạ"]

->bill_dialogue_1
=== bill_dialogue ===
"Đã hơn 10 năm rồi nhỉ, thằng nhóc ngày nào còn theo chân Gran ra vườn giờ đã là cậu nhóc chững chạc rồi."
#speaker:Bill #background: Say #portrait: Bill_Smile #layout:Left #audio:animal_crossing_mid
"Thời gian đúng là nhanh thật, trong khi ta cứ quanh quẩn ở trong thị trấn để rèn thì mọi thứ xung quanh đã thay đổi rồi."
#speaker:Bill #background: Say #portrait: Bill_Default #layout:Left #audio:animal_crossing_mid
"Rồi thị trấn này sẽ mất đi một người đàn ông vĩ đại, nhưng ta hy vọng rằng cháu sẽ có thể vượt qua được sự vĩ đại ấy. Ha Ha Ha."
#speaker:Bill #background: Say #portrait: Bill_Smile #layout:Left #audio:animal_crossing_mid
"Thôi nhóc về nghỉ ngơi đi ta sẽ luôn tiếp chuyện với nhóc ở đây."
#speaker:Bill #background: Say #portrait: Bill_Default #layout:Left #audio:animal_crossing_mid
->DONE
=== bill_dialogue_1 ===
"Hừm, nhóc nghĩ ta không biết nhóc là ai à. Và nhóc nên lắng nghe người ta nói khi họ đang giữa cuộc trò chuyện chứ."
#speaker:Bill #background: Say #portrait: Bill_Angry #layout:Left #audio:animal_crossing_mid
"Có vẻ nhóc đã thay đổi từ khi lên thành phố rồi nhỉ. Thôi thì nhóc về đây là được rồi.
#speaker:Bill #background: Say #portrait: Bill_Default_1 #layout:Left #audio:animal_crossing_mid
"Về nghỉ ngơi đi ta sẽ luôn tiếp chuyện với nhóc ở đây."
#speaker:Bill #background: Say #portrait: Bill_Default_1 #layout:Left #audio:animal_crossing_mid
->DONE
=== end_josh ===
"Thôi gặp lại cậu sau, tớ đi làm việc nhé."
#speaker:Josh #background: Say #portrait: Josh_Default #layout:Left #audio:animal_crossing_mid
-> DONE
=== end_lyria ===
"Từ giờ mong cậu giúp đỡ nhé. Hy vọng cậu sẽ giúp tớ trong việc nghiên cứu về các loại thực vật và văn tự cổ."
#speaker:Lyria #background: Say #portrait: Lyria_Smile #layout:Left #audio:animal_crossing_mid
"Vâng, mong cậu cũng sẽ chia sẻ kiến thức thêm cho mình."
#speaker:Alex #background: Say #portrait: Alex_Default #layout:Left #audio:animal_crossing_mid
-> DONE
