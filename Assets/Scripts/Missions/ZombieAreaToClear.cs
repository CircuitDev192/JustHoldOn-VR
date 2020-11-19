using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAreaToClear : MonoBehaviour
{
    private float timer = 5f;
    [HideInInspector]
    public bool shouldCountDown = false;
    private bool areaCleared = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldCountDown)
        {
            timer -= Time.deltaTime;
        }

        //timer prevents the mission from ending as soon as the player reaches the mission area
        if (ZombieSpawnManager.instance.GetMissionZombies().Count <= 5 && timer <= 0f)
        {
            if (!areaCleared)
            {
                EventManager.TriggerPlayerClearedArea();
                areaCleared = true;
            }
        }
    }
}
