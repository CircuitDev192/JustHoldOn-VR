using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleText : MonoBehaviour
{
	Text text;
	public string textA;
	public string textB;

	void Start()
	{
		text = GetComponent<Text>();
	}

	public void _ToggleText()
	{
		text.text = text.text == textA ? textB : textA;
	}
}
