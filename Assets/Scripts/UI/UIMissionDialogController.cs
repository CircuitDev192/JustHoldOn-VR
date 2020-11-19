using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionDialogController : MonoBehaviour
{
    [SerializeField] private Text talkPrompt;
    [SerializeField] private Text missionDialog;

    private bool canTalk = true;


    // Start is called before the first frame update
    void Start()
    {
        EventManager.PlayerAtMissionGiver += PlayerAtMissionGiver;
        EventManager.PlayerLeftMissionGiver += PlayerLeftMissionGiver;
        EventManager.PlayerSpokeToMissionGiver += PlayerSpokeToMissionGiver;
        EventManager.InstantiateNextMission += InstantiateNextMission;
        EventManager.EndMission += EndMission;
    }

    private void EndMission()
    {
        canTalk = true;
    }

    private void InstantiateNextMission()
    {
        missionDialog.gameObject.SetActive(false);
    }

    private void PlayerSpokeToMissionGiver(string npcDialog)
    {
        talkPrompt.gameObject.SetActive(false);
        missionDialog.gameObject.SetActive(true);
        missionDialog.text = npcDialog;
        canTalk = false;
    }

    private void PlayerLeftMissionGiver()
    {
        talkPrompt.gameObject.SetActive(false);
    }

    private void PlayerAtMissionGiver()
    {
        if (canTalk)
        {
            talkPrompt.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        EventManager.PlayerAtMissionGiver -= PlayerAtMissionGiver;
        EventManager.PlayerLeftMissionGiver -= PlayerLeftMissionGiver;
        EventManager.PlayerSpokeToMissionGiver -= PlayerSpokeToMissionGiver;
        EventManager.InstantiateNextMission -= InstantiateNextMission;
        EventManager.EndMission -= EndMission;
    }
}
