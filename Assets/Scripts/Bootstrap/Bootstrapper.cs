using UnityEngine;
using UnityEngine.SceneManagement;

public static class BootstrapInit
{
    private static string bootstrapSceneName = "Bootstrap";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene c = SceneManager.GetSceneAt(i);
            if (c.name == bootstrapSceneName)
                return;
        }

        SceneManager.LoadScene(bootstrapSceneName, LoadSceneMode.Additive);
    }
}

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; }

    public GameObject canvasToOpen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(int i)
    {
        SceneSwitch ss = FindAnyObjectByType<SceneSwitch>();
        if (ss != null)
        {
            ss.SwitchToScene(i);
        }
    }

    public void OpenSettings(GameObject o) {
        SettingsManager sm = FindAnyObjectByType<SettingsManager>(FindObjectsInactive.Include);
        Debug.Log("Found settings manager: " + (sm != null));
        if (sm != null)
        {
                sm.transform.gameObject.SetActive(true);
            
        }
        canvasToOpen = o;

    }

    public void CloseSettings() { 

        canvasToOpen.SetActive(true);
    }
}
