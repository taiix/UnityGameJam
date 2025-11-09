using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSwitch : MonoBehaviour
{
    public void SwitchToScene(int index)
    {
        if (index < 0 || index >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Invalid scene index: " + index);
            throw new System.ArgumentOutOfRangeException(nameof(index), "Scene index is out of range.");
        }
        
        SceneManager.LoadScene(index);

    }

    public void QuitGame()
    {
        Application.Quit();
  
    }
}
