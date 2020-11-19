using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEndSequenceController : MonoBehaviour
{
    private Image fadeToFromBlack;
    // Start is called before the first frame update
    void Start()
    {
        fadeToFromBlack = GetComponentInChildren<Image>();
        EventManager.FadeToBlack += FadeToBlack;
        EventManager.StartNextCreditsSequence += StartNextCreditsSequence;
    }

    private void FadeToBlack()
    {
        fadeToFromBlack.GetComponent<Animator>().SetTrigger("FadeToBlack");
    }

    private void StartNextCreditsSequence()
    {
        fadeToFromBlack.GetComponent<Animator>().SetTrigger("FadeFromBlack");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventManager.FadeToBlack -= FadeToBlack;
        EventManager.StartNextCreditsSequence -= StartNextCreditsSequence;
    }
}
