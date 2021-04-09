using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawnManager : MonoBehaviour
{
    public static ZombieSpawnManager instance;

    [SerializeField] private int maxLivingZombies = 50;
    [SerializeField] private int maxDeadZombies = 15;
    [SerializeField] private float spawnRadius = 20.0f;
    [SerializeField] private float minSpawnDistance = 50.0f;
    [SerializeField] private float maxSpawnDistance = 200.0f;

    [SerializeField] private GameObject[] zombiePrefabs;
    private List<GameObject> zombies = new List<GameObject>();
    private List<GameObject> missionZombies = new List<GameObject>();
    private Queue<GameObject> deadZombies = new Queue<GameObject>();

    [SerializeField]
    private GameObject[] spawnPoints;

    [SerializeField]
    private GameObject player;

    private GameObject[] missionSpawnPoints;
    private int missionZombiesToSpawn;
    private bool shouldSpawnMissionZombies = false;
    private bool infiniteSpawns = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = PlayerManager.instance.player;
        EventManager.zombieShouldDespawn += DespawnZombie;
        EventManager.ZombieKilled += ZombieDied;
        EventManager.HeliCrashed += HeliCrashed;
    }


    private void FixedUpdate()
    {
        if (shouldSpawnMissionZombies)
        {
            HandleMissionSpawning();
        }
        else
        {
            HandleSpawning();
        }
    }

    public List<GameObject> GetMissionZombies()
    {
        return missionZombies;
    }

    private void DespawnZombie(GameObject zombie)
    {
        if(!zombies.Remove(zombie)) missionZombies.Remove(zombie);
        Destroy(zombie.gameObject);
    }

    private void HandleSpawning()
    {
        //Debug.Log("Zombies: " + zombies.Count.ToString());
        if (zombies.Count >= maxLivingZombies) return;

        List<GameObject> validSpawns = new List<GameObject>();

        foreach (GameObject spawnPoint in spawnPoints)
        {
            float dist = Vector3.Distance(player.transform.position, spawnPoint.transform.position);

            if (dist > minSpawnDistance && dist < maxSpawnDistance) validSpawns.Add(spawnPoint);
        }

        if (validSpawns.Count == 0)
        {
            //Debug.Log("No valid spawns found!");
            return;
        }

        int index = 0;
        float xOffset = 0;
        float zOffset = 0;


        index = UnityEngine.Random.Range(0, validSpawns.Count);
        xOffset = UnityEngine.Random.Range(-spawnRadius, spawnRadius);
        zOffset = UnityEngine.Random.Range(-spawnRadius, spawnRadius);

        Vector3 spawnLoc = validSpawns[index].transform.position;
        spawnLoc.x += xOffset;
        //spawnLoc.y += 0.5f;
        spawnLoc.z += zOffset;

        //NavMeshHit hitInfo;
        //if (!NavMesh.SamplePosition(spawnLoc, out hitInfo, spawnRadius, NavMesh.AllAreas)) continue;

        //spawnLoc = hitInfo.position;

        if (Physics.CheckBox(spawnLoc, new Vector3(0.25f, 0.5f, 0.25f)))
        {
            //Debug.Log("Physics check failed for spawn!");
            return;
        }

        zombies.Add(Instantiate(zombiePrefabs[UnityEngine.Random.Range(0, zombiePrefabs.Length)], spawnLoc, Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0)));
        //Debug.Log("Zombie spawned successfully!");
    }

    public void SetMissionZombieSpawns(GameObject[] missionSpawnPoints, int maxZombies, bool shouldSpawnZombies, bool infiniteSpawns)
    {
        this.missionSpawnPoints = missionSpawnPoints;
        missionZombiesToSpawn = maxZombies;
        shouldSpawnMissionZombies = shouldSpawnZombies;
        this.infiniteSpawns = infiniteSpawns;
    }

    public void StopMissionZombieSpawns()
    {
        shouldSpawnMissionZombies = false;
    }

    private void HandleMissionSpawning()
    {
        
        if (missionZombies.Count >= missionZombiesToSpawn)
        {
            if (!infiniteSpawns)
            {
                shouldSpawnMissionZombies = false;
            }
            return;
        }

        int index = 0;
        float xOffset = 0;
        float zOffset = 0;


        index = UnityEngine.Random.Range(0, missionSpawnPoints.Length);
        xOffset = UnityEngine.Random.Range(-spawnRadius, spawnRadius);
        zOffset = UnityEngine.Random.Range(-spawnRadius, spawnRadius);

        Vector3 spawnLoc = missionSpawnPoints[index].transform.position;
        spawnLoc.x += xOffset;
        //spawnLoc.y += 0.5f;
        spawnLoc.z += zOffset;

        //NavMeshHit hitInfo;
        //if (!NavMesh.SamplePosition(spawnLoc, out hitInfo, spawnRadius, NavMesh.AllAreas)) continue;

        //spawnLoc = hitInfo.position;

        //if (Physics.CheckBox(spawnLoc, new Vector3(0.25f, 0.5f, 0.25f)))
        //{
        // Debug.LogError("Physics check failed for spawn!");
        // return;
        //}
        GameObject missionZombie = Instantiate(zombiePrefabs[UnityEngine.Random.Range(0, zombiePrefabs.Length)], spawnLoc, Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
        missionZombies.Add(missionZombie);
        EventManager.TriggerMissionZombieSpawned(missionZombie);
        //Just to be sure
        Debug.LogWarning("Mission Zombie spawned.");
    }

    private void HeliCrashed()
    {
        StopMissionZombieSpawns();
    }

    private void ZombieDied(GameObject zombie)
    {
        if (!zombies.Remove(zombie)) missionZombies.Remove(zombie);

        deadZombies.Enqueue(zombie);

        if (deadZombies.Count > maxDeadZombies) Destroy(deadZombies.Dequeue());
    }

    private void OnDestroy()
    {
        EventManager.zombieShouldDespawn -= DespawnZombie;
        EventManager.ZombieKilled -= ZombieDied;
        EventManager.HeliCrashed -= HeliCrashed;
    }
}
