using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionAreaCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerPlayerAtMissionArea();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ZombieSpawnManager.instance.StopMissionZombieSpawns();
            EventManager.TriggerPlayerLeftMissionArea();
        }
    }
}
