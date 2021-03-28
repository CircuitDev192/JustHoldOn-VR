using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPosForHMG : MonoBehaviour
{
    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = this.transform.position;
    }

    public void ResetPos()
    {
        this.transform.position = pos;
    }
}
