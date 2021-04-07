using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VArmory
{
    public class RestartCurrentScene : MonoBehaviour
    {
        InteractionVolume iv;

        private void Start()
        {
            iv = GetComponent<InteractionVolume>();

            if (!iv)
            {
                Debug.LogError("Interaction Volume on RestartScene Obj is invalid");
            }

            iv._StartInteraction += RestartScene;
        }

        public void RestartScene()
        {
            iv.StopInteraction();
            //reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
