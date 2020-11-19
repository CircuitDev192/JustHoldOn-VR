using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlashbangController : MonoBehaviour
{
    [SerializeField] private Image blindnessEffect;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.FlashbangDetonated += FlashbangDetonated;
        player = PlayerManager.instance.player.transform;
    }

    private void FlashbangDetonated(Vector3 flashbangPosition, float stunDistance)
    {
        if (Vector3.Distance(flashbangPosition, player.position) <= stunDistance)
        {
            blindnessEffect.GetComponent<Animator>().SetTrigger("FlashbangDetonated");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventManager.FlashbangDetonated -= FlashbangDetonated;
    }
}
