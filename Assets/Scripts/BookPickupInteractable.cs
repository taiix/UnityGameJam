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

    // Snapping state (kept only for books)
    public bool IsSnappedInSlot { get; private set; }
    private Transform snappedParent;

    public override void OnFocus()
    {
        if (!isHeld)
        {
            interactionText = "Hold LMB to pick up";
        }
    }

    public override void OnInteract()
    {
        // No-op
    }

    public override void OnLoseFocus()
    {
        if (!isHeld)
        {
            interactionText = string.Empty;
        }
    }
}
