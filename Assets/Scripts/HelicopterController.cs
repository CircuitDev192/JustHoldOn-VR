using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour, IHeliDamage
{
    [SerializeField] private GameObject mainRotor;
    [SerializeField] private GameObject tailRotor;
    [SerializeField] private GameObject spotlight;
    [SerializeField] private Transform endPosition;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource explosionAudioSource;
    [SerializeField] private AudioClip helicopterSound;
    [SerializeField] private AudioClip[] explosions;
    [SerializeField] private GameObject flarePrefab;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private Transform[] fireSpawnpoints;
    private List<Transform> spawnpoints = new List<Transform>();
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject crashedHeliPrefab;
    private float health = 100f;
    public bool startMove;
    public bool crashing = false;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartHelicopterMove += StartHelicopterMove;

        player = PlayerManager.instance.player.transform;

        foreach(Transform spawn in fireSpawnpoints)
        {
            spawnpoints.Add(spawn);
        }
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

    private void FireFlare()
    {
        //Instantiate flare with random downward velocity
        GameObject flare = Instantiate(flarePrefab, transform.position, Quaternion.identity);
        Vector3 velocity = new Vector3(Random.Range(-1f, 1f), -1f, Random.Range(-1f, 1f));
        flare.gameObject.GetComponent<Rigidbody>().AddForce(velocity.normalized * 30f, ForceMode.Impulse);
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (crashing)
        {
            return;
        }

        if (health <= 0f)
        {
            //Heli destroyed
            crashing = true;
            EventManager.TriggerStopSurvivalCountdown();
            startMove = false;
            spotlight.SetActive(false);
            StartCoroutine(Crash());
            StartCoroutine(Explosions());
        }
        else
        {
            //Set off explosion and fire prefabs
            Transform spawn = spawnpoints[Random.Range(0, spawnpoints.Count - 1)];
            spawnpoints.Remove(spawn);
            Instantiate(explosionPrefab, spawn.position, Quaternion.identity);
            Instantiate(firePrefab, spawn.position, Quaternion.identity);
            explosionAudioSource.PlayOneShot(explosions[Random.Range(0, explosions.Length - 1)], 0.7f);
        }


    }

    IEnumerator Crash()
    {
        //Crash heli into player
        while (Vector3.Distance(transform.position, player.position) > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, 15f * Time.deltaTime);
            transform.localRotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.up); // Hopefully this will work to rotate the helicopter as is falls.
            yield return null;
        }
        audioSource.Stop();
        crashedHeliPrefab.SetActive(true);
        EventManager.TriggerHeliCrashed();
        this.gameObject.SetActive(false);
    }

    IEnumerator Explosions()
    {
        //Play explosion sounds
        while (true)
        {
            explosionAudioSource.PlayOneShot(explosions[Random.Range(0, explosions.Length - 1)], 0.7f);
            FireFlare();
            yield return new WaitForSeconds(Random.Range(1.5f, 3f));
        }
    }
}
