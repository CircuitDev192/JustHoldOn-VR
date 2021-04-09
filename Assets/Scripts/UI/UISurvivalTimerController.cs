using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISurvivalTimerController : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private Image cutToBlack;
    [SerializeField] private Animator cutToBlackAnim;
    [SerializeField] private Animator fadeToBlackAnim;
    private bool startTimer;
    private bool heliStarted;
    [SerializeField] private float timerValue = 120f;
    [SerializeField] private float timeToStartHeli = 45f;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartSurvivalCountdown += StartSurvivalCountdown;
        EventManager.StopSurvivalCountdown += StopSurvivalCountdown;
        EventManager.GameEnded += GameEnded;
        EventManager.SurvivalMissionFailed += SurvivalMissionFailed;
        EventManager.HeliCrashed += HeliCrashed;
    }

    private void GameEnded()
    {
        fadeToBlackAnim.SetTrigger("FadeToBlack");
    }

    private void StartSurvivalCountdown()
    {
        timerText.enabled = true;
        startTimer = true;
    }

    private void StopSurvivalCountdown()
    {
        timerText.enabled = false;
        startTimer = false;
    }

    private void SurvivalMissionFailed()
    {
        if (timerValue > 10f)
        {
            EventManager.TriggerPlayerKilled();
        }
    }

    private void HeliCrashed()
    {
        cutToBlack.gameObject.SetActive(true);
        StartCoroutine(FadeBackIn());
    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {
            timerValue -= Time.deltaTime;
            float time = timerValue;
            //update timerText
            int seconds = (int)(time % 60);
            time /= 60;
            int minutes = (int)(time % 60);

            timerText.text = string.Format("Time left:  {0}:{1}", minutes.ToString("0"), seconds.ToString("00"));

            if (timerValue < timeToStartHeli && !heliStarted)
            {
                EventManager.TriggerStartHelicopterMove();
                heliStarted = true;
            }

            if (timerValue <= 0f)
            {
                EventManager.TriggerEndMission();
                startTimer = false;
            }
        }
    }

    IEnumerator FadeBackIn()
    {
        yield return new WaitForSeconds(3f);
        cutToBlackAnim.SetTrigger("Start");
        yield return new WaitForSeconds(5f);
        EventManager.TriggerNPCKilled();
    }

    private void OnDestroy()
    {
        EventManager.StartSurvivalCountdown -= StartSurvivalCountdown;
        EventManager.StopSurvivalCountdown -= StopSurvivalCountdown;
        EventManager.GameEnded -= GameEnded;
        EventManager.SurvivalMissionFailed -= SurvivalMissionFailed;
        EventManager.HeliCrashed -= HeliCrashed;
    }
}
