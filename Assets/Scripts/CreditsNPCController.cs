using UnityEngine;

public class CreditsNPCController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private int idleAnimationInt; //MUST be 0 if waypoints is not empty;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("Animation_int", idleAnimationInt);
    }
}
