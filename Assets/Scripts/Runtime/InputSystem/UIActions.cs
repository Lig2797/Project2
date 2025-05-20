using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIActions : Controls.IUIActions
{
    public Action closeInventoryEvent;
    public Action continueDialogueEvent;
    public Action submitEvent;

    public void OnCloseInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) closeInventoryEvent?.Invoke();
    }

    public void OnContinueDialogue(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) continueDialogueEvent?.Invoke();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) submitEvent?.Invoke();
    }
}
