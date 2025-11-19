using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;

public class LoadingController : MonoBehaviour
{
    private ProgressBar progressBar;

    void Start()
    {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        progressBar = root.Q<ProgressBar>("LoadingBar");

        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        string targetScene = SceneLoader.GetTargetScene();

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        op.allowSceneActivation = false;

        float minLoadingTime = 1.5f;
        float timer = 0f;
        float fakeProgress = 0f;

        while (!op.isDone)
        {
            //Here is the true value, but I prefer a smooth fake progress
            //progressBar.value = Mathf.Clamp01(op.progress / 0.9f) * 100f;

            fakeProgress = Mathf.MoveTowards(fakeProgress, op.progress, Time.deltaTime * 0.5f);

            progressBar.value = fakeProgress * 100f;

            timer += Time.deltaTime;

            if (op.progress >= 0.9f && timer >= minLoadingTime)
            {
                progressBar.value = 100f;

                yield return new WaitForSeconds(0.2f);

                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}