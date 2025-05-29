using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DoorTrigger : MonoBehaviour
{
    public SpriteRenderer doorSprite;
    public Sprite openDoorSprite;
    public Sprite closedDoorSprite;

    private BoxCollider2D boxCollider;

    private bool isOpen = false;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isOpen)
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        doorSprite.sprite = openDoorSprite;
        isOpen = true;
    }

    private void CloseDoor()
    {
        doorSprite.sprite = closedDoorSprite;
        isOpen = false;
    }
}
