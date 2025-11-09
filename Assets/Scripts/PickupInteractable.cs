using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupInteractable : Interactable
{
    [Header("Pickup Settings")]
    [SerializeField] private float holdDistance = 2.0f;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float maxDistanceBeforeDrop = 5f;
    [SerializeField] private float holdDrag = 10f;

    [Header("Scroll Distance Settings")]
    [SerializeField] private float minHoldDistance = 0.5f;
    [SerializeField] private float maxHoldDistance = 4.0f;
    [Tooltip("Distance change per one scroll notch (120 units).")]
    [SerializeField] private float scrollDistanceStep = 0.25f;
    [Tooltip("Invert scroll direction.")]
    [SerializeField] private bool invertScroll = false;

    [Header("Rotation Settings")]
    [Tooltip("Degrees per second while holding Q (horizontal/world up axis).")]
    [SerializeField] private float horizontalRotateSpeed = 60f;
    [Tooltip("Degrees per second while holding E (vertical/local X axis).")]
    [SerializeField] private float verticalRotateSpeed = 60f;
    [Tooltip("Clamp local X rotation when rotating vertically (optional).")]
    [SerializeField] private bool clampVertical = true;
    [SerializeField] private float minVerticalAngle = -85f;
    [SerializeField] private float maxVerticalAngle = 85f;

    private BoxCollider boxCollider;
    private Rigidbody rb;
    private Camera cam;
    private float originalDrag;
    private RigidbodyConstraints originalConstraints;

    public override void Awake()
    {
        base.Awake();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        originalDrag = rb.linearDamping;
        originalConstraints = rb.constraints;
    }

    public override void OnFocus()
    {
        if (!isHeld)
        {
            if (this.enabled)
            {
                if (GetComponent<Ingredient>() != null)
                    interactionText = $"Hold LMB to pick up " +
                        $"<color=yellow>{GetComponent<Ingredient>().ingredientName}</color>";
                else interactionText = "Hold LMB to pick up";

            }
        }
    }

    public override void OnLoseFocus()
    {
        if (!isHeld)
        {
            interactionText = string.Empty;
        }
    }

    public override void OnInteract()
    {
        // Not used; pickup handled by Grab action.
    }

    public void BeginHold()
    {
        if (isHeld) return;
        boxCollider.excludeLayers = LayerMask.GetMask("Player");
        isHeld = true;
        interactionText = string.Empty;
        rb.useGravity = false;
        rb.freezeRotation = false;
        rb.linearDamping = holdDrag;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void EndHold()
    {
        if (!isHeld) return;
        boxCollider.excludeLayers = LayerMask.GetMask("Nothing");
        isHeld = false;
        rb.useGravity = true;
        rb.linearDamping = originalDrag;
        rb.constraints = originalConstraints;
    }

    private void Update()
    {
        if (!isHeld) return;

        HandleScrollAdjustDistance();
        HandleRotation();
    }

    private void HandleScrollAdjustDistance()
    {
        if (Mouse.current == null) return;

        // Scroll returns a Vector2; y is vertical scroll (positive usually scroll up).
        float raw = Mouse.current.scroll.ReadValue().y; // Typical values ±120 per notch on Windows.
        if (Mathf.Abs(raw) < Mathf.Epsilon) return;

        float directionFactor = invertScroll ? -1f : 1f;
        // Normalize by typical notch value (120) so one notch = scrollDistanceStep.
        holdDistance += directionFactor * (raw / 120f) * scrollDistanceStep;
        holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, maxHoldDistance);

        // Ensure drop threshold covers max distance so object does not auto-drop when extended.
        if (maxDistanceBeforeDrop < maxHoldDistance)
        {
            maxDistanceBeforeDrop = maxHoldDistance;
        }
    }

    private void HandleRotation()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float dt = Time.deltaTime;

        if (keyboard.qKey.isPressed)
        {
            transform.Rotate(Vector3.up * horizontalRotateSpeed * dt, Space.World);
        }

        if (keyboard.eKey.isPressed)
        {
            transform.Rotate(Vector3.right * verticalRotateSpeed * dt, Space.Self);

            if (clampVertical)
            {
                Vector3 euler = transform.localEulerAngles;
                float x = euler.x;
                if (x > 180f) x -= 360f;
                x = Mathf.Clamp(x, minVerticalAngle, maxVerticalAngle);
                euler.x = x;
                transform.localEulerAngles = euler;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isHeld) return;
        if (cam == null) cam = Camera.main;

        Vector3 targetPos = cam.transform.position + cam.transform.forward * holdDistance;
        Vector3 toTarget = targetPos - rb.position;

        if (toTarget.magnitude > maxDistanceBeforeDrop)
        {
            EndHold();
            return;
        }

        rb.linearVelocity = toTarget * moveSpeed;
    }
}
