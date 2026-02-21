using UnityEngine;
using UnityEngine.Events;
using System.Collections; // <-- ADD

public class PentagramPuzzleManager : MonoBehaviour
{
    [SerializeField] private CandlePuzzleSlot[] slots;

    [SerializeField] private GameObject completionParticles;
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float spawnDelay = 2f;

    public UnityEvent OnPuzzleSolved;

    private bool solved;
    private bool pendingEvaluate;

    private void Awake()
    {
        if (slots == null || slots.Length == 0)
        {
            slots = GetComponentsInChildren<CandlePuzzleSlot>(includeInactive: true);
        }
    }

    private void Update()
    {
        if (!pendingEvaluate) return;
        pendingEvaluate = false;
        Evaluate();
    }

    public void RequestEvaluate()
    {
        pendingEvaluate = true;
    }

    private void Evaluate()
    {
        if (solved) return;
        if (slots == null || slots.Length == 0) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsSatisfied)
            {
                Debug.Log($"[PentagramPuzzle] Slot {i} not satisfied. HasOccupant={slots[i].HasOccupant} IsSatisfied={slots[i].IsSatisfied} SlotName={slots[i].name}");
                return;
            }
        }

        solved = true;

        Debug.Log("[PentagramPuzzle] Solved!");

        if (completionParticles != null)
            completionParticles.SetActive(true);

        if (objectToSpawn != null)
            StartCoroutine(SpawnAfterDelay());

        OnPuzzleSolved?.Invoke();
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        var point = spawnPoint != null ? spawnPoint : transform;
        Instantiate(objectToSpawn, point.position, point.rotation);

        objectToSpawn = null;
    }
}
