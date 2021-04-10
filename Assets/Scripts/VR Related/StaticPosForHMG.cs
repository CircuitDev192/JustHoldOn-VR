using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPosForHMG : MonoBehaviour
{
    [SerializeField] private Transform pos;

    public void ResetPos()
    {
        this.transform.position = pos.position + new Vector3(0f, 0.0374295f, 0f);
    }
}
