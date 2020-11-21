using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemoZombieSpawner : MonoBehaviour
{
    public static DemoZombieSpawner instance;

    [SerializeField] private int maxLivingZombies = 15;
    [SerializeField] private int maxDeadZombies = 10;
    [SerializeField] private float spawnRadius = 20.0f;

    [SerializeField] private GameObject[] zombiePrefabs;
    private List<GameObject> zombies = new List<GameObject>();
    private Queue<GameObject> deadZombies = new Queue<GameObject>();

    [SerializeField]
    private GameObject[] spawnPoints;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //EventManager.zombieShouldDespawn += DespawnZombie;
        //EventManager.ZombieKilled += ZombieDied;
    }

    private void FixedUpdate()
    {
        HandleSpawning();
    }

    private void DespawnZombie(GameObject zombie)
    {
        zombies.Remove(zombie);
        Destroy(zombie.gameObject);
    }

    private void HandleSpawning()
    {
        //Debug.Log("Zombies: " + zombies.Count.ToString());
        if (zombies.Count >= maxLivingZombies) return;

        int index = 0;
        float xOffset = 0;
        float zOffset = 0;

        index = Random.Range(0, spawnPoints.Length);
        xOffset = Random.Range(-spawnRadius, spawnRadius);
        zOffset = Random.Range(-spawnRadius, spawnRadius);

        Vector3 spawnLoc = spawnPoints[index].transform.position;
        spawnLoc.x += xOffset;
        //spawnLoc.y += 0.5f;
        spawnLoc.z += zOffset;

        //NavMeshHit hitInfo;
        //if (!NavMesh.SamplePosition(spawnLoc, out hitInfo, spawnRadius, NavMesh.AllAreas)) continue;

        //spawnLoc = hitInfo.position;

        if (Physics.CheckBox(spawnLoc, new Vector3(0.25f, 0.5f, 0.25f)))
        {
            Debug.Log("Physics check failed for spawn!");
            return;
        }

        zombies.Add(Instantiate(zombiePrefabs[Random.Range(0, zombiePrefabs.Length)], spawnLoc, Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0)));
        //Debug.Log("Zombie spawned successfully!");
    }

    private void ZombieDied(GameObject zombie)
    {
        zombies.Remove(zombie);

        deadZombies.Enqueue(zombie);

        if (deadZombies.Count > maxDeadZombies) Destroy(deadZombies.Dequeue());
    }

    private void OnDestroy()
    {
        EventManager.zombieShouldDespawn -= DespawnZombie;
        EventManager.ZombieKilled -= ZombieDied;
    }
}
