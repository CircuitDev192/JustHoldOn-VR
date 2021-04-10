using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneVehicleController : MonoBehaviour
{
    [SerializeField] private Transform hiddenMover;
    private bool atDest = false;
    private bool atPoint = false;
    [SerializeField] private bool isLeadVehicle = false;
    private CutsceneHiddenMover cutsceneHiddenMover;
    private float mult = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DriveThroughWaypoints());
        StartCoroutine(TurnToFaceWaypoint());
        cutsceneHiddenMover = hiddenMover.gameObject.GetComponent<CutsceneHiddenMover>();
    }

    private void Update()
    {

        float distance = Vector3.Distance(transform.position, hiddenMover.position);

        if (distance >= 10f)
        {
            mult -= 0.01f;
        }
        else if (distance < 10f && distance >= 8.5f)
        {
            //Do Nothing
        }
        else
        {
            mult += 0.01f;
        }

        cutsceneHiddenMover.speed = mult;

    }

    IEnumerator DriveThroughWaypoints()
    {
        while (true)
        { 
            transform.position = Vector3.MoveTowards(transform.position, hiddenMover.position, 2.0f * Time.deltaTime);
            yield return null;
            if (Vector3.Distance(transform.position, hiddenMover.position) < 1f)
            {
                atDest = true;
                if (isLeadVehicle) EventManager.TriggerStartNextCreditsSequence();
                break;
            }
        }
        yield return null;
    }

    IEnumerator TurnToFaceWaypoint()
    {
        while (!atDest)
        {
            Vector3 direction = (hiddenMover.position - transform.position);
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 2f * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
}
