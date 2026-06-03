using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    private Animator animator;

    [Header("Coin")]
    public GameObject handCoin;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (handCoin != null)
            handCoin.SetActive(false);
    }

    public void GiveCoin()
    {
        animator.speed = 1f;
        animator.SetTrigger("GiveCoin");
    }

    // Called by animation event
    public void FreezeHand()
    {
        animator.speed = 0f;

        if (handCoin != null)
            handCoin.SetActive(true);
    }

    public void ResumeAnimation()
    {
        animator.speed = 1f;
    }
}