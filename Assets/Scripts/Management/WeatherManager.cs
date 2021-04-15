﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private GameObject[] weatherTiles;
    [SerializeField] private float activeDistance;
    private Transform playerTransform;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] lightningSounds;
    [SerializeField] private float minTimeBetweenLightning;
    [SerializeField] private float maxTimeBetweenLightning;
    private float timeToNextLightning;
    [SerializeField] private Light lightningFlash;
    
    [SerializeField] private AudioClip[] thunderSounds;
    [SerializeField] private float minTimeBetweenThunder;
    [SerializeField] private float maxTimeBetweenThunder;
    private float timeToNextThunder;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = PlayerManager.instance.player.transform;

        timeToNextLightning = Time.time + Random.Range(minTimeBetweenLightning, maxTimeBetweenLightning);
        timeToNextThunder = Time.time + Random.Range(minTimeBetweenThunder, maxTimeBetweenThunder);
        lightningFlash.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject tile in weatherTiles)
        {
            float distance = Vector3.Distance(playerTransform.position, tile.transform.position);

            if (distance > activeDistance) tile.SetActive(false);

            else tile.SetActive(true);
        }

        if (Time.time > timeToNextLightning)
        {
            AudioClip clip = lightningSounds[Random.Range(0, lightningSounds.Length)];
            audioSource.PlayOneShot(clip);
            timeToNextLightning = Time.time + Random.Range(minTimeBetweenLightning, maxTimeBetweenLightning);

            lightningFlash.enabled = true;
            StartCoroutine(ManageSounds());
            
        }

        if (Time.time > timeToNextThunder)
        {
            AudioClip clip = thunderSounds[Random.Range(0, thunderSounds.Length)];
            audioSource.PlayOneShot(clip);
            timeToNextThunder = Time.time + Random.Range(minTimeBetweenThunder, maxTimeBetweenThunder);
        }

        
    }

    private IEnumerator ManageSounds()
    {
        yield return new WaitForSeconds(0.3f);
        lightningFlash.enabled = false;
    }
}
