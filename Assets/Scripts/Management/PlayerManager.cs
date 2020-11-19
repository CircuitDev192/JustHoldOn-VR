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
        //what to do when player dies
    }

    private void PlayerHealthChanged(float healthDelta)
    {
        playerHealth += healthDelta;
    }

    private void OnDestroy()
    {
        EventManager.PlayerHealthChanged -= PlayerHealthChanged;
        EventManager.PlayerKilled -= PlayerKilled;
    }
}
