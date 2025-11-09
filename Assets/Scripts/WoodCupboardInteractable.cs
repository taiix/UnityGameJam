using UnityEngine;

public class WoodCupboardInteractable : Interactable
{
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Animation")]
    [SerializeField] private float rotateDuration = 0.6f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private BoxCollider frontCollider;

    // Target local rotations
    private Quaternion leftClosedRot;
    private Quaternion rightClosedRot;
    private Quaternion leftOpenRot;
    private Quaternion rightOpenRot;

    private bool isOpen = false;
    private bool animating = false;
    private Coroutine currentAnimation;

    public override void OnFocus()
    {
        if (!animating)
        {
            if (isOpen)
            {
                interactionText = string.Empty;
            }
            else
            {

                interactionText = "Press F to open";
            }
        }
    }

    public override void OnInteract()
    {
        // Toggle desired state
        isOpen = !isOpen;
        if (isOpen)
        {
            frontCollider.enabled = false;
        }
        else
        {
                       frontCollider.enabled = true;
        }

        // Restart animation if already running
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(AnimateDoors(isOpen));

        interactionText = string.Empty;
        InteractionHandler.Instance?.UpdateInteractionText(string.Empty);
    }

    public override void OnLoseFocus()
    {
        // No need to clear here; InteractionHandler handles hiding UI.
    }

    private void Awake()
    {
        // Cache rotations (assumes current inspector/default setup is closed state)
        leftClosedRot = leftDoor.localRotation;
        rightClosedRot = rightDoor.localRotation;

        // Original open values from your instant version
        leftOpenRot = Quaternion.Euler(-90, 0, 100);
        rightOpenRot = Quaternion.Euler(-90, 0, -100);
    }

    private System.Collections.IEnumerator AnimateDoors(bool opening)
    {
        animating = true;

        // Determine start/end rotations
        Quaternion lStart = leftDoor.localRotation;
        Quaternion rStart = rightDoor.localRotation;
        Quaternion lEnd = opening ? leftOpenRot : leftClosedRot;
        Quaternion rEnd = opening ? rightOpenRot : rightClosedRot;

        // Enable colliders only after fully open (keep them disabled while moving closed)
        if (!opening)
        {
            ToggleDoorColliders(false);
        }

        float t = 0f;
        while (t < rotateDuration)
        {
            float normalized = t / rotateDuration;
            float eased = easeCurve.Evaluate(normalized);

            leftDoor.localRotation = Quaternion.Slerp(lStart, lEnd, eased);
            rightDoor.localRotation = Quaternion.Slerp(rStart, rEnd, eased);

            t += Time.deltaTime;
            yield return null;
        }

        // Snap final to ensure precision
        leftDoor.localRotation = lEnd;
        rightDoor.localRotation = rEnd;

        if (opening)
        {
            ToggleDoorColliders(true);
        }

        animating = false;
        currentAnimation = null;
    }

    private void ToggleDoorColliders(bool enabledState)
    {
        if (leftDoor.TryGetComponent<BoxCollider>(out var lCol))
            lCol.enabled = enabledState;
        if (rightDoor.TryGetComponent<BoxCollider>(out var rCol))
            rCol.enabled = enabledState;
    }
}
