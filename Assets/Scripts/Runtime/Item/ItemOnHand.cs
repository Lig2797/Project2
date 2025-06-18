using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class ItemOnHand : MonoBehaviour
{
    public void ActivateItemOnHand(Sprite image, bool isActivate)
    {
        if (isActivate)
        {
            SetItemSprite(image);
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
