using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagedLightController : MonoBehaviour
{
    [SerializeField] private Light managedLight;

    // Start is called before the first frame update
    void Start()
    {
        LightManager.AddLight(this);
    }

    public float CalculateIllumination(Vector3 position)
    {
        //Debug.Log("Calculating lights illumination!");

        if (!managedLight.enabled)
        {
            //Debug.Log("Flashlight not enabled!");
            return 0f;
        }

        float distance = Vector3.Distance(this.transform.position, position);

        // Outside of the lights range means it isn't illuminating the position
        if (distance > managedLight.range) return 0f;

        Vector3 direction = (position - this.transform.position).normalized;
        Ray ray = new Ray(this.transform.position, direction);

        // Is there an object between the light and the position, thus occluding it?
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            // We hit something before the point
            if (hitInfo.distance < distance) return 0f;
        }

        // Perform illumination calculation based on light type and return
        return (managedLight.type == LightType.Point) ? CalculatePointLightIllumination(position, distance) : CalculateSpotLightIllumination(position, distance);
    }

    private float CalculatePointLightIllumination(Vector3 position, float distance)
    {
        // This should be the correct attenuation calculation
        return managedLight.intensity / (distance * distance);
    }

    private float CalculateSpotLightIllumination(Vector3 position, float distance)
    {
        Vector3 direction = (position - this.transform.position).normalized;

        float angleBetween = Vector3.Angle(this.transform.forward, direction);

        // The point isn't within the spotlight
        if (angleBetween > managedLight.spotAngle / 2f) return 0f;

        // Hopefully this calculation will suffice
        return managedLight.intensity / (distance * distance);
    }

    private void OnDestroy()
    {
        LightManager.RemoveLight(this);
    }
}
