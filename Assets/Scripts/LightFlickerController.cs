using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickerController : MonoBehaviour
{
    [SerializeField] private Light pointLight;
    [SerializeField] private Light spotLight;

    [SerializeField] private float dimmingValue;
    private float pointLightMaxBrightness;
    private float spotLightMaxBrightness;

    [SerializeField] private float flickerMinimumDelay;
    [SerializeField] private float flickerMaximumDelay;
    private float timeToFlicker;
    // Start is called before the first frame update
    void Start()
    {
        pointLightMaxBrightness = pointLight.intensity;
        spotLightMaxBrightness = spotLight.intensity;
        timeToFlicker = Time.time + Random.Range(flickerMinimumDelay, flickerMaximumDelay);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timeToFlicker)
        {
            timeToFlicker = Time.time + Random.Range(flickerMinimumDelay, flickerMaximumDelay);
            StartCoroutine(Flicker());
        }
    }

    private IEnumerator Flicker()
    {
        pointLight.intensity = pointLightMaxBrightness * dimmingValue;
        spotLight.intensity = spotLightMaxBrightness * dimmingValue;

        yield return new WaitForSeconds(0.2f);

        pointLight.intensity = pointLightMaxBrightness;
        spotLight.intensity = spotLightMaxBrightness;

        yield return new WaitForSeconds(0.1f);

        pointLight.intensity = pointLightMaxBrightness * dimmingValue;
        spotLight.intensity = spotLightMaxBrightness * dimmingValue;

        yield return new WaitForSeconds(0.3f);

        pointLight.intensity = pointLightMaxBrightness;
        spotLight.intensity = spotLightMaxBrightness;

        yield return new WaitForSeconds(0.2f);

        pointLight.intensity = pointLightMaxBrightness * dimmingValue;
        spotLight.intensity = spotLightMaxBrightness * dimmingValue;

        yield return new WaitForSeconds(0.1f);

        pointLight.intensity = pointLightMaxBrightness;
        spotLight.intensity = spotLightMaxBrightness;
    }
}
