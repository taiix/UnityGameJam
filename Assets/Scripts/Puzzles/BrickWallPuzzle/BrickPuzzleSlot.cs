using UnityEngine;

public class BrickPuzzleSlot : MonoBehaviour
{
    [SerializeField] private Transform snapPoint;

    private BrickInteractable occupant;
    private BrickWallPuzzleManager manager;

    public bool IsSatisfied = false;

    private void Awake()
    {
        manager = GetComponentInParent<BrickWallPuzzleManager>();
        if (snapPoint == null) snapPoint = transform;

        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (occupant != null) return;

        var brick = other.GetComponentInParent<BrickInteractable>();
        if (brick == null) return;
        if (brick.IsSnappedInSlot) return;

        occupant = brick;
        occupant.SnapTo(newParent: transform, snapPoint: snapPoint);
        IsSatisfied = true;

        manager?.RequestEvaluate();
    }

    public void NotifyOccupantLit()
    {
        manager?.RequestEvaluate();
    }
}
