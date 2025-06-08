using System;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance { get; private set; }

    public PlayerEvents playerEvents;
    public GoldEvents goldEvents;
    public MiscEvents miscEvents;
    public QuestEvents questEvents;
    public DialogueEvents dialogueEvents;
    public ActiveUIPanelEvents activeUIPanelEvents;
    public OptionsEvents optionsEvents;
    public DataEvents dataEvents;
    public GameFlowEvents gameFlowEvents;
    public NetworkEvents networkEvents;
    public CutsceneEvents cutsceneEvents;
    public NpcEvents npcEvents;
    public DateTimeEvents dateTimeEvents;
    public NetworkObjectEvents networkObjectEvents;

    public InputReader inputReader;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // initialize all events
        playerEvents = new PlayerEvents();
        goldEvents = new GoldEvents();
        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
        dialogueEvents = new DialogueEvents();
        activeUIPanelEvents = new ActiveUIPanelEvents();
        optionsEvents = new OptionsEvents();
        dataEvents = new DataEvents();
        gameFlowEvents = new GameFlowEvents();
        networkEvents = new NetworkEvents();
        cutsceneEvents = new CutsceneEvents();
        npcEvents = new NpcEvents();
        dateTimeEvents = new DateTimeEvents();
        networkObjectEvents = new NetworkObjectEvents();
    }
}
