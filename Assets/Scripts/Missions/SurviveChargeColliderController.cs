using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurviveChargeColliderController : MonoBehaviour
{
    private List<GameObject> zombies = new List<GameObject>();
    private bool matchFound = false;
    private bool isFailed = false;

    private void Start()
    {
        EventManager.ZombieKilled += ZombieKilled;
    }

    private void ZombieKilled(GameObject deadZombie)
    {
        foreach (GameObject zombie in zombies)
        {
            if (zombie == deadZombie)
            {
                zombies.Remove(zombie);
                break;
            }
        }
    }

    private void Update()
    {
        if (zombies.Count > 15 && !isFailed)
        {
            isFailed = true;
            EventManager.TriggerSurvivalMissionFailed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("zombie"))
        {
            foreach (GameObject zombie in zombies)
            {
                if (zombie.gameObject == other.transform.root.gameObject)
                {
                    matchFound = true;
                    break;
                }
            }
            if (!matchFound)
            {
                zombies.Add(other.transform.root.gameObject);
            }
            matchFound = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("zombie"))
        {
            foreach (GameObject zombie in zombies)
            {
                if (zombie.gameObject == other.transform.root.gameObject)
                {
                    zombies.Remove(zombie);
                    break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        EventManager.ZombieKilled -= ZombieKilled;
    }
}
