using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.Rendering;

public class InkExternalFunctions
{
    public void Bind(Story story)
    {
        story.BindExternalFunction("StartQuest", (string questId) => StartQuest(questId));
        story.BindExternalFunction("AdvanceQuest", (string questId) => AdvanceQuest(questId));
        story.BindExternalFunction("FinishQuest", (string questId) => FinishQuest(questId));
        story.BindExternalFunction("AddItem", (string itemId) => AddItem(itemId));
        story.BindExternalFunction("ShowHelpUI", (int index) => ShowHelpUI(index));
        story.BindExternalFunction("CompletedFirstCutscene", () => CompletedFirstCutScene());
        story.BindExternalFunction("CompletedSecondCutscene", () => CompletedSecondCutScene());
        story.BindExternalFunction("CompletedThirdCutscene", () => CompletedThirdCutScene());
        story.BindExternalFunction("CompletedAllCutscene", () => CompletedAllCutScene());
        story.BindExternalFunction("RemoveItem", (string itemName, int amount) => RemoveItem(itemName, amount));
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("StartQuest");
        story.UnbindExternalFunction("AdvanceQuest");
        story.UnbindExternalFunction("FinishQuest");
        story.UnbindExternalFunction("AddItem");
        story.UnbindExternalFunction("ShowHelpUI");
        story.UnbindExternalFunction("CompletedFirstCutscene");
        story.UnbindExternalFunction("CompletedSecondCutscene");
        story.UnbindExternalFunction("CompletedThirdCutscene");
        story.UnbindExternalFunction("CompletedAllCutscene");
        story.UnbindExternalFunction("RemoveItem");
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

    private void ShowHelpUI(int index)
    {
        GameEventsManager.Instance.inputReader.DisableControl();
        GameEventsManager.Instance.uiEvents.OpenHelpUI(index);
    }

    public void CompletedFirstCutScene()
    {
        GameFlowManager.Instance.gameFlowSO.gameFlowData.SetCompletedFirstCutscene(true);
    }

    public void CompletedSecondCutScene()
    {
        GameFlowManager.Instance.gameFlowSO.gameFlowData.SetCompletedSecondCutscene(true);
    }

    public void CompletedThirdCutScene()
    {
        GameFlowManager.Instance.gameFlowSO.gameFlowData.SetCompletedThirdCutscene(true);
    }

    public void CompletedAllCutScene()
    {
        GameFlowManager.Instance.gameFlowSO.gameFlowData.SetCompletedAllCutscene(true);
    }

    public void RemoveItem(string itemName, int amout)
    {
        GameEventsManager.Instance.inventoryEvents.RemoveItem(itemName, amout);
    }
}
