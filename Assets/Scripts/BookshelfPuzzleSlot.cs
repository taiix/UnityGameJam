using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BookshelfPuzzleSlot : MonoBehaviour
{
    [Tooltip("Which book type must be placed here.")]
    public BookType requiredType;

    [Tooltip("Where the book will snap (leave null to use this transform).")]
    [SerializeField] private Transform snapPoint;

    private BookPickupInteractable occupant;
    private BookshelfPuzzleManager manager;

    public bool HasOccupant => occupant != null;
    public bool IsSatisfied;

    private void Awake()
    {
        manager = GetComponentInParent<BookshelfPuzzleManager>();
        if (snapPoint == null) snapPoint = transform;
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (occupant != null) return;

        if (other.TryGetComponent(out BookPickupInteractable book))
        {
            if (book.bookType != requiredType) return;

            occupant = book;
            IsSatisfied = true;
            book.transform.SetParent(this.transform);
            manager?.RequestEvaluate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (occupant != null && other.gameObject == occupant.gameObject)
        {
            if(other.GetComponent<BookPickupInteractable>().bookType == requiredType)
            {
                IsSatisfied = false;
            }
            other.transform.SetParent(null);
            occupant = null;
            manager?.RequestEvaluate();
        }
    }
}