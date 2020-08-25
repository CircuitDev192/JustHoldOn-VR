using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererFade : MonoBehaviour
{
	[SerializeField] float visibility;
	[SerializeField] protected float fadeTime;

	TrailRenderer line;

	void Start()
	{
		line = GetComponent<TrailRenderer>();
		line.sharedMaterial.SetFloat("_InvFade", 3);
	}

	void Update()
	{
		visibility -= Time.deltaTime / fadeTime;
		line.sharedMaterial.SetColor("_TintColor", new Color(.5f, .5f, .5f, visibility));

		if (visibility <= 0)
			Destroy(gameObject);
	}
}
