using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        yield return null;

        AsyncOperation loadOp;

        if (sceneToLoad == "")
        {
            loadOp = SceneManager.LoadSceneAsync("Game");
            Debug.LogError("LoadingSceneManager was not given a scene to load.");
        }
        else
        {
            loadOp = SceneManager.LoadSceneAsync(sceneToLoad);
        }

        loadOp.allowSceneActivation = false;
        yield return new WaitForSeconds(5f);

        while (!loadOp.isDone)
        {
            if (loadOp.progress >= 0.9f)
            {
                loadOp.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
