using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    Animator animator;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void GiveCoin()
    {
        Debug.Log("Give coin triggered");

        animator.speed = 1f;

        animator.ResetTrigger("GiveCoin");
        animator.SetTrigger("GiveCoin");
    }


    public void FreezeHand()
    {
        Debug.Log("Freeze called");

        // disable for testing
        // animator.speed = 0f;
    }


    public void ResumeAnimation()
    {
        animator.speed = 1f;
    }
}