using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneHiddenMover : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    public float speed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DriveThroughWaypoints());
    }

    IEnumerator DriveThroughWaypoints()
    {
        foreach (Transform point in waypoints)
        {
            while (true)
            {
                transform.position = Vector3.MoveTowards(transform.position, point.position, speed * Time.deltaTime);
                yield return null;
                if (Vector3.Distance(transform.position, point.position) < 1f)
                {
                    break;
                }
            }
        }
        yield return null;
    }
}
