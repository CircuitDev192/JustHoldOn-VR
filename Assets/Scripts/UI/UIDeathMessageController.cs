using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDeathMessageController : MonoBehaviour
{
    private Text deathMessage;
    private Image deathMessageFade;
    [SerializeField] private string[] deathMessageTexts;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.PlayerKilled += PlayerKilled;
        deathMessage = GetComponentInChildren<Text>();
        deathMessageFade = GetComponentInChildren<Image>();
    }

    private void PlayerKilled()
    {
        deathMessage.enabled = true;
        deathMessageFade.enabled = true;

        deathMessage.text = deathMessageTexts[Random.Range(0, deathMessageTexts.Length)];

        deathMessage.GetComponent<Animator>().SetTrigger("Text");
        deathMessageFade.GetComponent<Animator>().SetTrigger("Fade");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnDestroy()
    {
        EventManager.PlayerKilled -= PlayerKilled;
    }
}
