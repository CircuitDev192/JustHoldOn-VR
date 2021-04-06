using UnityEngine;
using UnityEngine.UI;

public class UIDeathMessageController : MonoBehaviour
{
    private Text deathMessage;
    private Image deathMessageFade;
    [SerializeField] private string[] deathMessageTexts;
    [SerializeField] private string[] npcKilledMessageTexts;
    private bool failState = false;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.PlayerKilled += PlayerKilled;
        EventManager.NPCKilled += NPCKilled;
        deathMessage = GetComponentInChildren<Text>();
        deathMessageFade = GetComponentInChildren<Image>();
    }

    private void NPCKilled()
    {
        if (!failState)
        {
            failState = true;
            deathMessage.enabled = true;
            deathMessageFade.enabled = true;

            deathMessage.text = npcKilledMessageTexts[Random.Range(0, npcKilledMessageTexts.Length)];

            deathMessage.GetComponent<Animator>().SetTrigger("Text");
            deathMessageFade.GetComponent<Animator>().SetTrigger("Fade");
        }
    }

    private void PlayerKilled()
    {
        if (!failState)
        {
            failState = true;
            deathMessage.enabled = true;
            deathMessageFade.enabled = true;

            deathMessage.text = deathMessageTexts[Random.Range(0, deathMessageTexts.Length)];

            deathMessage.GetComponent<Animator>().SetTrigger("Text");
            deathMessageFade.GetComponent<Animator>().SetTrigger("Fade");
        }
    }



    private void OnDestroy()
    {
        EventManager.PlayerKilled -= PlayerKilled;
        EventManager.NPCKilled -= NPCKilled;
    }
}
