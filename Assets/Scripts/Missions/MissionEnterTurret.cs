using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnterTurret : MonoBehaviour
{
    private bool finalMissionStarted = false;

    private void Start()
    {
        EventManager.FinalMissionInstantiated += FinalMissionInstantiated;
        EventManager.PlayerEnteredMissionVehicle += PlayerEnteredMissionVehicle;
    }

    private void FinalMissionInstantiated()
    {
        finalMissionStarted = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && finalMissionStarted)
        {
            EventManager.TriggerPlayerEnteredMissionVehicle();
        }
    }

    void PlayerEnteredMissionVehicle()
    {
        EventManager.TriggerPlayerLeftMissionVehicle();
        EventManager.PlayerEnteredMissionVehicle -= PlayerEnteredMissionVehicle;
        //EventManager.StartMission();
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        EventManager.FinalMissionInstantiated -= FinalMissionInstantiated;
        EventManager.PlayerEnteredMissionVehicle -= PlayerEnteredMissionVehicle;
    }
}
