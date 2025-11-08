using UnityEngine;
using UnityEngine.InputSystem;

public class PickupInteractable : Interactable
{
    [Header("Pickup Settings")]
    [SerializeField] private float holdDistance = 2.0f;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float maxDistanceBeforeDrop = 5f;
    [SerializeField] private float holdDrag = 10f;

    [Header("Rotation Settings")]
    [Tooltip("Degrees per second while holding Q (horizontal/world up axis).")]
    [SerializeField] private float horizontalRotateSpeed = 60f;
    [Tooltip("Degrees per second while holding E (vertical/local X axis).")]
    [SerializeField] private float verticalRotateSpeed = 60f;
    [Tooltip("Clamp local X rotation when rotating vertically (optional).")]
    [SerializeField] private bool clampVertical = true;
    [SerializeField] private float minVerticalAngle = -85f;
    [SerializeField] private float maxVerticalAngle = 85f;

    private Rigidbody rb;
    private Camera cam;
    private float originalDrag;
    private RigidbodyConstraints originalConstraints;

    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        originalDrag = rb.linearDamping;
        originalConstraints = rb.constraints;
    }

    public override void OnFocus()
    {
        if (!isHeld)
        {
            interactionText = "Hold LMB to pick up";
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
        isHeld = false;
        rb.useGravity = true;
        rb.linearDamping = originalDrag;
        rb.constraints = originalConstraints;
    }

    private void Update()
    {
        if (!isHeld) return;
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float dt = Time.deltaTime;

        // Hold Q = rotate horizontally (world up)
        if (keyboard.qKey.isPressed)
        {
            transform.Rotate(Vector3.up * horizontalRotateSpeed * dt, Space.World);
        }

        // Hold E = rotate vertically (local X)
        if (keyboard.eKey.isPressed)
        {
            // Apply rotation first
            transform.Rotate(Vector3.right * verticalRotateSpeed * dt, Space.Self);

            // Optionally clamp local X rotation
            if (clampVertical)
            {
                Vector3 euler = transform.localEulerAngles;
                // Convert 0..360 to -180..180 for clamping
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
