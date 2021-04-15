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

    public float nextFootstepTime;
    public AudioClip[] runningSounds;
    public AudioSource audioSource;

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
            PlayFootStepSound();
            shouldRun = false;
        }

        if (Vector3.Distance(navTarget.position, transform.position) < 1f)
        {
            Destroy(this.gameObject);
        }
    }

    public void PlayFootStepSound()
    {
        if (nextFootstepTime > Time.time) return;

        if (animator.GetFloat("Speed_f") <= 0.1f) return;

        PlaySound(runningSounds);

        nextFootstepTime = Time.time + Mathf.Clamp(1.5f / this.nav.speed, 0.4f, 1f);
    }

    public void PlaySound(AudioClip[] clipArray)
    {
        int index = Random.Range(0, clipArray.Length);
        AudioClip clipToPlay = clipArray[index];

        audioSource.clip = clipToPlay;
        audioSource.pitch = Random.Range(0.75F, 1.25F);
        audioSource.Play();
    }

    IEnumerator PlayRunAnim()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.2f));
        animator.SetFloat("Speed_f", 0.9f);
    }
}
