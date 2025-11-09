using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BothKnightsGate : MonoBehaviour
{
    [Header("Knights")]
    [SerializeField] private Knight knightA;
    [SerializeField] private Knight knightB;

    [Header("Gates to move (spread away from origin)")]
    [SerializeField] private GameObject[] gates;

    [Header("Spread Settings")]
    [SerializeField] private Transform spreadOrigin;
    [SerializeField] private bool horizontalOnly = true;

    [Header("Motion")]
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private AnimationCurve ease = null;

    [Header("Lock & Look Up")]
    [SerializeField] private float lockDelayAfterOpen = 0.5f;
    [SerializeField] private float forcedLookUpPitch = -60f;
    [SerializeField] private float lookUpDuration = 2f;
    [SerializeField] private AnimationCurve lookUpEase = null;

    [Header("References")]
    [SerializeField] private CharacterMovement playerMovement;
    [SerializeField] private InteractionHandler interactionHandler;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private FadeTextThenEnableButton postLookSequence; // run when camera reached end

    private bool fired;

    private void Awake()
    {
        if (ease == null) ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        if (lookUpEase == null) lookUpEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        if (spreadOrigin == null) spreadOrigin = transform;
    }

    private void Update()
    {
        if (fired) return;
        if (knightA == null || knightB == null) return;

        if (knightA.HasSword && knightB.HasSword)
        {
            fired = true;
            StartCoroutine(OpenSequence());
        }
    }

    private IEnumerator OpenSequence()
    {
        yield return SpreadGates();

        if (lockDelayAfterOpen > 0f)
            yield return new WaitForSeconds(lockDelayAfterOpen);

        LockAndRotateUp();
    }

    private void LockAndRotateUp()
    {
        if (playerMovement == null) playerMovement = FindObjectOfType<CharacterMovement>();
        if (playerRb == null && playerMovement != null) playerRb = playerMovement.GetComponent<Rigidbody>();
        if (playerInput == null && playerMovement != null) playerInput = playerMovement.GetComponent<PlayerInput>();
        if (interactionHandler == null) interactionHandler = FindObjectOfType<InteractionHandler>();

        if (interactionHandler != null)
        {
            var pickups = FindObjectsOfType<PickupInteractable>();
            foreach (var p in pickups)
                if (p.isHeld) p.EndHold();

            interactionHandler.HideInteractionUI();
            interactionHandler.enabled = false;
        }

        if (playerInput != null)
            playerInput.enabled = false;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }

        if (playerMovement != null)
        {
            // When look-up animation completes, trigger the fade+button sequence.
            playerMovement.AnimatePitch(
                forcedLookUpPitch,
                lookUpDuration,
                lookUpEase,
                disableControlsAfter: true,
                onComplete: () =>
                {
                    if (postLookSequence != null)
                        postLookSequence.Play();
                });
        }

        Cursor.visible = true;
    }

    private IEnumerator SpreadGates()
    {
        if (gates == null || gates.Length == 0) yield break;

        int n = gates.Length;
        var trs = new Transform[n];
        var rbs = new Rigidbody[n];
        var wasKinematic = new bool[n];
        var startPos = new Vector3[n];
        var targetPos = new Vector3[n];

        for (int i = 0; i < n; i++)
        {
            if (gates[i] == null) continue;
            trs[i] = gates[i].transform;
            rbs[i] = gates[i].GetComponent<Rigidbody>();

            if (rbs[i] != null)
            {
                wasKinematic[i] = rbs[i].isKinematic;
                rbs[i].isKinematic = true;
            }

            startPos[i] = trs[i].position;
            Vector3 outward = startPos[i] - spreadOrigin.position;
            if (horizontalOnly) outward.y = 0f;
            if (outward.sqrMagnitude < 1e-6f) outward = trs[i].TransformDirection(Vector3.back);
            outward.Normalize();
            targetPos[i] = startPos[i] + outward * moveDistance;
        }

        float t = 0f;
        float dur = Mathf.Max(0.0001f, moveDuration);
        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            float k = ease != null ? ease.Evaluate(Mathf.Clamp01(t)) : Mathf.Clamp01(t);

            for (int i = 0; i < n; i++)
            {
                if (trs[i] == null) continue;
                trs[i].position = Vector3.Lerp(startPos[i], targetPos[i], k);
            }
            yield return null;
        }

        for (int i = 0; i < n; i++)
        {
            if (trs[i] != null) trs[i].position = targetPos[i];
            if (rbs[i] != null) rbs[i].isKinematic = wasKinematic[i];
        }
    }

    public void ResetTrigger() => fired = false;
}
