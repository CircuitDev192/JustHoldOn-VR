using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneWheelController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localRotation *= Quaternion.AngleAxis(210 * Time.deltaTime, Vector3.left);
    }
}
