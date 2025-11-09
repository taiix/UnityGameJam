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
    [Tooltip("Sound settings panel.")]
    [SerializeField] private GameObject soundSettingsPanel;
    [Tooltip("Controls/settings panel.")]
    [SerializeField] private GameObject controlsPanel;

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
        SetSoundSettingsActive(false);
        SetControlsPanelActive(false);
    }

    private void OnEnable()
    {
        if (playerInput != null && playerInput.actions != null)
        {
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
            pauseAction.performed -= OnPausePerformed;
    }

    private void Update()
    {
        if (pauseAction == null && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscapePress();
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext _)
    {
        HandleEscapePress();
    }

    // ESC logic: If ANY panel (pause, sound, controls) is open -> close ALL and resume.
    // Otherwise open pause menu.
    private void HandleEscapePress()
    {
        if (IsAnyPanelOpen())
        {
            CloseAllPanelsAndResume();
        }
        else
        {
            Pause();
        }
    }

    private bool IsAnyPanelOpen()
    {
        return (menuRoot != null && menuRoot.activeSelf) ||
               (soundSettingsPanel != null && soundSettingsPanel.activeSelf) ||
               (controlsPanel != null && controlsPanel.activeSelf);
    }

    private void CloseAllPanelsAndResume()
    {
        // Resume handles menuRoot & subpanels cleanup.
        Resume();
    }

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;

        SetMenuActive(true);
        SetSoundSettingsActive(false);
        SetControlsPanelActive(false);

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
        SetSoundSettingsActive(false);
        SetControlsPanelActive(false);

        if (pauseWithTimeScale) Time.timeScale = 1f;
        if (pauseAudioListener) AudioListener.pause = false;

        if (unlockCursorOnPause)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        characterMovement?.EnableControls();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    // UI Button hooks
    public void OnResumeButton() => Resume();
    public void OnQuitButton() => Application.Quit();

    public void OnOpenSoundSettings()
    {
        if (!isPaused) Pause();
        SetMenuActive(false);
        SetSoundSettingsActive(true);
        SetControlsPanelActive(false);
    }

    public void OnOpenControlsPanel()
    {
        if (!isPaused) Pause();
        SetMenuActive(false);
        SetControlsPanelActive(true);
        SetSoundSettingsActive(false);
    }

    public void OnCloseSoundSettings()
    {
        SetSoundSettingsActive(false);
        // Do not auto-return to pause; ESC now closes everything.
        if (isPaused && !IsAnyPanelOpen()) SetMenuActive(true);
    }

    public void OnCloseControlsPanel()
    {
        SetControlsPanelActive(false);
        if (isPaused && !IsAnyPanelOpen()) SetMenuActive(true);
    }

    private void SetMenuActive(bool active)
    {
        if (menuRoot != null) menuRoot.SetActive(active);
    }

    private void SetSoundSettingsActive(bool active)
    {
        if (soundSettingsPanel != null) soundSettingsPanel.SetActive(active);
    }

    private void SetControlsPanelActive(bool active)
    {
        if (controlsPanel != null) controlsPanel.SetActive(active);
    }
}
