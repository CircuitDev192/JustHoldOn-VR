using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionItemPickup : MonoBehaviour
{
    [SerializeField]
    private string itemName;
    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            EventManager.TriggerPlayerCollidedWithMissionItem(itemName);
            EventManager.PlayerPickedUpMissionItem += PlayerPickedUpMissionItem;
            StartCoroutine(WaitForPlayerToPickup());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            EventManager.PlayerPickedUpMissionItem -= PlayerPickedUpMissionItem;
            EventManager.TriggerPlayerLeftMissionItem();
        }
    }

    private void OnDestroy()
    {
        EventManager.PlayerPickedUpMissionItem -= PlayerPickedUpMissionItem;
        EventManager.TriggerPlayerLeftMissionItem();
    }

    void PlayerPickedUpMissionItem()
    {
        StopAllCoroutines();
        EventManager.PlayerPickedUpMissionItem -= PlayerPickedUpMissionItem;
        Destroy(this.gameObject);
    }

    private IEnumerator WaitForPlayerToPickup()
    { 
        while (playerInRange)
        {
            /*
             * if (Input.GetKeyDown(KeyCode.E))
            {
                EventManager.TriggerPlayerPickedUpMissionItem();
            }
            */
            yield return new WaitForSeconds(1f);
            EventManager.TriggerPlayerPickedUpMissionItem();
            yield return null;
        }
    }
}
