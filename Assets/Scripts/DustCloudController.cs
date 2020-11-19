using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DustCloudController : MonoBehaviour
{
    [SerializeField] private VisualEffect dustEffect;
    [SerializeField] private float heliDistanceToGroundThreshold;
    [SerializeField] private Transform heliTransform;

    // Start is called before the first frame update
    void Start()
    {
        dustEffect.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;

        transform.position = new Vector3(position.x, 0, position.z);

        if (Vector3.Distance(heliTransform.position, transform.position) < heliDistanceToGroundThreshold) dustEffect.enabled = true;
        else dustEffect.enabled = false;
    }
}
