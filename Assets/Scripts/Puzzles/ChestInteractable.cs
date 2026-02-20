using UnityEngine;

public class ChestInteractable : Interactable
{
    [SerializeField] private Animator chestAnimator;

    bool isOpened = false;
    public override void OnFocus()
    {
        if (!isOpened)
        {
            interactionText = "Press F to open";
        }
        else
        {
            interactionText = string.Empty;
        }
    }

    public override void OnInteract()
    {
        if (isOpened) return;
        interactionText = string.Empty;
        isOpened = true;
        chestAnimator.SetTrigger("ChestTrigger");

    }

    public override void OnLoseFocus()
    {

    }
}
