using UnityEngine;

public class UIWorkbenchPanel : Singleton<UIWorkbenchPanel>
{
    [SerializeField] private GameObject craftingPanel;

    public void OpenCraftingPanel()
    {
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Crafting panel is not assigned.");
        }
    }
}
