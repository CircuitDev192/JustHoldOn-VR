using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentChildrenOnStart : MonoBehaviour
{

    public float delay = 0.5f;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(delay);
        transform.DetachChildren();
	}
}
