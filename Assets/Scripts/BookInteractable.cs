using UnityEngine;

public class BookInteractable : Interactable
{


    public override void OnFocus()
    {
        interactionText = "Press F ";
    }

    public override void OnInteract()
    {
        InteractionHandler.Instance?.UpdateInteractionText(string.Empty);
        Debug.Log("Book has been interacted with.");
    }

    public override void OnLoseFocus()
    {
        interactionText = string.Empty;
    }
}
