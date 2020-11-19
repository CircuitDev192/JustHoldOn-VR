using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CutsceneNPCController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent nav;
    [SerializeField] private Transform navTarget;
    public bool shouldRun = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        nav.speed = 6f;
        StartCoroutine(PlayRunAnim());
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldRun)
        {
            nav.SetDestination(navTarget.position);
            shouldRun = false;
        }
    }

    IEnumerator PlayRunAnim()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.2f));
        animator.SetFloat("Speed_f", 0.9f);
    }
}
