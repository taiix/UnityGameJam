using UnityEngine;
public enum BookType
{
    Sun,
    Moon,
    Infinity,
    Hourglass,
    Cycle
}
public class BookPickupInteractable : PickupInteractable
{
    [Header("Book Info")]
    public BookType bookType;

    public override void OnFocus()
    {
        if (!isHeld)
        {
            interactionText = $"Hold LMB to pick up";
        }
    }

    public override void OnInteract()
    {
    
    }

    public override void OnLoseFocus()
    {
        if (!isHeld)
        {
            interactionText = string.Empty;
        }
    }
}
