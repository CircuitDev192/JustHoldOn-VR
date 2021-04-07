using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

public class LightFlickerController : MonoBehaviour
{
    [SerializeField] private Light pointLight;
    [SerializeField] private Light spotLight;
    [SerializeField] private VolumetricLightBeam volumetrics;

    [SerializeField] private float dimmingValue;
    private float pointLightMaxBrightness;
    private float spotLightMaxBrightness;
    private float VolumetricsMaxBrightness;

    [SerializeField] private float flickerMinimumDelay;
    [SerializeField] private float flickerMaximumDelay;
    private float timeToFlicker;
    private bool hasVolumetrics = false;
    // Start is called before the first frame update
    void Start()
    {
        pointLightMaxBrightness = pointLight.intensity;
        spotLightMaxBrightness = spotLight.intensity;
        if (volumetrics != null)
        {
            hasVolumetrics = true;
            VolumetricsMaxBrightness = volumetrics.intensityGlobal;
        } else
        {
            hasVolumetrics = false;
            Debug.Log(this.gameObject + " doesn't have a volumetirc. Should it?");
        }
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
        if (hasVolumetrics) volumetrics.intensityGlobal = VolumetricsMaxBrightness * dimmingValue;

        yield return new WaitForSeconds(0.2f);

        pointLight.intensity = pointLightMaxBrightness;
        spotLight.intensity = spotLightMaxBrightness;
        if (hasVolumetrics) volumetrics.intensityGlobal = VolumetricsMaxBrightness;

        yield return new WaitForSeconds(0.1f);

        pointLight.intensity = pointLightMaxBrightness * dimmingValue;
        spotLight.intensity = spotLightMaxBrightness * dimmingValue;
        if (hasVolumetrics) volumetrics.intensityGlobal = VolumetricsMaxBrightness * dimmingValue;

        yield return new WaitForSeconds(0.3f);

        pointLight.intensity = pointLightMaxBrightness;
        spotLight.intensity = spotLightMaxBrightness;
        if (hasVolumetrics) volumetrics.intensityGlobal = VolumetricsMaxBrightness;

        yield return new WaitForSeconds(0.2f);

        pointLight.intensity = pointLightMaxBrightness * dimmingValue;
        spotLight.intensity = spotLightMaxBrightness * dimmingValue;
        if (hasVolumetrics) volumetrics.intensityGlobal = VolumetricsMaxBrightness * dimmingValue;

        yield return new WaitForSeconds(0.1f);

        pointLight.intensity = pointLightMaxBrightness;
        spotLight.intensity = spotLightMaxBrightness;
        if (hasVolumetrics) volumetrics.intensityGlobal = VolumetricsMaxBrightness;
    }
}
