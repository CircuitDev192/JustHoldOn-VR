using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCollider : MonoBehaviour {

	void OnCollisionEnter(Collision other)
	{
		Destroy(other.gameObject, 0f);
	}
}
