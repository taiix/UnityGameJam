using UnityEngine;

public class TorchInteract : Interactable
{
    GameObject fire;
    public PaintingRoom paintingScript;
    bool fireIsLit;
    [SerializeField]
    private int torchIndex;

    private void Start()
    {
        fire = transform.GetChild(0).transform.gameObject;
    }

    public override void OnFocus()
    {

    }

    public override void OnInteract()
    {
        if (!fireIsLit)
        {
            fire.SetActive(true);
            fireIsLit = true;
        }
        else
        {
            fire.SetActive(false);
            fireIsLit = false;
        }
        paintingScript.SetSwitch(torchIndex, fireIsLit);
    }

    public override void OnLoseFocus()
    {
        
    }
}
