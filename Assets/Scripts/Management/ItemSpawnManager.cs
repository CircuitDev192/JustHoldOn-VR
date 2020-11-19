using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints; //Order of spawns must match the spawnables
    [SerializeField] private GameObject[] spawnables;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.EndMission += EndMission;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EndMission()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Instantiate(spawnables[i], spawnPoints[i].position, Quaternion.identity, this.transform);
        }
    }

    private void OnDestroy()
    {
        EventManager.EndMission -= EndMission;
    }
}
