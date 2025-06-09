using UnityEngine;
using UnityEngine.UI;

public class UIWorkbenchPanel : Singleton<UIWorkbenchPanel>
{
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
    }

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

    public void Close()
    {
        craftingPanel.SetActive(false);
        GameEventsManager.Instance.inputReader.SwitchActionMap(ActionMap.Player);
    }    
}
