using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class InventoryController : NetworkBehaviour, IDataPersistence
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;

    // GameEvent
    [SerializeField] private GameEvent onOpenInventory;
    [SerializeField] private GameEvent onCloseInventory;
    [SerializeField] private GameEvent onChangeSelectedSlot;
    [SerializeField] private GameEvent onInventoryLoad;

    private void Start()
    {
        if (!IsOwner) enabled = false;
    }

    private void OpenInventory()
    {
        onOpenInventory.Raise(this, ActionMap.UI);
    }
    private void CloseInventory()
    {
        onCloseInventory.Raise(this, ActionMap.Player);
    }

    public void SwitchActionMap(Component sender, object data)
    {
        ActionMap map = (ActionMap)data;
        _inputReader.SwitchActionMap(map);
        if(map == ActionMap.UI)
        {
            _inventoryManagerSO.isOpeningInventory = true;
        }
        else
        {
            _inventoryManagerSO.isOpeningInventory = false;
        }
    }
    private void GetInputValueToChangeSlot(int value, bool isKeyboard)
    {

        if (isKeyboard)
        {

            if (value != _inventoryManagerSO.selectedSlot)
            {
                onChangeSelectedSlot.Raise(this, value); // gui duy nhat newValue thoi
                
            }
            
        }
        else
        {
            int newValue = _inventoryManagerSO.selectedSlot + value;
            if (newValue > 8) newValue = 0;
            else if (newValue < 0) newValue = 8;
            
            onChangeSelectedSlot.Raise(this, newValue);
        }
    }

    public void StartToLoad(GameData gameData)
    {
        
    }

    public void StartToSave(ref GameData gameData)
    {
       
    }

    public void LoadData(GameData data)
    {
        _inputReader.playerActions.changeInventorySlotEvent += GetInputValueToChangeSlot;
        _inputReader.playerActions.openInventoryEvent += OpenInventory;
        _inputReader.uiActions.closeInventoryEvent += CloseInventory;

        _inventoryManagerSO.inventory = data.InventoryData;
        ItemDatabase.Instance.SetItem(_inventoryManagerSO.inventory.InventoryItemList);
        onInventoryLoad.Raise(this, null);
        _inventoryManagerSO.RefreshCurrentHoldingItem();
        onChangeSelectedSlot.Raise(this, _inventoryManagerSO.selectedSlot);
    }   
    
    public void SaveData(ref GameData gameData)
    {
        gameData.SetInventoryData(_inventoryManagerSO.inventory);

        _inputReader.playerActions.changeInventorySlotEvent -= GetInputValueToChangeSlot;
        _inputReader.playerActions.openInventoryEvent -= OpenInventory;
        _inputReader.uiActions.closeInventoryEvent -= CloseInventory;
    }
}
