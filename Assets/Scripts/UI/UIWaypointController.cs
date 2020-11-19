using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaypointController : MonoBehaviour
{
    private Image waypointIcon;
    private Text distanceText;

    private Transform player;
    private Vector3 target;
    private Camera cam;

    [SerializeField] private bool waypointEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        waypointIcon = GetComponent<Image>();
        distanceText = GetComponentInChildren<Text>();
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
        float angle = Vector3.Dot((target - cam.transform.position).normalized, cam.transform.forward);

        if (angle <= 0f)
        {
            ToggleUI(false);
        }
        else
        {
            ToggleUI(true);
            transform.position = cam.WorldToScreenPoint(target);
        }
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
