using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VArmory;

public class EndingSequenceManager : MonoBehaviour
{
    [SerializeField] private GameObject invisWalls; //hold player in at the end
    [SerializeField] private GameObject[] npcs;
    private int currentCreditsSequence = 0;
    [SerializeField] private GameObject playerRig;
    [SerializeField] private GameObject playerController;
    [SerializeField] private GameObject heli;
    [SerializeField] private Transform playerTele;
    [SerializeField] private Transform playerTeleHeli;
    [SerializeField] private Animator fadeInOut;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartNextCreditsSequence += StartNextCreditsSequence;
    }

    private void Update()
    {
        if (Vector3.Distance(heli.transform.position, playerRig.transform.position) < 7f && currentCreditsSequence == 1)
        {
            StartNextCreditsSequence();
        }
    }

    private void StartNextCreditsSequence()
    {
        currentCreditsSequence++;
        switch (currentCreditsSequence)
        {
            case 1:
                //fade to black first, then do this
                fadeInOut.SetTrigger("FadeToBlack");
                StartCoroutine(WaitToTelePlayer());
                
                break;
            case 2:

                //fade to black again
                fadeInOut.SetTrigger("FadeToBlack");
                StartCoroutine(WaitToTelePlayer2());
                StartCoroutine(WaitToEndGame());
                break;
            default:
                break;
        }
    }

    IEnumerator WaitToTelePlayer()
    {
        yield return new WaitForSeconds(2.6f);
        playerRig.transform.position = playerTele.position;
        playerRig.transform.rotation = playerTele.rotation;
        
        invisWalls.SetActive(true);

        foreach (GameObject npc in npcs)
        {
            npc.GetComponent<CutsceneNPCController>().meshRenderer.enabled = true;
            npc.GetComponent<CutsceneNPCController>().shouldRun = true;
        }

        yield return new WaitForSeconds(2f);
        fadeInOut.SetTrigger("FadeFromBlack");
    }

    IEnumerator WaitToTelePlayer2()
    {
        yield return new WaitForSeconds(2.6f);
        playerRig.transform.position = playerTeleHeli.position;
        playerRig.transform.rotation = playerTeleHeli.rotation;
        playerRig.transform.parent = heli.transform;
        heli.transform.position += new Vector3(0f, 20f, 0f);
        EventManager.TriggerStartHelicopterMove();
        yield return new WaitForSeconds(2f);
        fadeInOut.SetTrigger("FadeFromBlack");
    }

    IEnumerator WaitToEndGame()
    {
        yield return new WaitForSeconds(7.5f);
        fadeInOut.SetTrigger("FadeToBlack");
        yield return new WaitForSeconds(3f);
        EventManager.TriggerStartCredits();
    }

    private void OnDestroy()
    {
        EventManager.StartNextCreditsSequence -= StartNextCreditsSequence;
    }
}
