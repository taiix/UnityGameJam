using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxForce = 10f;
    public float sprintSpeed = 8f;
    public float normalSpeed = 4f;

    [Header("Look")]
    public float sensitivity = 1f;
    [SerializeField] private float minPitch = -90f;
    [SerializeField] private float maxPitch = 90f;

    [Header("Grounding")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private int jumpForce = 5;

    [Header("Cinemachine Target")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Vector3 cameraTargetLocalOffset = new Vector3(0f, 1.7f, 0f);

    private Rigidbody rb;
    private PlayerInput playerInput;

    private Vector2 move;
    private Vector2 look;
    private float yaw;
    private float pitch;
    private float speed;
    private bool canJump = true;
    private bool activateControls = true;

    private InputActionMap player;
    private InputAction jumpAction;
    private InputAction sprintAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        player = playerInput.currentActionMap;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        speed = normalSpeed;
        Cursor.lockState = CursorLockMode.Locked;

        if (cameraTarget == null)
        {
            cameraTarget = new GameObject("CameraTarget").transform;
            cameraTarget.SetParent(transform, false);
            cameraTarget.localPosition = cameraTargetLocalOffset;
        }

        yaw = transform.eulerAngles.y;
        pitch = 0f;
    }

    void OnEnable()
    {
        jumpAction = player.FindAction("Jump");
        if (jumpAction != null) { jumpAction.performed += Jump; jumpAction.Enable(); }

        sprintAction = player.FindAction("Sprint");
        if (sprintAction != null) { sprintAction.performed += StartSprinting; sprintAction.canceled += StopSprinting; sprintAction.Enable(); }
    }

    void OnDisable()
    {
        if (jumpAction != null) { jumpAction.performed -= Jump; jumpAction.Disable(); }
        if (sprintAction != null) { sprintAction.performed -= StartSprinting; sprintAction.canceled -= StopSprinting; sprintAction.Disable(); }
    }

    // Read & accumulate look every rendered frame (before Brain runs)
    void Update()
    {
        if (!activateControls) return;

        // Accumulate yaw/pitch here instead of LateUpdate
        yaw += look.x * sensitivity;
        pitch = Mathf.Clamp(pitch - look.y * sensitivity, minPitch, maxPitch);
    }

    void FixedUpdate()
    {
        if (!activateControls) return;

        // Apply yaw via physics
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));

        if (IsOnValidSlope())
            MoveCharacter();
    }

    void LateUpdate()
    {
        if (!activateControls) return;

        // Only apply pitch to target (no further yaw modification)
        if (cameraTarget != null)
        {
            cameraTarget.localPosition = cameraTargetLocalOffset; // keep stable offset
            cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    bool IsOnValidSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 2f))
            return Vector3.Angle(hit.normal, Vector3.up) < maxSlopeAngle;
        return true;
    }

    public void OnMove(InputAction.CallbackContext ctx) => move = ctx.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext ctx) => look = ctx.ReadValue<Vector2>();

    void MoveCharacter()
    {
        Vector3 current = rb.linearVelocity;
        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
        Vector3 forward = yawRot * Vector3.forward;
        Vector3 right = yawRot * Vector3.right;

        Vector3 desired = (right * move.x + forward * move.y) * speed;
        Vector3 change = desired - current;
        change.y = 0f;
        Vector3.ClampMagnitude(change, maxForce);

        rb.AddForce(change, ForceMode.VelocityChange);
    }

    void StartSprinting(InputAction.CallbackContext _) => speed = sprintSpeed;
    void StopSprinting(InputAction.CallbackContext _) => speed = normalSpeed;

    void Jump(InputAction.CallbackContext _)
    {
        if (!canJump) return;
        if (GroundCheck())
        {
            StartCoroutine(JumpCooldown());
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool GroundCheck()
    {
        float radius = GetComponent<CapsuleCollider>().radius * 0.5f;
        float distance = GetComponent<CapsuleCollider>().bounds.extents.y + groundCheckDistance;
        return Physics.SphereCast(rb.position, radius, Vector3.down, out _, distance);
    }

    IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(0.5f);
        canJump = true;
    }

    public void DisableControls() => activateControls = false;
    public void EnableControls() => activateControls = true;

    // For POV (Option B) if needed later
    public void SyncYawFromCamera(Transform camTransform) => yaw = camTransform.rotation.eulerAngles.y;
}
