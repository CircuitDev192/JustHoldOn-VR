using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfoController : MonoBehaviour
{
    // Health Information
    [SerializeField] private Image healthBar;

    // Weapon Information
    [SerializeField] private Text weaponName;
    [SerializeField] private Text roundsInMag;
    [SerializeField] private Text totalAmmo;

    // Attachment Information
    [SerializeField] private Image flashlightPowerBar;
    [SerializeField] private Image suppressorDurabilityBar;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.PlayerHealthChanged += PlayerHealthChanged;

        EventManager.PlayerEnteredMissionVehicle += PlayerEnteredMissionVehicle;
    }

    private void PlayerEnteredMissionVehicle()
    {
        this.gameObject.SetActive(false);
    }

    private void PlayerHealthChanged(float health)
    {
        healthBar.transform.localScale = new Vector3(health / 100f, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    private void OnDestroy()
    {
        EventManager.PlayerHealthChanged -= PlayerHealthChanged;

        EventManager.PlayerEnteredMissionVehicle -= PlayerEnteredMissionVehicle;
    }
}
