using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockGraphicsToController : MonoBehaviour
{
    private void Update()
    {
        this.transform.localPosition = new Vector3(0f, this.transform.localPosition.y, 0f);
    }
}
