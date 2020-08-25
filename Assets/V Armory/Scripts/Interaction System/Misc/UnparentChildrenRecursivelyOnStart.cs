using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentChildrenRecursivelyOnStart : MonoBehaviour
{
    public float delay = 0.5f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        Transform[] children = transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
            child.parent = null;
    }
}
