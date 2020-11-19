using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLightController : MonoBehaviour
{
    [SerializeField] Light fireLight;
    [SerializeField] private float maxLumenUpdateDeviation;
    [SerializeField] private float minLumens;
    [SerializeField] private float maxLumens;
    private float currentLumens;

    // Start is called before the first frame update
    void Start()
    {
        currentLumens = fireLight.intensity;
    }
    
    void FixedUpdate()
    {
        float multiplier = Random.Range(-1f, 1f);
        currentLumens = currentLumens + multiplier * maxLumenUpdateDeviation;
        currentLumens = Mathf.Clamp(currentLumens, minLumens, maxLumens);

        fireLight.intensity = currentLumens;
    }
}
