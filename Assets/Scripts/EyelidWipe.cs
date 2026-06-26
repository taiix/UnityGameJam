using System;
using System.Collections;
using UnityEngine;

// Drives two full-width UI panels (top/bottom "lids") via their anchors, so they
// retract off-screen like an eye opening. Resolution-independent since it only
// ever touches anchorMin/anchorMax, never pixel sizes.
public class EyelidWipe : MonoBehaviour
{
    [SerializeField] private RectTransform topLid;
    [SerializeField] private RectTransform bottomLid;

    [Tooltip("Fraction of the screen each lid covers when fully closed. 0.5 = lids meet exactly in the middle.")]
    [SerializeField] private float closedCoverage = 0.5f;

    private Coroutine routine;

    // 0 = fully closed (lids meet in the middle), 1 = fully open (lids retracted off-screen).
    public void SetOpenAmount(float openAmount)
    {
        openAmount = Mathf.Clamp01(openAmount);
        float covered = closedCoverage * (1f - openAmount);

        if (topLid != null)
            topLid.anchorMin = new Vector2(0f, 1f - covered);

        if (bottomLid != null)
            bottomLid.anchorMax = new Vector2(1f, covered);
    }

    public Coroutine Open(float duration, AnimationCurve curve = null, Action onComplete = null)
    {
        return PlayFromTo(0f, 1f, duration, curve, onComplete);
    }

    public Coroutine Close(float duration, AnimationCurve curve = null, Action onComplete = null)
    {
        return PlayFromTo(1f, 0f, duration, curve, onComplete);
    }

    private Coroutine PlayFromTo(float from, float to, float duration, AnimationCurve curve, Action onComplete)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(AnimateOpenAmount(from, to, duration, curve, onComplete));
        return routine;
    }

    private IEnumerator AnimateOpenAmount(float from, float to, float duration, AnimationCurve curve, Action onComplete)
    {
        SetOpenAmount(from);
        float elapsed = 0f;
        duration = Mathf.Max(0.0001f, duration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float k = curve != null ? curve.Evaluate(t) : t;
            SetOpenAmount(Mathf.Lerp(from, to, k));
            yield return null;
        }

        SetOpenAmount(to);
        routine = null;
        onComplete?.Invoke();
    }
}
