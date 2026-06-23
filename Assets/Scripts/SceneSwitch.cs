using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingPercentage;

    [SerializeField] private float fillDuration = 2f;
  
    public void SwitchToScene(int index)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneAsync(index));
    }

    private IEnumerator LoadSceneAsync(int index)
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        asyncOperation.allowSceneActivation = false;

        float timer = 0f;

        while (timer < fillDuration || asyncOperation.progress < 0.9f)
        {
            timer += Time.deltaTime;

            float timedProgress = Mathf.Clamp01(timer / fillDuration);
            float realProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            loadingBar.value = Mathf.Min(timedProgress, realProgress);

            loadingPercentage.text =
                Mathf.RoundToInt(loadingBar.value * 100f) + "%";

            yield return null;
        }

        loadingBar.value = 1f;
        loadingPercentage.text = "100%";

        asyncOperation.allowSceneActivation = true;
        while(!asyncOperation.isDone)
            yield return null;

        loadingScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
