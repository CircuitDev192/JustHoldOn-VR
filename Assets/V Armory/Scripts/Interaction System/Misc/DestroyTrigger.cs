using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTrigger : MonoBehaviour {

	void Start()
	{

	}

	void OnTriggerExit(Collider other)
	{
		Destroy(other.gameObject);
	}
}
