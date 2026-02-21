using UnityEngine;

public class BrickInteractable : PickupInteractable
{
    public bool IsSnappedInSlot { get; private set; }

    public bool IsLit { get; private set; }

    [Header("Candle VFX")]
    [SerializeField] private ParticleSystem placedEffect;

    private Transform snappedParent;

    private bool isPlaced = false;
    private Collider col;

    public override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider>();

        if (placedEffect != null)
        {
            placedEffect.gameObject.SetActive(false);
            placedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public override void OnFocus()
    {
        if (!isHeld && !IsSnappedInSlot)
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
        if (IsSnappedInSlot)
        {
            return;
        }
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
        placedEffect.gameObject.SetActive(true);
        placedEffect.Play();

        gameObject.layer = 1;


        var body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
            body.useGravity = false;
        }
    }

}
