using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeTextThenEnableButton : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Graphic textGraphic;     // works with TextMeshProUGUI or UI Text/Image
    [SerializeField] private Button targetButton;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 1.5f;      // seconds for alpha 0->1
    [SerializeField] private float buttonEnableDelay = 3f;   // seconds after start to enable button
    [SerializeField] private bool useUnscaledTime = true;    // survive Time.timeScale = 0

    [Header("Behavior")]
    [SerializeField] private AnimationCurve ease = null;     // easing for fade (0..1)
    [SerializeField] private bool playOnEnable = false;      // auto-run on enable
    [SerializeField] private bool resetOnEnable = true;      // reset state when object (re)enables (e.g., after returning to scene)
    [SerializeField] private bool disableButtonAtStart = true;
    [SerializeField] private bool setButtonActive = true;    // also SetActive(true) when enabling
    [SerializeField] private bool setButtonInteractable = true;
    [SerializeField] private bool unlockCursorOnStart = true; // show/unlock cursor when text appears

    private Coroutine routine;

    private void Awake()
    {
        if (ease == null) ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    }

    private void OnEnable()
    {
        if (resetOnEnable) ResetState();
        if (playOnEnable) Play();
    }

    public void Play()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Run());
    }

    public void ResetState()
    {
        if (textGraphic != null)
        {
            if (!textGraphic.gameObject.activeSelf)
                textGraphic.gameObject.SetActive(true);

            var c = textGraphic.color;
            textGraphic.color = new Color(c.r, c.g, c.b, 0f);
        }

        if (targetButton != null && disableButtonAtStart)
        {
            if (setButtonActive) targetButton.gameObject.SetActive(false);
            if (setButtonInteractable) targetButton.interactable = false;
        }
    }

    private IEnumerator Run()
    {
        if (textGraphic == null) yield break;

        // Ensure cursor is usable when the text appears
        if (unlockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Prepare text (alpha 0) and button disabled state
        ResetState();

        // Fade alpha 0 -> 1
        Color baseCol = textGraphic.color;
        float t = 0f;
        float dur = Mathf.Max(0.0001f, fadeDuration);

        while (t < 1f)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt / dur;
            float k = ease.Evaluate(Mathf.Clamp01(t));
            textGraphic.color = new Color(baseCol.r, baseCol.g, baseCol.b, k);
            yield return null;
        }
        textGraphic.color = new Color(baseCol.r, baseCol.g, baseCol.b, 1f);

        // Enable button after remaining time (relative to start)
        float remaining = Mathf.Max(0f, buttonEnableDelay - fadeDuration);
        if (remaining > 0f)
        {
            if (useUnscaledTime) yield return new WaitForSecondsRealtime(remaining);
            else yield return new WaitForSeconds(remaining);
        }

        if (targetButton != null)
        {
            if (setButtonActive) targetButton.gameObject.SetActive(true);
            if (setButtonInteractable) targetButton.interactable = true;
        }

        routine = null;
    }
}