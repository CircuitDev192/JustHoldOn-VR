using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CutsceneNPCController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent nav;
    public SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Transform navTarget;
    public bool shouldRun = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        meshRenderer.enabled = false;
        nav.speed = Random.Range(3.5f, 5f);
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

        if (Vector3.Distance(navTarget.position, transform.position) < 1f)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator PlayRunAnim()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.2f));
        animator.SetFloat("Speed_f", 0.9f);
    }
}
