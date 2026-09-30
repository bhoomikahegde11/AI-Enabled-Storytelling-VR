using UnityEngine;
using System.Collections;

public class BackgroundNPCConversation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Talking Timing")]
    [SerializeField] private float minIdleTime = 4f;
    [SerializeField] private float maxIdleTime = 10f;

    [SerializeField] private float minTalkingTime = 2f;
    [SerializeField] private float maxTalkingTime = 6f;

    private static readonly int IsTalking = Animator.StringToHash("IsTalking");

    private void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        StartCoroutine(RandomConversation());
    }

    private IEnumerator RandomConversation()
    {
        while (true)
        {
            // Stand idle for a random amount of time
            animator.SetBool(IsTalking, false);

            yield return new WaitForSeconds(
                Random.Range(minIdleTime, maxIdleTime)
            );

            // Talk for a random amount of time
            animator.SetBool(IsTalking, true);

            yield return new WaitForSeconds(
                Random.Range(minTalkingTime, maxTalkingTime)
            );
        }
    }
}