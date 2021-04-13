using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManagerButBetter : MonoBehaviour
{

    private void OnValidate()
    {
        foreach (Transform child in this.gameObject.transform)
        {
            child.parent = null;
        }

        GameObject[] tmp = GameObject.FindGameObjectsWithTag("EnvConcrete");
        foreach (GameObject obj in tmp)
        {
            if (Vector3.Distance(this.gameObject.transform.position, obj.transform.position) <= 35f && obj.GetComponentInParent<TileManagerButBetter>() == null)
            {
                obj.transform.parent = this.gameObject.transform;
            }
        }
        GameObject[] tmp2 = GameObject.FindGameObjectsWithTag("EnvMetal");
        foreach (GameObject obj in tmp2)
        {
            if (Vector3.Distance(this.gameObject.transform.position, obj.transform.position) <= 35f && obj.GetComponentInParent<TileManagerButBetter>() == null)
            {
                obj.transform.parent = this.gameObject.transform;
            }
        }
        GameObject[] tmp3 = GameObject.FindGameObjectsWithTag("EnvGrass");
        foreach (GameObject obj in tmp3)
        {
            if (Vector3.Distance(this.gameObject.transform.position, obj.transform.position) <= 35f && obj.GetComponentInParent<TileManagerButBetter>() == null)
            {
                obj.transform.parent = this.gameObject.transform;
            }
        }
        GameObject[] tmp4 = GameObject.FindGameObjectsWithTag("EnvDirt");
        foreach (GameObject obj in tmp4)
        {
            if (Vector3.Distance(this.gameObject.transform.position, obj.transform.position) <= 35f && obj.GetComponentInParent<TileManagerButBetter>() == null)
            {
                obj.transform.parent = this.gameObject.transform;
            }
        }
    }
}
