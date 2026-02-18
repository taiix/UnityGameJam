using System.Collections.Generic;
using UnityEngine;


public class CursorToggleController : MonoBehaviour
{
    private GrimoireManager grimoire;

    [Header("References")]
    public GameObject directionalUIGroup;
    public KeyCode toggleKey = KeyCode.Mouse1;
    public RectTransform circleCenter;

    [Header("Directional Elements")]
    public RectTransform topElement;
    public RectTransform rightElement;
    public RectTransform bottomElement;
    public RectTransform leftElement;

    [Header("Scaling")]
    public float highlightScale = 1.3f;
    public float normalScale = 1f;
    public float scaleSpeed = 10f;

    [Header("Dead Zone")]
    public float centerDeadZoneRadius = 40f;

    [Header("Combo Display")]
    public Transform comboDisplayParent;
    public GameObject topIconPrefab;
    public GameObject rightIconPrefab;
    public GameObject bottomIconPrefab;
    public GameObject leftIconPrefab;

    [Header("Combo Timeout")]
    public float comboClearDelay = 30f;

    // private FirstPersonController controller;
    private RectTransform hoveredElement;
    private string lastDirection = "";
    private List<GameObject> spawnedIcons = new List<GameObject>();

    private bool isToggling = false;
    private float lastComboTime = -1f;

    private SpellCaster spellCaster;
    private List<string> inputSequence = new List<string>();


    void Start()
    {
        grimoire = Object.FindFirstObjectByType<GrimoireManager>();

        spellCaster = Object.FindFirstObjectByType<SpellCaster>();

        //controller = GetComponent<FirstPersonController>();

        if (directionalUIGroup != null)
        {
            directionalUIGroup.SetActive(false);
            ResetScalesInstantly();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isToggling = true;
            ClearComboIcons();
            directionalUIGroup.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //controller.cameraCanMove = false;
        }

        if (Input.GetKeyUp(toggleKey))
        {
            isToggling = false;
            directionalUIGroup.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            //controller.cameraCanMove = true;

            hoveredElement = null;
            ResetScalesInstantly();

            if (spawnedIcons.Count > 0)
            {
                lastComboTime = Time.time;
            }
        }

        if (Input.GetMouseButtonDown(0) && inputSequence.Count > 0)
        {
            TryCastSpell();
            ClearComboIcons();
        }


        if (spawnedIcons.Count > 0 && lastComboTime > 0 && !isToggling)
        {
            float timeSinceRelease = Time.time - lastComboTime;
            Debug.Log($"Combo active for: {timeSinceRelease:F1}s");

            if (timeSinceRelease > comboClearDelay)
            {
                ClearComboIcons();
            }
        }

        if (isToggling)
        {
            UpdateHoverDetection();
        }

        SmoothScale(topElement, topElement == hoveredElement);
        SmoothScale(rightElement, rightElement == hoveredElement);
        SmoothScale(bottomElement, bottomElement == hoveredElement);
        SmoothScale(leftElement, leftElement == hoveredElement);
    }

    void UpdateHoverDetection()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(circleCenter, mousePos, null, out localPoint);

        if (localPoint.magnitude <= centerDeadZoneRadius)
        {
            hoveredElement = null;
            lastDirection = "";
            return;
        }

        float angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
        angle -= 90f; // Shift so that 0° is up (top)
        if (angle < 0) angle += 360f;

        string direction = "";

        if (IsAngleInRange(angle, 315, 45))
        {
            hoveredElement = topElement;
            direction = "Top";
        }
        else if (IsAngleInRange(angle, 45, 135))
        {
            hoveredElement = rightElement;
            direction = "Right";
        }
        else if (IsAngleInRange(angle, 135, 225))
        {
            hoveredElement = bottomElement;
            direction = "Bottom";
        }
        else
        {
            hoveredElement = leftElement;
            direction = "Left";
        }

        if (direction != "" && direction != lastDirection)
        {
            lastDirection = direction;
            AddDirectionIcon(direction);
        }
    }

    void AddDirectionIcon(string direction)
    {
        if (grimoire != null && spawnedIcons.Count >= grimoire.GetMaxInputs())
        {
            Debug.Log("Max combo length reached for Page " + (grimoire.CurrentPageIndex + 1));
            return;
        }

        GameObject icon = null;

        switch (direction)
        {
            case "Top": icon = Instantiate(topIconPrefab, comboDisplayParent); break;
            case "Right": icon = Instantiate(rightIconPrefab, comboDisplayParent); break;
            case "Bottom": icon = Instantiate(bottomIconPrefab, comboDisplayParent); break;
            case "Left": icon = Instantiate(leftIconPrefab, comboDisplayParent); break;
        }

        if (icon != null)
        {
            spawnedIcons.Add(icon);
        }

        /*
  if (inputSequence.Count > 0 && inputSequence[inputSequence.Count - 1] == direction)
      return;
  */

        inputSequence.Add(direction);


        lastComboTime = Time.time;
    }

    void ClearComboIcons()
    {
        foreach (GameObject icon in spawnedIcons)
            Destroy(icon);

        spawnedIcons.Clear();
        inputSequence.Clear();
        lastDirection = "";
        lastComboTime = -1f;
    }

    bool IsAngleInRange(float angle, float min, float max)
    {
        if (min < max)
            return angle >= min && angle < max;
        else
            return angle >= min || angle < max;
    }

    void SmoothScale(RectTransform element, bool isHovered)
    {
        float target = isHovered ? highlightScale : normalScale;
        element.localScale = Vector3.Lerp(element.localScale, Vector3.one * target, Time.deltaTime * scaleSpeed);
    }

    void ResetScalesInstantly()
    {
        topElement.localScale = Vector3.one * normalScale;
        rightElement.localScale = Vector3.one * normalScale;
        bottomElement.localScale = Vector3.one * normalScale;
        leftElement.localScale = Vector3.one * normalScale;
    }
    void TryCastSpell()
    {
        if (grimoire == null || spellCaster == null) return;

        int currentPage = grimoire.CurrentPageIndex;
        string spell = spellCaster.MatchSpell(currentPage, inputSequence);

        if (!string.IsNullOrEmpty(spell))
        {
            Debug.Log($"✨ Casted spell: {spell}");
            // Optional: trigger animation, VFX, or send spell event
        }
        else
        {
            Debug.Log("❌ Invalid spell combo.");
        }
    }

}
