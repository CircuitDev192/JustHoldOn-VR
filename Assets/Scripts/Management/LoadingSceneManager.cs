using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        yield return null;

        AsyncOperation loadOp = SceneManager.LoadSceneAsync("Game");
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
