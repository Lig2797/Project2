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
        story.BindExternalFunction("AddItem", (string itemId) => AddItem(itemId));
        story.BindExternalFunction("LoadScene", (string sceneName) => LoadScene(sceneName));
        story.BindExternalFunction("ShowHelpUI", (int index) => ShowHelpUI(index));
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("StartQuest");
        story.UnbindExternalFunction("AdvanceQuest");
        story.UnbindExternalFunction("FinishQuest");
        story.UnbindExternalFunction("AddItem");
        story.UnbindExternalFunction("LoadScene");
        story.UnbindExternalFunction("ShowHelpUI");
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

    private void AddItem(string itemId)
    {
        GameEventsManager.Instance.inventoryEvents.AddItem(itemId);
    }

    private void LoadScene(string sceneName)
    {
        Loader.Scene scene = ConvertToScene(sceneName);
        DialogueManager.Instance.ExitDialogueAndLoadScene(scene);
    }

    private void ShowHelpUI(int index)
    {
        GameEventsManager.Instance.inputReader.DisableControl();
        GameEventsManager.Instance.uiEvents.OpenHelpUI(index);
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
            return Loader.Scene.WorldScene; // Default value
        }
    }
}
