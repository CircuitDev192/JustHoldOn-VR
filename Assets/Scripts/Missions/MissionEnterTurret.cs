using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnterTurret : MonoBehaviour
{
    [SerializeField]
    private string vehicleName;
    private bool playerInRange = false;
    private bool finalMissionStarted = false;

    private void Start()
    {
        EventManager.FinalMissionInstantiated += FinalMissionInstantiated;
    }

    private void FinalMissionInstantiated()
    {
        finalMissionStarted = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && finalMissionStarted)
        {
            playerInRange = true;
            EventManager.TriggerPlayerCollidedWithMissionVehicle(vehicleName);
            EventManager.PlayerEnteredMissionVehicle += PlayerEnteredMissionVehicle;
            StartCoroutine(WaitForPlayerToEnter());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && finalMissionStarted)
        {
            playerInRange = false;
            EventManager.PlayerEnteredMissionVehicle -= PlayerEnteredMissionVehicle;
            EventManager.TriggerPlayerLeftMissionVehicle();
        }
    }

    void PlayerEnteredMissionVehicle()
    {
        StopAllCoroutines();
        EventManager.TriggerPlayerLeftMissionVehicle();
        EventManager.PlayerEnteredMissionVehicle -= PlayerEnteredMissionVehicle;
        EventManager.StartMission();
    }

    private IEnumerator WaitForPlayerToEnter()
    {
        while (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EventManager.TriggerPlayerEnteredMissionVehicle();
            }
            yield return null;
        }
    }

    private void OnDestroy()
    {
        EventManager.FinalMissionInstantiated -= FinalMissionInstantiated;
        EventManager.PlayerEnteredMissionVehicle -= PlayerEnteredMissionVehicle;
    }
}
