using UnityEngine;

public class MenuActions : MonoBehaviour
{
    public void OpenSettings(GameObject o) => Bootstrapper.Instance.OpenSettings(o);
    public void CloseSettings() => Bootstrapper.Instance.CloseSettings();

}
