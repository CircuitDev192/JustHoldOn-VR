using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConsumableWheelController : MonoBehaviour
{
    [SerializeField] private Button rockButton;
    [SerializeField] private Button fragButton;
    [SerializeField] private Button flareButton;
    [SerializeField] private Button medkitButton;
    [SerializeField] private Button flashButton;
    [SerializeField] private Text selectedItem;
    [SerializeField] private GameObject[] itemCounts;
    [SerializeField] private string[] itemNames;
    [SerializeField] private GameObject suppressors;

    private void Awake()
    {
        EventManager.UpdateItemCountUI += UpdateItemCountUI;
        rockButton.gameObject.SetActive(false);
        fragButton.gameObject.SetActive(false);
        flareButton.gameObject.SetActive(false);
        medkitButton.gameObject.SetActive(false);
        flashButton.gameObject.SetActive(false);
        selectedItem.gameObject.SetActive(false);
        suppressors.gameObject.SetActive(false);

        EventManager.TriggerPlayerChangedConsumable(selectedItem.text);
        EventManager.TriggerMouseShouldHide(true);
    }

    private void UpdateItemCountUI(string consumableName, int newValue)
    {
        int counter = 0;
        foreach(string consumable in itemNames)
        {
            if (consumable == consumableName)
            {
                itemCounts[counter].GetComponentInChildren<Text>().text = newValue.ToString();
            }
            counter++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            rockButton.gameObject.SetActive(true);
            fragButton.gameObject.SetActive(true);
            flareButton.gameObject.SetActive(true);
            medkitButton.gameObject.SetActive(true);
            flashButton.gameObject.SetActive(true);
            selectedItem.gameObject.SetActive(true);
            suppressors.gameObject.SetActive(true);

            EventManager.TriggerMouseShouldHide(false);       

            Time.timeScale = 0.3f;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            rockButton.gameObject.SetActive(false);
            fragButton.gameObject.SetActive(false);
            flareButton.gameObject.SetActive(false);
            medkitButton.gameObject.SetActive(false);
            flashButton.gameObject.SetActive(false);
            selectedItem.gameObject.SetActive(false);
            suppressors.gameObject.SetActive(false);

            EventManager.TriggerPlayerChangedConsumable(selectedItem.text);
            EventManager.TriggerMouseShouldHide(true);

            Time.timeScale = 1f;
        }
    }

    public void RockButtonHover()
    {
        selectedItem.text = itemNames[0];
    }

    public void FragButtonHover()
    {
        selectedItem.text = itemNames[1];
    }

    public void FlareButtonHover()
    {
        selectedItem.text = itemNames[2];
    }

    public void MedkitButtonHover()
    {
        selectedItem.text = itemNames[3];
    }

    public void FlashButtonHover()
    {
        selectedItem.text = itemNames[4];
    }

    private void OnDestroy()
    {
        EventManager.UpdateItemCountUI -= UpdateItemCountUI;
    }
}
