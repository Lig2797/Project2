using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class InkExternalFunctions
{
    public void Bind(Story story)
    {
        story.BindExternalFunction("StartQuest", (string questId) => StartQuest(questId));
        story.BindExternalFunction("AdvanceQuest", (string questId) => AdvanceQuest(questId));
        story.BindExternalFunction("FinishQuest", (string questId) => FinishQuest(questId));
        story.BindExternalFunction("Load", (string sceneName) => Load(sceneName));
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("StartQuest");
        story.UnbindExternalFunction("AdvanceQuest");
        story.UnbindExternalFunction("FinishQuest");
    }

    private void StartQuest(string questId)
    {
        GameEventsManager.Instance.questEvents.StartQuest(questId);
    }

    private void AdvanceQuest(string questId)
    {
        GameEventsManager.Instance.questEvents.AdvanceQuest(questId);
    }

    private void FinishQuest(string questId)
    {
        GameEventsManager.Instance.questEvents.FinishQuest(questId);
    }

    private void Load(string sceneName)
    {
        Loader.Scene scene = ConvertToScene(sceneName);
        Loader.Load(scene);
    }

    private Loader.Scene ConvertToScene(string sceneName)
    {
        if (System.Enum.TryParse(sceneName, out Loader.Scene scene))
        {
            return scene;
        }
        else
        {
            Debug.LogError($"Invalid scene name: {sceneName}");
            return Loader.Scene.CharacterSelectScene; // Default value
        }
    }
}
