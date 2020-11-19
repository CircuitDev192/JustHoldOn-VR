using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Mission5Cheat()
    {
        SaveManager.SaveMissionIndex(3);
        Debug.Log("Mission set to mission 4");
    }

    public void ResetProgress()
    {
        SaveManager.SaveMissionIndex(0);
        Debug.Log("Mission progress reset");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame(){
        Application.Quit();
    }
}
