using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    [SerializeField] private GameObject mainRotor;
    [SerializeField] private GameObject tailRotor;
    [SerializeField] private Transform endPosition;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip helicopterSound;
    public bool startMove;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartHelicopterMove += StartHelicopterMove;
    }

    private void StartHelicopterMove()
    {
        startMove = true;
        audioSource.clip = helicopterSound;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        mainRotor.transform.localRotation *= Quaternion.AngleAxis(1000 * Time.deltaTime, Vector3.down);
        tailRotor.transform.localRotation *= Quaternion.AngleAxis(2500 * Time.deltaTime, Vector3.right);

        if (startMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition.position, 15f * Time.deltaTime);
            if (Vector3.Distance(transform.position, endPosition.position) < 0.5f)
            {
                transform.position = endPosition.position;
            }
        }
    }

    private void OnDestroy()
    {
        EventManager.StartHelicopterMove -= StartHelicopterMove;
    }
}
