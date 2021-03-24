using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FlareController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private VisualEffect flareEffect;
    [SerializeField] private float flareDuration = 30f;
    [SerializeField] private float extinguishDuration = 2f;

    [SerializeField] Light flareLight;
    [SerializeField] private float maxLumenUpdateDeviation;
    [SerializeField] private float minLumens;
    [SerializeField] private float maxLumens;
    private float currentLumens;
    
    private float timeToExtinguish;
    private float extinguishRate;
    private bool effectIsActive = false;
    private bool isExtinguishing = false;

    private void Start()
    {
        // setting it now just made some logic simpler, even though it'll get reset
        timeToExtinguish = Time.time + flareDuration;

        extinguishRate = maxLumens / extinguishDuration;

        //flareEffect.SetInt("spawnRate", 0);
    }

    // Update is called once per frame
    void Update()
    {
        float magnitude = rigidBody.velocity.magnitude;

        if (magnitude <= 0.1f && !effectIsActive)
        {
            // Want the duration to start from when the flare stops
            timeToExtinguish = Time.time + flareDuration;
            effectIsActive = true;
            flareEffect.SetInt("spawnRate", 10);
        }

        if(Time.time > timeToExtinguish)
        {
            isExtinguishing = true;
            StartCoroutine(ExtinguishRoutine());
        }
    }

    private void FixedUpdate()
    {
        if (isExtinguishing) return;

        float multiplier = Random.Range(-1f, 1f);
        currentLumens = currentLumens + multiplier * maxLumenUpdateDeviation;
        currentLumens = Mathf.Clamp(currentLumens, minLumens, maxLumens);

        flareLight.intensity = currentLumens;
    }

    private IEnumerator ExtinguishRoutine()
    {
        flareEffect.SetInt("spawnRate", 0);

        while (flareLight.intensity > 0)
        {
            currentLumens = currentLumens - extinguishRate * Time.deltaTime;
            currentLumens = Mathf.Clamp(currentLumens, 0, maxLumens);

            flareLight.intensity = currentLumens;

            yield return new WaitForEndOfFrame();
        }
        
        Destroy(this.gameObject);
    }
}
