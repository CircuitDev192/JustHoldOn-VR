using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSequenceManager : MonoBehaviour
{
    [SerializeField] private GameObject vehicles;
    [SerializeField] private GameObject[] npcs;
    [SerializeField] private GameObject helicopter;
    private Camera cam;
    [SerializeField] private Transform camStartPos2; //where the cam should go for scene 2
    [SerializeField] private Vector3 cameraStartAngle2; //rotation for scene 2
    [SerializeField] private Vector3 cameraStartAngle3; //rotation for scene 3
    [SerializeField] private Transform camStartPos3; //where the cam should go for scene 3
    [SerializeField] private float creditsWaitTime;
    private int currentCreditsSequence = 0;
    private bool moveCam = true;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        EventManager.StartNextCreditsSequence += StartNextCreditsSequence;
        StartCoroutine(EndGameSequence());
    }

    private void StartNextCreditsSequence()
    {
        currentCreditsSequence++;
        switch (currentCreditsSequence)
        {
            case 1:
                //set camera to next pos, trigger event to start the next movement
                Destroy(vehicles.gameObject, 1f);
                cam.transform.position = camStartPos2.position;
                cam.transform.eulerAngles = cameraStartAngle2;
                StartCoroutine(MoveCamera());
                foreach (GameObject npc in npcs)
                {
                    npc.GetComponent<CutsceneNPCController>().shouldRun = true;
                }
                break;
            case 2:
                moveCam = false;
                helicopter.transform.position = new Vector3(helicopter.transform.position.x, helicopter.transform.position.y + 20, helicopter.transform.position.z);
                helicopter.GetComponentInChildren<HelicopterController>().startMove = true;
                cam.transform.position = camStartPos3.position;
                cam.transform.eulerAngles = cameraStartAngle3;
                foreach (GameObject npc in npcs)
                {
                    Destroy(npc.gameObject);
                }
                break;
            case 3:
                //trigger a hold black, then set scene back to main menu
                EventManager.TriggerCreditsUI();
                StartCoroutine(WaitForCredits());
                break;
            default:
                break;
        }
    }

    IEnumerator MoveCamera()
    {
        while (moveCam)
        {
            cam.transform.Translate(Vector3.forward * 6f * Time.deltaTime);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForCredits()
    {
        yield return new WaitForSeconds(creditsWaitTime);
        SceneManager.LoadScene(0);
    }

    IEnumerator EndGameSequence()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(7f);
            EventManager.TriggerFadeToBlack();
            yield return new WaitForSeconds(3f);
            EventManager.TriggerStartNextCreditsSequence();
        }
    }

    private void OnDestroy()
    {
        EventManager.StartNextCreditsSequence -= StartNextCreditsSequence;
    }
}
