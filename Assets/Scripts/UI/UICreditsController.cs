using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreditsController : MonoBehaviour
{
    private Image creditsBG;
    // Start is called before the first frame update
    void Start()
    {
        creditsBG = GetComponentInChildren<Image>();
        EventManager.CreditsUI += CreditsUI;
    }

    private void CreditsUI()
    {
        creditsBG.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventManager.CreditsUI -= CreditsUI;
    }
}
