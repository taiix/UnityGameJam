using UnityEngine;
using UnityEngine.Events;

public class BookshelfPuzzleManager : MonoBehaviour
{
    [SerializeField] private Bookshelf bookshelf;
    [SerializeField] private BookshelfPuzzleSlot[] slots;
    public GameObject enableRoom;
    public UnityEvent OnPuzzleSolved;

    public bool solved = false;
    private bool pendingEvaluate;

    private void Start()
    {
        if (slots == null || slots.Length == 0)
        {
            slots = GetComponentsInChildren<BookshelfPuzzleSlot>();
        }
    }

    private void Update()
    {
        if (pendingEvaluate)
        {
            pendingEvaluate = false;
            Evaluate();
        }
    }

    public void RequestEvaluate()
    {
        pendingEvaluate = true;
    }

    private void Evaluate()
    {
        if (slots == null || slots.Length == 0) return;

        bool allSatisfied = true;
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsSatisfied)
            {
                allSatisfied = false;
                break;
            }
        }

        if (allSatisfied)
        {
            if (!solved)
            {
                solved = true;
                OnPuzzleSolved?.Invoke();
                bookshelf?.MoveBookShelf(false, 2f, 1f);
            }
        }
        else
        {
            if (solved)
            {
                solved = false;
                bookshelf?.MoveBackToOriginal(0f);
                enableRoom.SetActive(true);
            }
        }
    }
}