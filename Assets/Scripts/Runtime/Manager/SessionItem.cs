using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.Multiplayer.Widgets;
using UnityEngine.UI;

public class SessionItem : Selectable, ISelectHandler
{
    ISessionInfo m_SessionInfo;

    public UnityEvent<ISessionInfo> OnSessionSelected;

    public void OnSelect(BaseEventData eventData)
    {
        OnSessionSelected?.Invoke(m_SessionInfo);
    }
}
