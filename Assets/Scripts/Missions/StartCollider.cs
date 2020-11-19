using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCollider : MonoBehaviour
{
    private bool missionStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!missionStarted)
        {
            if (other.CompareTag("Player"))
            {
                EventManager.TriggerStartMission();
                missionStarted = true;
            }
        }
    }
}
