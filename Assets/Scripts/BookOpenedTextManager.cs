using TMPro;
using UnityEngine;

public class BookOpenedTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leftPageText;
    [SerializeField] private TextMeshProUGUI rightPageText;

    [SerializeField] private GameObject openBookUI;
    
    private bool isBookOpen = false;

    public void SetBookPageText(string leftText, string rightText)
    {
        if (leftPageText != null)
        {
            leftPageText.text = leftText;
        }
        if (rightPageText != null)
        {
            rightPageText.text = rightText;
        }
    }

    public void ToggleBookUI()
    {
        isBookOpen = !isBookOpen;
        if (openBookUI != null)
        {
            openBookUI.SetActive(isBookOpen);
        }
    }

    private void Update()
    {
        if(isBookOpen &&  Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleBookUI();
        }
    }

}
