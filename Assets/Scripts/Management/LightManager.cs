using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LightManager
{
    private static List<ManagedLightController> lights = new List<ManagedLightController>();

    public static Vector3 CalculateIllumination(Vector3 position)
    {
        Vector3 finalIllumination = new Vector3(0f, 0f, 0f);

        // Sum the illumination from each light
        foreach(ManagedLightController light in lights)
        {
            float illumination = light.CalculateIllumination(position);

            Vector3 direction = (position - light.transform.position).normalized;

            finalIllumination += illumination * direction;
        }

        return finalIllumination;
    }

    public static void AddLight(ManagedLightController light)
    {
        lights.Add(light);
    }

    public static void RemoveLight(ManagedLightController light)
    {
        lights.Remove(light);
    }
}
