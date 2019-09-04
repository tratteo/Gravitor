using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public const string ASYNCLOADER_SCENE_NAME = "AsyncLoader";
    public const string MAP_NAME = "LinearMap";
    public void LoadScene(string name)
    {
        int index = SceneUtility.GetBuildIndexByScenePath(name);
        if (index != -1)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            StartCoroutine(SharedUtilities.GetInstance().StartSceneWithDelay(0, index));
        }
    }

    public void LoadSceneAsynchronously(string name)
    {
        int index = SceneUtility.GetBuildIndexByScenePath(name);
        if (index != -1)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            int asyncSceneIndex = SceneUtility.GetBuildIndexByScenePath(ASYNCLOADER_SCENE_NAME);
            AsyncLoadIndexSaver.SetIndexToPreload(index);
            StartCoroutine(SharedUtilities.GetInstance().StartSceneWithDelay(0, asyncSceneIndex));
        }
    }

    public void ReloadCurrentSceneAsynchronously()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index != -1)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            int asyncSceneIndex = SceneUtility.GetBuildIndexByScenePath(ASYNCLOADER_SCENE_NAME);
            AsyncLoadIndexSaver.SetIndexToPreload(index);
            StartCoroutine(SharedUtilities.GetInstance().StartSceneWithDelay(0, asyncSceneIndex));
        }
    }
}
