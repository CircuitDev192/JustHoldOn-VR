using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Mission4Skip : MonoBehaviour, MainMenuObject
{
    public void Hit()
    {
        SaveManager.SaveMissionIndex(3);
    }
}
