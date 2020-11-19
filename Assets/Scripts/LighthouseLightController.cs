using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighthouseLightController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.eulerAngles += new Vector3(0, (10f * Time.deltaTime), 0);
    }
}
