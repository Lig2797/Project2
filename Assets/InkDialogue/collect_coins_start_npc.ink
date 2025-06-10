=== collectCoinsStart ===
{ CollectCoinsQuestState :
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> END
}

= requirementsNotMet
// not possible for this quest, but putting something here anyways
Quay lại đây nói chuyện với tớ dến khi nào cậu làm xong việc.  #speaker:Josh #portrait:Josh_Default #layout:Right #audio:animal_crossing_mid
-> END

= canStart
Này cậu gì ơi. #speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
Khoan đã là Josh đây mà. #speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
Ô! Alex? #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
Cậu về lúc nào mà không nói cho tớ biết. #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
Tớ mới về cách đây không lâu. Cậu dạo này thế nào. #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
Cậu nhìn mà không biết sao? Cầm lấy cây rìu này và phụ tớ chặt gỗ mau.#speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid

~ AddItem("Axe")

Này tớ vừa mới về thôi mà Josh #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
Josh!!! #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid

 ~ StartQuest(CollectCoinsQuestId)
 
Chờ tớ với Josh. #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
- -> END

= inProgress
Cậu làm đến đâu rồi? #speaker:Josh #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid
-> END

= canFinish
Ồ? Cậu làm xong rồi đấy à nhanh hơn tôi tưởng đấy. #speaker:Josh #portrait:Josh_Smile #layout:Left #audio:animal_crossing_mid
Mau mang chúng đến chỗ Lyria đi có thể cô ấy sẽ gửi cậu vài món quà. #speaker:Josh #portrait:Josh_Smile #layout:Left #audio:animal_crossing_mid
-> END

= finished
Xong rồi ư. Giờ cậu hãy giao chúng đến Lyria. Đi sang phải vào làm sẽ gặp. #speaker:Josh #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid

-> END