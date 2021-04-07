using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PoliceStationNPCController : MonoBehaviour, IDamageAble
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private int idleAnimationInt; //MUST be 0 if waypoints is not empty;
    [SerializeField] private bool shouldWalk; //MUST Be false if waypoints is empty
    [SerializeField] private Transform[] waypoints; // leave empty if you dont want NPC to walk
    private float minWalkDelay = 2f;
    private float maxWalkDelay = 6f;
    private float timeToWalk;
    [SerializeField] private Transform retreatPoint; //where they should go for mission 5
    private bool shouldRetreat = false;
    private bool killed = false;

    [SerializeField] private GameObject floatingText;
    [SerializeField] private string[] textsToSay;
    [SerializeField] private string[] retreatText;
    [SerializeField] private float minTimeBetweenDialog;
    [SerializeField] private float maxTimeBetweenDialog;
    private Coroutine randomDialog = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator.SetInteger("Animation_int", idleAnimationInt);
        navMeshAgent.speed = 2f;
        animator.SetFloat("Speed_f", 0f);
        timeToWalk = Time.time + Random.Range(minWalkDelay, maxWalkDelay);
        EventManager.DisableFloodLightSounds += DisableFloodLightSounds; //really shouldn't use this event, but its called when the lights go out in mission 5, so it works.

        randomDialog = StartCoroutine(RandomDialog());


        Rigidbody[] rigidBodies = this.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!killed)
        {
            floatingText.transform.LookAt(PlayerManager.instance.player.transform, Vector3.up);
            floatingText.transform.eulerAngles = new Vector3(0, floatingText.transform.eulerAngles.y, 0);

            if (Vector3.Distance(floatingText.transform.position, PlayerManager.instance.player.transform.position) < 30f)
            {
                floatingText.gameObject.SetActive(true);
            }
            else
            {
                floatingText.gameObject.SetActive(false);
            }

            if (waypoints.Length != 0 && !shouldRetreat)
            {
                if (shouldWalk && timeToWalk < Time.time)
                {
                    shouldWalk = false;
                    animator.SetFloat("Speed_f", 0.49f);
                    StartCoroutine(NavMeshStartDelay());
                }
                if (!shouldWalk && navMeshAgent.remainingDistance <= 0.5f)
                {
                    shouldWalk = true;
                    animator.SetFloat("Speed_f", 0f);
                    timeToWalk = Time.time + Random.Range(minWalkDelay, maxWalkDelay);
                }
            }
        }
    }

    private void DisableFloodLightSounds()
    {
        animator.SetFloat("Speed_f", 0.9f);
        animator.SetInteger("Animation_int", 0);
        navMeshAgent.speed = 5f;
        shouldRetreat = true;
        StopCoroutine(randomDialog);
        floatingText.GetComponentInChildren<TextMesh>().text = retreatText[Random.Range(0, retreatText.Length)];
        StartCoroutine(RetreatNavMesh());
    }

    IEnumerator RetreatNavMesh()
    {
        navMeshAgent.SetDestination(retreatPoint.position);
        yield return new WaitForSeconds(3f);
        navMeshAgent.SetDestination(retreatPoint.position);
        yield return new WaitForSeconds(3f);
        navMeshAgent.SetDestination(retreatPoint.position);
        floatingText.GetComponentInChildren<TextMesh>().text = "";
    }

    IEnumerator NavMeshStartDelay()
    {
        yield return new WaitForSeconds(3.25f);
        navMeshAgent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].position);
    }

    IEnumerator RandomDialog()
    {
        while (true)
        {
            floatingText.GetComponentInChildren<TextMesh>().text = "";
            yield return new WaitForSeconds(Random.Range(minTimeBetweenDialog, maxTimeBetweenDialog));
            floatingText.GetComponentInChildren<TextMesh>().text = textsToSay[Random.Range(0, textsToSay.Length)];
            yield return new WaitForSeconds(6f);
        }
    }

    private void OnDestroy()
    {
        EventManager.DisableFloodLightSounds -= DisableFloodLightSounds;
    }

    public void Damage(float damage, string limbName)
    {
        if (!killed)
        {
            killed = true;
            StopAllCoroutines();
            floatingText.GetComponentInChildren<TextMesh>().text = "";
            EventManager.TriggerNPCKilled();
            animator.enabled = false;
            navMeshAgent.enabled = false;

            Rigidbody[] rigidBodies = this.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rigidBody in rigidBodies)
            {
                rigidBody.isKinematic = false;
                rigidBody.useGravity = true;
            }
        }
    }
}
