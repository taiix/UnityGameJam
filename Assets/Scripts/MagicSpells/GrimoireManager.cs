using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GrimoireManager : MonoBehaviour
{
    [Header("Grimoire Settings")]
    public int totalPages = 5;
    public int[] maxInputsPerPage = new int[] { 4, 5, 6, 7, 8 };

    [Header("UI Elements")]
    public TMP_Text pageText;
    public GameObject grimoirePanel;

    [Header("Input Settings")]
    public KeyCode previousPageKey = KeyCode.A;
    public KeyCode nextPageKey = KeyCode.E;

    public int CurrentPageIndex { get; private set; } = 0;

    public delegate void PageChanged(int newPageIndex);
    public event PageChanged OnPageChanged;

    // ✅ Spell combos per page
    private Dictionary<int, List<List<string>>> pageCombos = new Dictionary<int, List<List<string>>>
    {
        { 0, new List<List<string>> { new List<string>{ "Top", "Right", "Bottom", "Left" } } },
        { 1, new List<List<string>> { new List<string>{ "Top", "Top", "Right", "Right", "Bottom" } } },
        { 2, new List<List<string>> { new List<string>{ "Left", "Right", "Left", "Right", "Top", "Bottom" } } },
        { 3, new List<List<string>> { new List<string>{ "Bottom", "Top", "Left", "Right", "Bottom", "Top", "Left" } } },
        { 4, new List<List<string>> { new List<string>{ "Top", "Right", "Top", "Left", "Bottom", "Right", "Bottom", "Left" } } }
    };

    void Start()
    {
        UpdatePageUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(previousPageKey))
        {
            SwitchPage(-1);
        }
        else if (Input.GetKeyDown(nextPageKey))
        {
            SwitchPage(1);
        }
    }

    void SwitchPage(int direction)
    {
        int newIndex = Mathf.Clamp(CurrentPageIndex + direction, 0, totalPages - 1);
        if (newIndex != CurrentPageIndex)
        {
            CurrentPageIndex = newIndex;
            Debug.Log("Switched to page: " + (CurrentPageIndex + 1));
            OnPageChanged?.Invoke(CurrentPageIndex);
            UpdatePageUI();
        }
    }

    public int GetMaxInputs()
    {
        return maxInputsPerPage[CurrentPageIndex];
    }

    void UpdatePageUI()
    {
        if (pageText != null)
        {
            pageText.text = $"Page: {CurrentPageIndex + 1}/{totalPages}";
        }
    }

    // ✅ Get valid combos for current page
    public List<List<string>> GetCombosForPage(int pageIndex)
    {
        if (pageCombos.TryGetValue(pageIndex, out var combos))
        {
            return combos;
        }

        return new List<List<string>>();
    }
}
