using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanSplashSoundController : MonoBehaviour
{

    private AudioSource audioSource;
    [SerializeField] private AudioClip[] splashSounds;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(splashSounds[Random.Range(0, splashSounds.Length)]);
        }
    }
}
