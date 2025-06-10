=== collectCoinsFinish ===
{ CollectCoinsQuestState:
    - "FINISHED": -> finished
    - else: -> default
}

= finished
Ô! Alex cậu về lúc nào vậy? #speaker:Lyria #portrait:Lyria_Smile #layout:Left #audio:animal_crossing_mid

Tớ vừa mới về. Josh bảo tớ mang đống gỗ này đến chỗ cậu #speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

~ RemoveItem("Wood", 5)

Ôi, quý quá cảm ơn cậu nhé. #speaker:Lyria #portrait:Lyria_Smile #layout:Left #audio:animal_crossing_mid

Tớ có vài món đồ làm vườn mà ông cậu nhờ tớ gửi giúp. #speaker:Lyria #portrait:Lyria_Smile #layout:Left #audio:animal_crossing_mid

~ AddItem("Strawberry Seed")
~ AddItem("Potato Seed")
~ AddItem("Carrot Seed")
~ AddItem("WaterCan")
~ AddItem("Hoe")

Là hạt giống, cuốc và bình tưới nước. Hay quá bây giờ đó có thể đi làm vườn rồi.#speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
-> END

= default
Chào Alex! Lâu lắm mới gặp cậu. Câu có vấn đề gì không. #speaker:Lyria #portrait:Lyria_Smile #layout:Left #audio:animal_crossing_mid 

* [Không có gì cả.]
    -> END
* { CollectCoinsQuestState == "CAN_FINISH" } [Đây là một ít vật phẩm.]
    ~ FinishQuest(CollectCoinsQuestId)
    Oh? These coins are for me? Thank you!
-> END