using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UISaveFilePanelController : MonoBehaviour
{
    private VisualElement _saveFilePanel;
    private ListView _saveFileList;
    private Button _newGameButton;
    private Button _loadGameButton;
    private Button _saveFileBackButton;

    public VisualTreeAsset _saveFileData;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _saveFilePanel = root.Q<VisualElement>("SaveFilePanel");
        _saveFileList = root.Q<ListView>("SaveFileList");
        _newGameButton = root.Q<Button>("NewGameButton");
        _loadGameButton = root.Q<Button>("LoadGameButton");
        _saveFileBackButton = root.Q<Button>("SaveFileBackButton");
    }

    private void Start()
    {
        _loadGameButton.SetEnabled(false);

        PopulateSaveFiles();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.activeUIPanelEvents.onActiveSingleplayer += OnActive;
        GameEventsManager.Instance.activeUIPanelEvents.onDisActivateSingleplayer += OnDisableActive;
        _newGameButton.clicked += OnNewGameButtonClicked;
        _loadGameButton.clicked += OnLoadGameButtonClicked;
        _saveFileBackButton.clicked += OnBackButtonClicked;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.activeUIPanelEvents.onActiveSingleplayer -= OnActive;
        GameEventsManager.Instance.activeUIPanelEvents.onDisActivateSingleplayer -= OnDisableActive;
        _newGameButton.clicked -= OnNewGameButtonClicked;
        _loadGameButton.clicked -= OnLoadGameButtonClicked;
        _saveFileBackButton.clicked -= OnBackButtonClicked;
    }

    private void OnNewGameButtonClicked()
    {
        _saveFilePanel.style.display = DisplayStyle.None;
        GameEventsManager.Instance.startGameEvents.OnNewGameButtonClicked();
        SceneManager.LoadScene("CutScene");
    }

    private void OnLoadGameButtonClicked()
    {
        var selected = _saveFileList.selectedItem as string;
        if (!string.IsNullOrEmpty(selected))
        {
            OnSaveFileSelected(_saveFileList.selectedItems);
            Debug.Log("Load game: " + selected);
            _saveFilePanel.style.display = DisplayStyle.None;
            GameEventsManager.Instance.startGameEvents.OnLoadGameButtonClicked(selected);
        }
    }

    private void OnBackButtonClicked()
    {
        _saveFilePanel.RemoveFromClassList("savefile-panel-moveleft");
        GameEventsManager.Instance.activeUIPanelEvents.OnActiveMainMenu();
    }

    private void OnSaveFileSelected(IEnumerable<object> selectedItems)
    {
        _loadGameButton.SetEnabled(selectedItems != null && selectedItems.Any());
    }

    private void PopulateSaveFiles()
    {
        var saveNames = new List<string> { "Save 1", "Save 2", "Save 3" };

        _saveFileList.itemsSource = saveNames;
        _saveFileList.makeItem = () => new VisualElement();
        _saveFileList.bindItem = (element, index) =>
        {
            var saveName = saveNames[index];
            element.Clear();
            _saveFileData.CloneTree(element);
            element.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            element.style.height = new StyleLength(new Length(400, LengthUnit.Pixel));
            element.Q<VisualElement>("FileImage").style.backgroundImage = new StyleBackground(new Texture2D(100, 100));
            element.Q<Label>("FileName").text = saveName;
            element.Q<Label>("FileDate").text = "2023-10-01";
        };
        _saveFileList.selectionType = SelectionType.Single;
    }    

    private void OnActive()
    {
        _saveFilePanel.AddToClassList("savefile-panel-moveleft");
    }

    private void OnDisableActive()
    {
        _saveFilePanel.RemoveFromClassList("savefile-panel-moveleft");
    }
}
