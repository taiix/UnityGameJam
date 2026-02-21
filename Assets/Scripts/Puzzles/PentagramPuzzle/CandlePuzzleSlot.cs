using UnityEngine;

public class CandlePuzzleSlot : MonoBehaviour
{
    [SerializeField] private Transform snapPoint;

    private CandleInteractable occupant;
    private PentagramPuzzleManager manager;

    public bool HasOccupant => occupant != null;

    // Slot is satisfied only if a candle is placed AND lit.
    public bool IsSatisfied => occupant != null && occupant.IsLit;

    private void Awake()
    {
        manager = GetComponentInParent<PentagramPuzzleManager>();
        if (snapPoint == null) snapPoint = transform;

        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (occupant != null) return;

        var candle = other.GetComponentInParent<CandleInteractable>();
        if (candle == null) return;
        if (candle.IsSnappedInSlot) return;

        occupant = candle;
        occupant.SnapTo(this.transform, snapPoint: snapPoint);


        manager?.RequestEvaluate();
    }

    public void NotifyOccupantLit()
    {
        manager?.RequestEvaluate();
    }
}