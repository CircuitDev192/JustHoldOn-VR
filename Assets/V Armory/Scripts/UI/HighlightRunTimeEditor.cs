using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class HighlightRunTimeEditor : MonoBehaviour {

	public Material highlight;
	public Dropdown dropDown;
	public List<Color> colors = new List<Color>();

	public void IncreaseThickness()
	{
		SetHighlightThickness(highlight.GetFloat("g_flOutlineWidth") + 0.001f);
	}

	public void DecreaseThickness()
	{
		SetHighlightThickness(highlight.GetFloat("g_flOutlineWidth") - 0.001f);
	}

	void SetHighlightThickness(float newHighlightThickness)
	{
		newHighlightThickness = Mathf.Clamp(newHighlightThickness, 0.001f, 0.03f);
		highlight.SetFloat("g_flOutlineWidth", newHighlightThickness);
	}

	public void SetHighlightColorByDropDown()
	{
		highlight.SetColor("g_vOutlineColor", colors[dropDown.value]);
	}

}
