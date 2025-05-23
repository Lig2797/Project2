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
Come back once you've leveled up a bit more.  #speaker:Josh #portrait:Josh_Default #layout:Right #audio:animal_crossing_mid
-> END

= canStart
Will you collect 5 coins and bring them to my friend over there? #speaker:Josh #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid
* [Yes]
    ~ StartQuest(CollectCoinsQuestId)
    Great! #speaker:Josh #portrait:Josh_Smile #layout:Right #audio:animal_crossing_mid
* [No]
    Oh, ok then. Come back if you change your mind. #speaker:Josh #portrait:Josh_Sad #layout:Left #audio:animal_crossing_mid
- -> END

= inProgress
How is collecting those coins going? #speaker:Josh #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid
-> END

= canFinish
Oh? You collected the coins? Go give them to my friend over there and he'll give you a reward! #speaker:Josh #portrait:Josh_Smile #layout:Left #audio:animal_crossing_mid
-> END

= finished
Thanks for collecting those coins! #speaker:Josh #portrait:Josh_Smile #layout:Left #audio:animal_crossing_mid
-> END