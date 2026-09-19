using UnityEngine;
using System.Collections;
public class BagReceiver : MonoBehaviour
{
    public HandBagAnimation customer;
    public ChatManager chatManager;

    private bool completed = false;

    private void Awake()
    {
        if (chatManager == null)
        {
            chatManager = FindFirstObjectByType<ChatManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (completed)
            return;

        Debug.Log(
            "BAG RECEIVED: " +
            other.name
        );

        ScooperFill scooper = ScooperFill.Instance;

        if (scooper == null)
        {
            Debug.Log("Scooper doesn't exist!");
            return;
        }

        Debug.Log("Scooper Instance Found");
        Debug.Log("Scooper Filled: " + scooper.IsFilled());
        Debug.Log("Current Spice: " + scooper.currentSpice);

        bool tutorialModeActive = OrderManager.Instance != null && OrderManager.Instance.tutorialMode;
        bool pendingMarketplaceFulfillment = chatManager != null && chatManager.HasPendingFulfillment;

        if (!tutorialModeActive && !pendingMarketplaceFulfillment)
        {
            Debug.LogWarning("[BagReceiver] Ignoring bag delivery because there is no active tutorial order or accepted marketplace fulfillment.");
            return;
        }

        if (!scooper.IsFilled())
        {
            Debug.Log("Scooper Empty");
            return;
        }
        if (scooper.currentSpice != OrderManager.Instance.requestedSpice)
        {
            Debug.Log("Wrong Spice!");

            if (SpiceTutorialManager.Instance != null)
                SpiceTutorialManager.Instance.NotifyWrongSpiceBroughtToBag();

            OVRInput.SetControllerVibration(
        0.3f,
        0.3f,
        OVRInput.Controller.RTouch
    );

            scooper.EmptyScooper();

            StartCoroutine(StopHaptics());

            return;
        }

        completed = true;

        Debug.Log("Correct Spice");
        OVRInput.SetControllerVibration(
        1f,
        1f,
        OVRInput.Controller.RTouch
    );

        customer.FillBag(scooper.currentSpice);
        scooper.EmptyScooper();

        if (SpiceTutorialManager.Instance != null)
            SpiceTutorialManager.Instance.NotifyCorrectBagFilled();

        if (chatManager != null)
        {
            if (chatManager.HasPendingFulfillment)
            {
                chatManager.CompleteAcceptedFulfillment();
            }
            else
            {
                Debug.LogWarning("[BagReceiver] ChatManager assigned, but there is no pending accepted fulfillment to complete.");
            }
        }

        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.CompleteMarketplaceFulfillment();
        }


        StartCoroutine(StopHaptics());

        
    }
    IEnumerator StopHaptics()
    {
        yield return new WaitForSeconds(0.15f);

        OVRInput.SetControllerVibration(
            0,
            0,
            OVRInput.Controller.RTouch
        );
    }
    public void ResetBag()
    {
        completed = false;
    }
}
