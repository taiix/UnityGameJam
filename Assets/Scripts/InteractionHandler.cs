using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionHandler : MonoBehaviour
{
    public static InteractionHandler Instance { get; private set; }

    public Vector3 interactionRaypoint = new Vector3(0.5f, 0.5f, 0f);
    public float interactionDistance = default;
    public LayerMask layerMask;

    public Interactable currentInteractable;

    private Camera mainCamera;
    private InputActionAsset inputAsset;
    private InputActionMap player;
    private PlayerInput playerInput;

    [SerializeField] public GameObject interactionUI;

    // New grab support
    private InputAction grabAction;
    private PickupInteractable heldPickup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        playerInput = GetComponentInParent<PlayerInput>();
        inputAsset = playerInput.actions;
        player = inputAsset.FindActionMap("Player");
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        player.FindAction("Interaction").started += Interact;

        grabAction = player.FindAction("Grab");
        if (grabAction != null)
        {
            grabAction.started += OnGrabStarted;
            grabAction.canceled += OnGrabCanceled;
        }
    }

    private void OnDisable()
    {
        player.FindAction("Interaction").started -= Interact;

        if (grabAction != null)
        {
            grabAction.started -= OnGrabStarted;
            grabAction.canceled -= OnGrabCanceled;
        }
    }

    void FixedUpdate()
    {
        // While holding something, skip target switching
        if (heldPickup == null)
        {
            HandleInteractionCheck();
        }
    }

    void HandleInteractionCheck()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main camera is not set!");
            return;
        }

        var ray = mainCamera.ViewportPointToRay(interactionRaypoint);
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, layerMask))
        {
            if (hit.collider.TryGetComponent(out Interactable hitInteractable) && hitInteractable != currentInteractable)
            {
                if (currentInteractable != null)
                {
                    currentInteractable.OnLoseFocus();
                    interactionUI.SetActive(false);
                }

                currentInteractable = hitInteractable;
                currentInteractable.OnFocus();

                if (!string.IsNullOrEmpty(currentInteractable.interactionText))
                {
                    interactionUI.GetComponentInChildren<TextMeshProUGUI>().text = currentInteractable.interactionText;
                    interactionUI.SetActive(true);
                }
                else
                {
                    interactionUI.SetActive(false);
                }
            }
        }
        else if (currentInteractable != null)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
            interactionUI.SetActive(false);
        }
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        HandleInteractionInput();
    }

    void HandleInteractionInput()
    {
        if (heldPickup != null) return; // Ignore normal interact while holding
        if (currentInteractable != null)
        {
            currentInteractable.OnInteract();
        }
    }

    private void OnGrabStarted(InputAction.CallbackContext ctx)
    {
        if (heldPickup != null) return;

        if (currentInteractable != null && currentInteractable is PickupInteractable pickup)
        {
            heldPickup = pickup;
            heldPickup.BeginHold();
            interactionUI.SetActive(false);
        }
    }

    private void OnGrabCanceled(InputAction.CallbackContext ctx)
    {
        if (heldPickup != null)
        {
            heldPickup.EndHold();
            heldPickup = null;

            // Refresh UI if still looking at something interactable
            if (currentInteractable != null)
            {
                currentInteractable.OnFocus();
                if (!string.IsNullOrEmpty(currentInteractable.interactionText))
                {
                    interactionUI.GetComponentInChildren<TextMeshProUGUI>().text = currentInteractable.interactionText;
                    interactionUI.SetActive(true);
                }
            }
        }
    }

    public void UpdateInteractionText(string text)
    {
        interactionUI.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void HideInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
}
