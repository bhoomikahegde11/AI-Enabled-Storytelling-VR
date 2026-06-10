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
        Debug.Log("GiveCoin called");

        animator.speed = 1f;
        animator.SetTrigger("GiveCoin");
    }


    // Animation Event calls this
    public void FreezeHand()
    {
        Debug.Log("Freeze called");

        animator.speed = 0f;

        if (handCoin != null)
            handCoin.SetActive(true);
    }


    public void ResumeAnimation()
    {
        animator.speed = 1f;
    }
}