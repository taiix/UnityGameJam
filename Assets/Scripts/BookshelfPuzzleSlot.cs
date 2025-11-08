using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BookshelfPuzzleSlot : MonoBehaviour
{
    [Tooltip("Which book type must be placed here.")]
    public BookType requiredType;
    private BookPickupInteractable occupant;         
    private BookshelfPuzzleManager manager;
    private Collider slotCollider;

    public bool HasOccupant => occupant != null;
    public bool IsSatisfied;

    private void Awake()
    {
        slotCollider = GetComponent<Collider>();
        slotCollider.isTrigger = true;
        manager = GetComponentInParent<BookshelfPuzzleManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.TryGetComponent(out BookPickupInteractable book))
        {
            if(book.bookType == requiredType)
            {
                IsSatisfied = true;
                occupant = book;
                other.gameObject.transform.parent = this.transform;
                manager?.RequestEvaluate();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (occupant != null && other.gameObject == occupant.gameObject)
        {
            IsSatisfied = false;
            occupant = null;
            manager?.RequestEvaluate();
        }
    }
}