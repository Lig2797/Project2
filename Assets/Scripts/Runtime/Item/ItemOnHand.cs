using Unity.Netcode;
using UnityEngine;

public class ItemOnHand : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void ActivateItemOnHandServerRpc(string itemName, bool isActivate)
    {
        ActivateItemOnHandClientRpc(itemName, isActivate);
    }

    [ClientRpc]
    public void ActivateItemOnHandClientRpc(string itemName, bool isActivate)
    {
        if (isActivate)
        {
            Debug.Log("Activate item on hand: " + itemName);
            Item item = ItemDatabase.Instance.GetItemByName(itemName);
            SetItemSprite(item.image);
        }
        else
        {
            Debug.Log("Deactivate item on hand: " + itemName);
            SetItemSprite(null);
        }

    }
    public void SetItemSprite(Sprite image)
    {
        transform.GetComponent<SpriteRenderer>().sprite = image;
    }


}
