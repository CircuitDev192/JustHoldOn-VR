using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaypointController : MonoBehaviour
{
    private Image waypointIcon;
    private Text distanceText;

    private RectTransform rectTransform;
    private Transform player;
    private Vector3 target;
    private Camera cam;

    [SerializeField] private bool waypointEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        target = Vector3.zero;
        rectTransform = GetComponent<RectTransform>();
        waypointIcon = GetComponent<Image>();
        distanceText = transform.parent.transform.GetComponentInChildren<Text>();
        player = PlayerManager.instance.player.transform;
        EventManager.MissionWaypointChanged += MissionWaypointChanged;
        EventManager.EndMission += EndMission;
        EventManager.StartMission += StartMission;
        EventManager.PlayerCameraChanged += PlayerCameraChanged;
        cam = Camera.main;
    }

    private void PlayerCameraChanged(Camera newCamera)
    {
        cam = newCamera;
    }

    private void EndMission()
    {
        waypointEnabled = false;
    }

    private void StartMission()
    {
        waypointEnabled = true;
        ToggleUI(true);
    }

    private void MissionWaypointChanged(Vector3 missionWaypointPosition)
    {
        target = missionWaypointPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            waypointEnabled = !waypointEnabled;
        }
        
        if (waypointEnabled)
        {
            GetDistance();
            CheckOnScreen();
        }
        else
        {
            ToggleUI(false);
        }
    }
    
    void GetDistance()
    {
        float distance = Vector3.Distance(player.position, target);
        distanceText.text = distance.ToString("f1") + "m";
    }

    void CheckOnScreen()
    {
        //float angle = Vector3.Dot((target - cam.transform.position).normalized, cam.transform.forward);
        
        //if (angle <= 0f)
        //{
        //    ToggleUI(false);
        //}
        //else
        //{
        ToggleUI(true);

        Vector3 playerLook = new Vector3(0, cam.transform.eulerAngles.y, 0);
        Debug.LogError("playerlook: " + playerLook);
        Vector3 playerToObjective = new Vector3(0, (target - cam.transform.position).y, 0);
        Debug.LogError("playertoObj: " + playerToObjective);
        float sign = (playerToObjective.y < playerLook.y) ? -1.0f : 1.0f;
        Debug.LogError("sign: " + sign);
        float zAngle = Vector3.Angle(playerToObjective, playerLook) * sign;
        Debug.LogError("zangle: " + zAngle);
        rectTransform.eulerAngles = new Vector3(180, 0, zAngle);
        //}
    }

    void ToggleUI(bool uiEnable)
    {
        waypointIcon.enabled = uiEnable;
        distanceText.enabled = uiEnable;
    }

    private void OnDestroy()
    {
        EventManager.MissionWaypointChanged -= MissionWaypointChanged;
        EventManager.PlayerCameraChanged -= PlayerCameraChanged;
        EventManager.EndMission -= EndMission;
        EventManager.StartMission -= StartMission;
    }
}
