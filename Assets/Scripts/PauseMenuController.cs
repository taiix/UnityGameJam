using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Root GameObject of the pause menu UI (Canvas/Panel).")]
    [SerializeField] private GameObject menuRoot;
    [Tooltip("Player controller to disable while paused.")]
    [SerializeField] private CharacterMovement characterMovement;
    [Tooltip("Optional: PlayerInput to read a 'Pause' action (Escape/Start).")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Behavior")]
    [SerializeField] private bool pauseWithTimeScale = true;
    [SerializeField] private bool pauseAudioListener = false;
    [SerializeField] private bool unlockCursorOnPause = true;

    private InputAction pauseAction;
    private bool isPaused;

    private void Awake()
    {
        if (characterMovement == null) characterMovement = FindFirstObjectByType<CharacterMovement>();
        if (playerInput == null) playerInput = FindFirstObjectByType<PlayerInput>();
        SetMenuActive(false);
    }

    private void OnEnable()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            // Expect an action named "Pause" bound to Escape/Start
            pauseAction = playerInput.actions.FindAction("Pause", throwIfNotFound: false);
            if (pauseAction != null)
            {
                pauseAction.performed += OnPausePerformed;
                pauseAction.Enable();
            }
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
        }
    }

    private void Update()
    {
        // Fallback if no action asset is wired up
        if (pauseAction == null && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext _)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;

        SetMenuActive(true);

        if (pauseWithTimeScale) Time.timeScale = 0f;
        if (pauseAudioListener) AudioListener.pause = true;

        if (unlockCursorOnPause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        characterMovement?.DisableControls();
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        SetMenuActive(false);

        if (pauseWithTimeScale) Time.timeScale = 1f;
        if (pauseAudioListener) AudioListener.pause = false;

        if (unlockCursorOnPause)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        characterMovement?.EnableControls();
    }

    // UI Button hooks
    public void OnResumeButton() => Resume();
    public void OnQuitButton() => Application.Quit();

    private void SetMenuActive(bool active)
    {
        if (menuRoot != null) menuRoot.SetActive(active);
    }
}
