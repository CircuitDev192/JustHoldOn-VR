using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionData
{
    public int currentMissionIndex;

    public MissionData (int currentMission)
    {
        currentMissionIndex = currentMission;
    }
}
