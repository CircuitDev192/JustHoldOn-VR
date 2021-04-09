using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_QuitGame : MonoBehaviour, MainMenuObject
{
    public void Hit()
    {
        Application.Quit();
    }
}
