using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private float activeDistance;
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = PlayerManager.instance.player.transform;
    }

    void FixedUpdate()
    {
        foreach (GameObject tile in tiles)
        {
            float distance = Vector3.Distance(playerTransform.position, tile.transform.position);

            if (distance > activeDistance) tile.SetActive(false);

            else tile.SetActive(true);
        }
    }
}
