using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public Rigidbody Rigidbody => rb;
    public float CameraRestHeight => cameraTargetLocalOffset.y;

    [Header("Movement")]
    public float maxForce = 10f;
    public float sprintSpeed = 8f;
    public float normalSpeed = 4f;
    [SerializeField] private float airControlForce = 5f;
    [SerializeField] private float controlResumeSoftenDuration = 0.4f;

    [Header("Look (manual when using Cinemachine Option A)")]
    public float sensitivity = 1f;
    [SerializeField] private float minPitch = -90f;
    [SerializeField] private float maxPitch = 90f;

    [Header("Grounding")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float groundSnapDistance = 0.3f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private bool jumpEnabled = true;
    [SerializeField] private int jumpForce = 5;

    [Header("Cinemachine Target")]
    [Tooltip("Child transform used as Tracking Target for CinemachineCamera (NOT the MainCamera).")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Vector3 cameraTargetLocalOffset = new Vector3(0f, 1.7f, 0f);
    [SerializeField] private bool cameraCanMove = true;

    private Rigidbody rb;
    private PlayerInput playerInput;

    private Vector2 move;
    private Vector2 look;
    private float yaw;
    private float pitch;
    private float speed;
    private bool canJump = true;
    private bool activateControls = true;
    private float jumpGraceTimer = 0f;
    private float controlResumeSoftenTimer = 0f;

    private InputActionMap player;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private Coroutine pitchAnimRoutine;
    private Coroutine cameraHeightAnimRoutine;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        player = playerInput.currentActionMap;
    }

    private void Start()
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
            cameraTarget.localRotation = Quaternion.identity;
        }
        Time.timeScale = 1f;
        yaw = transform.eulerAngles.y;
        pitch = 0f;
    }

    private void OnEnable()
    {
        jumpAction = player.FindAction("Jump");
        if (jumpAction != null)
        {
            jumpAction.performed += Jump;
            jumpAction.Enable();
        }

        sprintAction = player.FindAction("Sprint");
        if (sprintAction != null)
        {
            sprintAction.performed += StartSprinting;
            sprintAction.canceled += StopSprinting;
            sprintAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.performed -= Jump;
            jumpAction.Disable();
        }
        if (sprintAction != null)
        {
            sprintAction.performed -= StartSprinting;
            sprintAction.canceled -= StopSprinting;
            sprintAction.Disable();
        }
    }

    private void FixedUpdate()
    {
        if (!activateControls) return;

        if (jumpGraceTimer > 0f)
            jumpGraceTimer -= Time.fixedDeltaTime;

        if (controlResumeSoftenTimer > 0f)
            controlResumeSoftenTimer -= Time.fixedDeltaTime;

        bool grounded = GroundCheck();

        if (!grounded && jumpGraceTimer <= 0f)
        {
            grounded = SnapToGroundIfClose();
        }

        if (grounded)
        {
            if (jumpGraceTimer <= 0f && rb.linearVelocity.y > 0f)
            {
                Vector3 v = rb.linearVelocity;
                v.y = 0f;
                rb.linearVelocity = v;
            }

            if (IsOnValidSlope())
            {
                MoveCharacter();
            }
        }
        else
        {
            MoveCharacterInAir();
        }
    }

    private void LateUpdate()
    {
        if (!activateControls) return;

        if (cameraCanMove)
        {
            yaw += look.x * sensitivity;
            pitch -= look.y * sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        if (cameraTarget != null)
        {
            cameraTarget.localPosition = cameraTargetLocalOffset;
            cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        // Still apply the current yaw even when cameraCanMove == false (keeps orientation stable).
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
    }

    private bool IsOnValidSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 2f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle < maxSlopeAngle;
        }
        return true;
    }

    public void OnMove(InputAction.CallbackContext ctx) => move = ctx.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext ctx)
    {
        // Prevent input from accumulating while locked.
        if (!cameraCanMove)
        {
            look = Vector2.zero;
            return;
        }

        look = ctx.ReadValue<Vector2>();
    }

    private void MoveCharacter()
    {
        Vector3 current = rb.linearVelocity;
        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
        Vector3 forward = yawRot * Vector3.forward;
        Vector3 right = yawRot * Vector3.right;
        Vector3 desired = (right * move.x + forward * move.y) * speed * ControlResumeSoftenFactor();

        Vector3 change = desired - current;
        change.y = 0f;
        change = Vector3.ClampMagnitude(change, maxForce);
        rb.AddForce(change, ForceMode.VelocityChange);
    }

    private void MoveCharacterInAir()
    {
        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
        Vector3 forward = yawRot * Vector3.forward;
        Vector3 right = yawRot * Vector3.right;
        Vector3 inputDir = right * move.x + forward * move.y;

        rb.AddForce(inputDir * airControlForce * ControlResumeSoftenFactor(), ForceMode.Force);
    }

    // While input is disabled (e.g. a cutscene), OnMove keeps caching whatever's held so a
    // key pressed the whole time doesn't get silently dropped. But that means the instant
    // controls resume, MoveCharacter sees that full-held input immediately and would slam
    // velocity straight to top speed in one tick. Ease it back in over a short window instead.
    private float ControlResumeSoftenFactor()
    {
        if (controlResumeSoftenTimer <= 0f || controlResumeSoftenDuration <= 0f) return 1f;
        return 1f - Mathf.Clamp01(controlResumeSoftenTimer / controlResumeSoftenDuration);
    }

    private void StartSprinting(InputAction.CallbackContext _) => speed = sprintSpeed;
    private void StopSprinting(InputAction.CallbackContext _) => speed = normalSpeed;

    private void Jump(InputAction.CallbackContext _)
    {
        if (!jumpEnabled || !canJump) return;
        if (GroundCheck())
        {
            StartCoroutine(JumpCooldown());
            jumpGraceTimer = 0.25f;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool GroundCheck()
    {
        float radius = GetComponent<CapsuleCollider>().radius * 0.5f;
        float distance = GetComponent<CapsuleCollider>().bounds.extents.y + groundCheckDistance;
        bool grounded = Physics.SphereCast(rb.position, radius, Vector3.down, out _, distance);
        return grounded;
    }

    private bool SnapToGroundIfClose()
    {
        var capsule = GetComponent<CapsuleCollider>();
        float radius = capsule.radius * 0.5f;
        float halfHeight = capsule.bounds.extents.y;

        if (!Physics.SphereCast(rb.position, radius, Vector3.down, out RaycastHit hit, halfHeight + groundCheckDistance + groundSnapDistance))
            return false;

        float targetY = hit.point.y + halfHeight;
        if (rb.position.y > targetY)
        {
            Vector3 pos = rb.position;
            pos.y = targetY;
            rb.MovePosition(pos);

            Vector3 v = rb.linearVelocity;
            if (v.y < 0f) v.y = 0f;
            rb.linearVelocity = v;
        }
        return true;
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(0.5f);
        canJump = true;
    }

    public void DisableControls() => activateControls = false;

    public void EnableControls()
    {
        activateControls = true;
        controlResumeSoftenTimer = controlResumeSoftenDuration;
        // Mouse delta refreshes every frame regardless of motion, so clearing it here is
        // safe (unlike `move`, which would get stuck at zero for a still-held key).
        look = Vector2.zero;
        // LateUpdate's rb.MoveRotation(yaw) was skipped the whole time controls were
        // disabled. Resync the cached yaw to whatever the body's rotation actually is
        // right now before that resumes, instead of trusting a value that's been stale
        // for the entire disabled window.
        yaw = transform.eulerAngles.y;
    }

    public void DisableJump() => jumpEnabled = false;
    public void EnableJump() => jumpEnabled = true;
    public void SetJumpEnabled(bool enabled) => jumpEnabled = enabled;

    public void DisableCameraMovement() => cameraCanMove = false;
    public void EnableCameraMovement() => cameraCanMove = true;
    public void SetCameraMovement(bool enabled) => cameraCanMove = enabled;

    // Jumps straight to a pitch value (e.g. to start a sequence looking down) without animating.
    public void SetPitch(float value)
    {
        pitch = Mathf.Clamp(value, minPitch, maxPitch);
        if (cameraTarget != null)
            cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    // Jumps the camera target straight to a given height (x/z keep the configured offset)
    // without animating, e.g. to start a sequence with the camera down low.
    public void SetCameraHeight(float y)
    {
        if (cameraTarget == null) return;
        Vector3 pos = cameraTargetLocalOffset;
        pos.y = y;
        cameraTarget.localPosition = pos;
    }

    // Animates the camera target's height. While activateControls is false, LateUpdate
    // won't be the one driving cameraTarget.localPosition, so this is what's responsible
    // for moving it smoothly instead of it snapping to cameraTargetLocalOffset the instant
    // controls resume.
    public Coroutine AnimateCameraHeight(float targetY, float duration, AnimationCurve curve, System.Action onComplete = null)
    {
        if (cameraHeightAnimRoutine != null) StopCoroutine(cameraHeightAnimRoutine);
        cameraHeightAnimRoutine = StartCoroutine(AnimateCameraHeightRoutine(targetY, duration, curve, onComplete));
        return cameraHeightAnimRoutine;
    }

    private IEnumerator AnimateCameraHeightRoutine(float targetY, float duration, AnimationCurve curve, System.Action onComplete)
    {
        float start = cameraTarget != null ? cameraTarget.localPosition.y : targetY;
        float elapsed = 0f;
        duration = Mathf.Max(0.0001f, duration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float k = curve != null ? curve.Evaluate(t) : t;
            SetCameraHeight(Mathf.Lerp(start, targetY, k));
            yield return null;
        }

        SetCameraHeight(targetY);
        cameraHeightAnimRoutine = null;
        onComplete?.Invoke();
    }

    // Animate pitch over time, then optionally disable controls and invoke onComplete.
    public void AnimatePitch(float targetPitch, float duration, AnimationCurve curve, bool disableControlsAfter = true, System.Action onComplete = null)
    {
        if (pitchAnimRoutine != null) StopCoroutine(pitchAnimRoutine);
        pitchAnimRoutine = StartCoroutine(AnimatePitchRoutine(targetPitch, duration, curve, disableControlsAfter, onComplete));
    }

    private IEnumerator AnimatePitchRoutine(float targetPitch, float duration, AnimationCurve curve, bool disableControlsAfter, System.Action onComplete)
    {
        float start = pitch;
        float end = Mathf.Clamp(targetPitch, minPitch, maxPitch);
        float elapsed = 0f;
        duration = Mathf.Max(0.0001f, duration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float k = curve != null ? curve.Evaluate(t) : t;
            pitch = Mathf.Lerp(start, end, k);

            if (cameraTarget != null)
                cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);

            yield return null;
        }

        pitch = end;
        if (cameraTarget != null)
            cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (disableControlsAfter)
            DisableControls();

        onComplete?.Invoke();
        pitchAnimRoutine = null;
    }

    public void SyncYawFromCamera(Transform cinemachineCameraTransform)
    {
        Vector3 e = cinemachineCameraTransform.rotation.eulerAngles;
        yaw = e.y;
    }
}