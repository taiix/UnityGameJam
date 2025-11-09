using UnityEngine;

public class OpenBookInteractable : Interactable
{
    [TextArea(5,10)]
    public string leftPageContent;

    [TextArea(5, 10)]
    public string rightPageContent;

    public override void OnFocus()
    {
        interactionText = "Press F to Open Book";
    }

    public override void OnInteract()
    {
        InteractionHandler.Instance?.UpdateInteractionText(string.Empty);
        FindFirstObjectByType<BookOpenedTextManager>().SetBookPageText(leftPageContent, rightPageContent);
        FindFirstObjectByType<BookOpenedTextManager>().ToggleBookUI();
    }

    public override void OnLoseFocus()
    {
        interactionText = string.Empty;
    }

}
