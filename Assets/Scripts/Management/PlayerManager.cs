using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public GameObject player;

    #region Player Values

    [SerializeField] private float playerHealth;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] public float soundMultiplier = 1f;

    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EventManager.PlayerHealthChanged += PlayerHealthChanged;
        EventManager.PlayerKilled += PlayerKilled;
    }

    private void PlayerKilled()
    {
       //This should probably handle stuff like what actually happens to the player upon death.
       // Allow the Game Manager to do scene and menu stuff on death instead.
    }

    private void PlayerHealthChanged(float health)
    {
        playerHealth = health;
    }

    private void OnDestroy()
    {
        EventManager.PlayerHealthChanged -= PlayerHealthChanged;
        EventManager.PlayerKilled -= PlayerKilled;
    }
}
