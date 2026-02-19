using UnityEngine;

public class CandleInteractable : PickupInteractable
{
    public bool IsSnappedInSlot { get; private set; }
    private Transform snappedParent;

    private bool isPlaced = false;
    private Collider col;


    public override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider>();
    }
    public override void OnFocus()
    {
        if (!isHeld && !isPlaced)
        {
            interactionText = "Hold LMB to pick up";
        }
        else
        {
            interactionText = string.Empty;
        }
    }

    public override void OnInteract()
    {
        if (isPlaced)
        {
            return;
        }
        // No-op
    }

    public override void OnLoseFocus()
    {
            interactionText = string.Empty;

    }

    public void SnapTo(Transform newParent, Transform snapPoint)
    {
        if (isPlaced) return;
        if (snapPoint == null) snapPoint = newParent;

        EndHold();

        IsSnappedInSlot = true;
        snappedParent = newParent;

        transform.SetParent(newParent, worldPositionStays: true);
        transform.position = snapPoint.position;
        transform.rotation = snapPoint.rotation;

        isPlaced = true;
        interactionText = string.Empty;

        if (col != null)
        {
            col.enabled = false;
        }

        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
        }
    }
}
