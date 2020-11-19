using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private int idleAnimationInt;
    [SerializeField] private int weaponAnimationInt;
    [SerializeField] private GameObject floatingText;
    private bool isTalkingToNPC = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetInteger("Animation_int", idleAnimationInt);
        animator.SetInteger("WeaponType_int", weaponAnimationInt);
        EventManager.PlayerAtMissionGiver += PlayerAtMissionGiver;
        EventManager.PlayerLeftMissionGiver += PlayerLeftMissionGiver;
    }

    private void Update()
    {
        floatingText.transform.LookAt(PlayerManager.instance.player.transform, Vector3.up);
        floatingText.transform.eulerAngles = new Vector3(0, floatingText.transform.eulerAngles.y, 0);

        if (!isTalkingToNPC && Vector3.Distance(floatingText.transform.position, PlayerManager.instance.player.transform.position) < 30f)
        {
            floatingText.gameObject.SetActive(true);
        }
        else 
        {
            floatingText.gameObject.SetActive(false);
        }
    }

    private void PlayerLeftMissionGiver()
    {
        isTalkingToNPC = false;
    }

    private void PlayerAtMissionGiver()
    {
        isTalkingToNPC = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EventManager.TriggerPlayerAtMissionGiver();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerPlayerLeftMissionGiver();
        }
    }

    private void OnDestroy()
    {
        EventManager.PlayerAtMissionGiver -= PlayerAtMissionGiver;
        EventManager.PlayerLeftMissionGiver -= PlayerLeftMissionGiver;
    }
}
