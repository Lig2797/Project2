using System;
using UnityEngine;

public class StartGameEvents
{
    public event Action OnNewGame;
    public void OnNewGameButtonClicked()
    {
        OnNewGame?.Invoke();
    }

    public event Action<string> OnLoadGame;
    public void OnLoadGameButtonClicked(string saveFileName)
    {
        OnLoadGame?.Invoke(saveFileName);
    }
}
