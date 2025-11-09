using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HighlightOnHover : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private string hoverSound = "ButtonHover";
    [SerializeField] private string clickSound = "ButtonClick";

    // UI elements (require EventSystem + GraphicRaycaster)
    public void OnPointerEnter(PointerEventData eventData) => PlayHover();
    public void OnPointerDown(PointerEventData eventData) => PlayClick();

    public void PlayHover()
    {
        SoundManager.instance?.PlaySound(hoverSound);
        Debug.Log("Hover sound played");
    }
    public void PlayClick() => SoundManager.instance?.PlaySound(clickSound);
}
