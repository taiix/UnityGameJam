using UnityEngine;

public class CandlePuzzleSlot : MonoBehaviour
{
    [SerializeField] private Transform snapPoint;
    [SerializeField] private ParticleSystem candleEffect;

    private CandleInteractable occupant;
    private PentagramPuzzleManager manager;

    public bool HasOccupant => occupant != null;
    public bool IsSatisfied => occupant != null;

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
        occupant.SnapTo(newParent: transform, snapPoint: snapPoint);
        if (candleEffect != null)
        {
            candleEffect.gameObject.SetActive(true);
            candleEffect.Play();
        }

        manager?.RequestEvaluate();
    }
}