using UnityEngine;

public class BookInteractable : Interactable
{
    bool hasInteracted = false;

    public override void OnFocus()
    {
        if (hasInteracted) return;
        interactionText = "Press F ";
    }

    public override void OnInteract()
    {
        InteractionHandler.Instance?.UpdateInteractionText(string.Empty);
        //Debug.Log("Book has been interacted with.");
        hasInteracted = true;

    }

    public override void OnLoseFocus()
    {
        interactionText = string.Empty;
    }
}
