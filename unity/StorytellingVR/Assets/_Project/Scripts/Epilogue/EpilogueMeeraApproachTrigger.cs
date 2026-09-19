using UnityEngine;

public class EpilogueMeeraApproachTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            if (EpilogueStoryManager.Instance != null)
            {
                EpilogueStoryManager.Instance.NotifyMeeraApproached();
            }
        }
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player") || other.transform.root.CompareTag("Player") || other.GetComponentInParent<CharacterController>() != null;
    }
}
