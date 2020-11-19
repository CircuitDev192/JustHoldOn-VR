using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISurvivalTimerController : MonoBehaviour
{
    private Text timerText;
    private Animator fadeToBlackAnim;
    private bool startTimer;
    private bool heliStarted;
    [SerializeField] private float timerValue = 120f;
    [SerializeField] private float timeToStartHeli = 45f;

    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponentInChildren<Text>();
        fadeToBlackAnim = GetComponentInChildren<Animator>();
        EventManager.StartSurvivalCountdown += StartSurvivalCountdown;
        EventManager.GameEnded += GameEnded;
        EventManager.SurvivalMissionFailed += SurvivalMissionFailed;
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

    private void SurvivalMissionFailed()
    {
        if (timerValue > 10f)
        {
            EventManager.TriggerPlayerKilled();
        }
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

    private void OnDestroy()
    {
        EventManager.StartSurvivalCountdown -= StartSurvivalCountdown;
        EventManager.GameEnded -= GameEnded;
        EventManager.SurvivalMissionFailed -= SurvivalMissionFailed;
    }
}
