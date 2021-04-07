using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{

    [SerializeField] private GameObject[] missionPrefabs;
    [SerializeField] public int currentMission;
    [SerializeField] private GameObject[] policeStationLights;
    private bool canTalkToMissionGiver = true;
    Coroutine waitToTalk = null;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.EndMission += EndMission;
        EventManager.PlayerAtMissionGiver += PlayerAtMissionGiver;
        EventManager.PlayerLeftMissionGiver += PlayerLeftMissionGiver;
        EventManager.InstantiateNextMission += InstantiateNextMission;

        //MissionData data = SaveManager.LoadMissionIndex();
        //if (data != null)
        //{
        //    currentMission = data.currentMissionIndex;
        //}
       // else
        ////{
        //    Debug.LogError("Mission Manager did not receive mission data from save file.");
        //}
    }

    private void InstantiateNextMission()
    {
        Instantiate(missionPrefabs[currentMission], Vector3.zero, Quaternion.identity);
    }

    private void PlayerAtMissionGiver()
    {
        waitToTalk = StartCoroutine(WaitForPlayerToTalk());
    }

    private void PlayerLeftMissionGiver()
    {
        StopCoroutine(waitToTalk);
    }

    private void PlayerSpokeToMissionGiver()
    {
        if (canTalkToMissionGiver)
        {
            canTalkToMissionGiver = false;
            if (missionPrefabs[currentMission].TryGetComponent<MissionFetch>(out var fetch))
            {
                EventManager.TriggerPlayerSpokeToMissionGiver(fetch.npcDialog);
            } else if (missionPrefabs[currentMission].TryGetComponent<MissionKill>(out var kill))
            {
                EventManager.TriggerPlayerSpokeToMissionGiver(kill.npcDialog);
            }
            else if (missionPrefabs[currentMission].TryGetComponent<MissionSurvive>(out var survive))
            {
                EventManager.TriggerPlayerSpokeToMissionGiver(survive.npcDialog);
            }
            StartCoroutine(StartNextMission());
        }
    }

    private void EndMission()
    {
        if (currentMission != missionPrefabs.Length - 1)
        {
            currentMission++;
            canTalkToMissionGiver = true;
            SaveManager.SaveMissionIndex(currentMission);
        }
        else
        {
            //Do this after the last mission is ended.
            EventManager.TriggerGameEnded();
        }
    }

    private void DisablePoliceStationLights()
    {
        EventManager.TriggerDisableFloodLightSounds();
        foreach (GameObject light in policeStationLights)
        {
            Destroy(light.gameObject);
        }
    }


    IEnumerator StartNextMission()
    {
        yield return new WaitForSeconds(10f);
        if (missionPrefabs[currentMission].TryGetComponent<MissionSurvive>(out var survive))
        {
            DisablePoliceStationLights();
            EventManager.TriggerPlayerSpokeToMissionGiver(survive.npcDialog2);
        }
        yield return new WaitForSeconds(10f);
        EventManager.TriggerInstantiateNextMission();
    }

    IEnumerator WaitForPlayerToTalk()
    {
        /*while (!Input.GetKeyDown(KeyCode.E))
        {
            yield return null;
        }
        */
        yield return new WaitForSeconds(1f);
        PlayerSpokeToMissionGiver();
    }

    private void OnDestroy()
    {
        EventManager.EndMission -= EndMission;
        EventManager.PlayerAtMissionGiver -= PlayerAtMissionGiver;
        EventManager.PlayerLeftMissionGiver -= PlayerLeftMissionGiver;
        EventManager.InstantiateNextMission -= InstantiateNextMission;
    }
}
