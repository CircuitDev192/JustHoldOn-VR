using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_StartGame : MonoBehaviour, MainMenuObject
{
    public void Hit()
    {
        SceneManager.LoadScene("Loading", LoadSceneMode.Single);
    }
}
