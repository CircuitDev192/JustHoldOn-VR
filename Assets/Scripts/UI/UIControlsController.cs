using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControlsController : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private void Start()
    {
        backButton.onClick.AddListener(BackButtonClicked);
    }

    private void BackButtonClicked()
    {
        EventManager.TriggerUIControlsBackClicked();
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(BackButtonClicked);
    }
}
