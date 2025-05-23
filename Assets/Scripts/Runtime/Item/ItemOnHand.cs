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
            Item item = ItemDatabase.Instance.GetItemByName(itemName);
            SetItemSprite(item.image);
        }
        else
        {
            SetItemSprite(null);
        }

    }
    public void SetItemSprite(Sprite image)
    {
        transform.GetComponent<SpriteRenderer>().sprite = image;
    }


}
