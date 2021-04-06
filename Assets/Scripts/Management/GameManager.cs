using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Play,
    Paused,
    Controls,
    Credits
}

public class GameManager : Context<GameManager>
{
    #region States

    public GameMenuState menuState = new GameMenuState();
    public GamePlayState playState = new GamePlayState();
    public GamePauseState pauseState = new GamePauseState();
    public GameCreditsState creditsState = new GameCreditsState();

    #endregion

    #region Fields

    public static GameManager instance;

    private Camera cam;

    [SerializeField] private GameObject[] managerPrefabs;
    private List<GameObject> managers;
    [SerializeField] private GameObject[] weaponPickupPrefabs;
    private GameObject player;

    private string sceneToLoad;
    private string sceneToUnLoad;

    #endregion

    private void Awake()
    {
        instance = this;

        managers = new List<GameObject>();

        foreach (GameObject manager in managerPrefabs)
        {
            managers.Add(Instantiate(manager, this.transform));
        }

        //DontDestroyOnLoad(this);
        InitializeContext();

        EventManager.UIResumeClicked += UIResumeClicked;
        EventManager.UIQuitClicked += UIQuitClicked;
        EventManager.GameEnded += GameEnded;
        EventManager.PlayerKilled += PlayerKilled;
        EventManager.NPCKilled += NPCKilled;
    }

    private void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        player = PlayerManager.instance.player;
        cam = Camera.main;
    }

    private void Update()
    {
        ManageState(this);
    }

    public override void InitializeContext()
    {
        currentState = playState;
        currentState.EnterState(this);
    }

    private void GameEnded()
    {
        StartCoroutine(EndGameSequence());
    }

    private void PlayerKilled()
    {
        //Player manager also listens to this event to play death music
        //Time.timeScale = 0.5f;
        StartCoroutine(WaitToReloadLevel()); //Will reload the scene 
    }

    private void NPCKilled()
    {
        //Player manager also listens to this event to play death music
        //Time.timeScale = 0.5f;
        StartCoroutine(WaitToReloadLevel()); //Will reload the scene 
    }

    IEnumerator WaitToReloadLevel()
    {
        yield return new WaitForSeconds(14f);
        SceneManager.LoadScene("Loading", LoadSceneMode.Single);

        /*
        yield return new WaitForSeconds(1f);
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;
        yield return new WaitForSeconds(9f);
        while (!loadOp.isDone)
        {
            if (loadOp.progress >= 0.9f)
            {
                loadOp.allowSceneActivation = true;
            }
            yield return null;
        }
        */
    }

    #region Scene Methods

    public void LoadScene(string sceneName)
    {
        sceneToLoad = sceneName;
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOp.completed += SceneLoadCompleted;
    }

    public void LoadSceneSynchronous(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private void SceneLoadCompleted(AsyncOperation obj)
    {
        EventManager.TriggerSceneLoaded(sceneToLoad);
    }

    public void UnLoadScene(string sceneName)
    {
        sceneToUnLoad = sceneName;
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
        unloadOp.completed += SceneUnLoadCompleted;
    }

    private void SceneUnLoadCompleted(AsyncOperation obj)
    {
        EventManager.TriggerSceneUnLoaded(sceneToUnLoad);
    }

    #endregion

    #region UIEvents

    private void UIResumeClicked()
    {
        currentState.ExitState(this);

        currentState = playState;
        currentState.EnterState(this);
    }

    private void UIQuitClicked()
    {
        Application.Quit();
    }

    #endregion

    IEnumerator EndGameSequence()
    {
        yield return new WaitForSeconds(3f);
        LoadSceneSynchronous("Ending");
    }

    private void OnDestroy()
    {
        EventManager.UIResumeClicked -= UIResumeClicked;
        EventManager.UIQuitClicked -= UIQuitClicked;
        EventManager.GameEnded -= GameEnded;
        EventManager.PlayerKilled -= PlayerKilled;
        EventManager.NPCKilled -= NPCKilled;
    }
}