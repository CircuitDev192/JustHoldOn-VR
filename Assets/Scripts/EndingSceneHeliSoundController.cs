using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSceneHeliSoundController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartHelicopterMove += StartHelicopterMove;
    }

    private void StartHelicopterMove()
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        EventManager.StartHelicopterMove -= StartHelicopterMove;
    }
}
