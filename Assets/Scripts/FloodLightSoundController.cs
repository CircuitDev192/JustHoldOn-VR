using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodLightSoundController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip engineSound;
    [SerializeField] private AudioClip engineDisableSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = engineSound;
        audioSource.Play();
        EventManager.DisableFloodLightSounds += DisableFloodLightSounds;
    }

    private void DisableFloodLightSounds()
    {
        audioSource.loop = false;
        audioSource.clip = engineDisableSound;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventManager.DisableFloodLightSounds -= DisableFloodLightSounds;
    }
}
