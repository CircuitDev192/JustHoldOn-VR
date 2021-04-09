using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_ResetProgress : MonoBehaviour, MainMenuObject
{
    public void Hit()
    {
        SaveManager.SaveMissionIndex(0);
    }
}
