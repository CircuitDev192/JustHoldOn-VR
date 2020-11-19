using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    private bool open = false;
    private bool close = false;
    [SerializeField] private Transform openTransform;
    [SerializeField] private Transform closedTransform;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.InstantiateNextMission += InstantiateNextMission;
        EventManager.PlayerSpokeToMissionGiver += PlayerSpokeToMissionGiver;
    }

    private void PlayerSpokeToMissionGiver(string obj)
    {
        close = true;
    }

    private void InstantiateNextMission()
    {
        open = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, openTransform.position, 5f * Time.deltaTime);
            if (Vector3.Distance(this.transform.position, openTransform.position) <= 0.05)
            {
                this.transform.position = openTransform.position;
                open = false;
            }
        }
        if (close)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, closedTransform.position, 5f * Time.deltaTime);
            if (Vector3.Distance(this.transform.position, closedTransform.position) <= 0.05)
            {
                this.transform.position = closedTransform.position;
                close = false;
            }
        }
    }

    private void OnDestroy()
    {
        EventManager.InstantiateNextMission -= InstantiateNextMission;
        EventManager.PlayerSpokeToMissionGiver -= PlayerSpokeToMissionGiver;
    }
}
