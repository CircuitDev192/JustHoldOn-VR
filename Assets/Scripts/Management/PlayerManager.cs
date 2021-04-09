using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public GameObject player;

    [SerializeField] private GameObject playerRig;
    [SerializeField] private Transform whereToTelePlayer;

    #region Player Values

    [SerializeField] private float playerHealth;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip flashbangEarRinging;
    [SerializeField] private AudioClip lastMissionMusic;
    [SerializeField] private AudioClip deathMusic;
    [SerializeField] private AudioClip killedNPCMusic;
    [SerializeField] private AudioClip explosion;
    [SerializeField] public float soundMultiplier = 1f;
    private bool isInLastMission = false;
    private bool failState = false;
    private bool heliHasCrashed = false;

    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EventManager.PlayerHealthChanged += PlayerHealthChanged;
        EventManager.PlayerKilled += PlayerKilled;
        EventManager.NPCKilled += NPCKilled;
        EventManager.PlayerEnteredMissionVehicle += PlayerEnteredMissionVehicle;
        EventManager.FlashbangDetonated += FlashbangDetonated;
        EventManager.HeliCrashed += HeliCrashed;
    }

    private void HeliCrashed()
    {
        heliHasCrashed = true;
        audioSource.Stop();
        FlashbangDetonated(player.transform.position, 5f);
        audioSource.PlayOneShot(explosion, soundMultiplier*2f);
        playerRig.transform.position = whereToTelePlayer.position;
        playerRig.transform.rotation = whereToTelePlayer.rotation;
    }

    private void PlayerKilled()
    {
        //This should probably handle stuff like what actually happens to the player upon death.
        // Allow the Game Manager to do scene and menu stuff on death instead.
        if (!isInLastMission && !failState)
        {
            failState = true;
            audioSource.PlayOneShot(deathMusic);
        }
    }

    private void NPCKilled()
    {
        //Same as player killed but may need additional functions for NPC death
        //  so making a seperate event
        if ((!isInLastMission && !failState) || heliHasCrashed)
        {
            failState = true;
            audioSource.PlayOneShot(killedNPCMusic);
        }
    }

    private void PlayerEnteredMissionVehicle()
    {
        audioSource.PlayOneShot(lastMissionMusic);
        isInLastMission = true;
    }

    private void FlashbangDetonated(Vector3 flashbangPosition, float stunDistance)
    {
        if (Vector3.Distance(flashbangPosition, player.transform.position) < stunDistance)
        {
            audioSource.PlayOneShot(flashbangEarRinging, 0.7f);
            //Lower the volume of all sounds that use this multiplier. 
            soundMultiplier = 0.2f;
            StartCoroutine(IncreaseMultiplierBackToOne());
        }
    }

    private void PlayerHealthChanged(float health)
    {
        playerHealth = health;
    }

    private IEnumerator IncreaseMultiplierBackToOne()
    {
        while (soundMultiplier < 1f)
        {
            soundMultiplier += 0.05f * Time.deltaTime;
            yield return null;
        }
        soundMultiplier = 1f;
    }

    private void OnDestroy()
    {
        EventManager.PlayerHealthChanged -= PlayerHealthChanged;
        EventManager.PlayerKilled -= PlayerKilled;
        EventManager.NPCKilled -= NPCKilled;
        EventManager.PlayerEnteredMissionVehicle -= PlayerEnteredMissionVehicle;
        EventManager.FlashbangDetonated -= FlashbangDetonated;
        EventManager.HeliCrashed -= HeliCrashed;
    }
}
