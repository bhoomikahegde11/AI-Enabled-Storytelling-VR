using System.Collections;
using UnityEngine;

public class CoinSequenceManager : MonoBehaviour
{
    [Header("Scene")]
    public CoinSceneManager sceneManager;

    [Header("Coins")]
    public GameObject[] coins;

    [Header("Info")]
    public CoinInfoManager infoManager;

    [Header("Instruction")]
    public InstructionPromptManager instructionPrompt;

    [Header("Coin Data")]
    public string[] names;
    public string[] types;

    [TextArea]
    public string[] descriptions;

    [Header("Input")]
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public OVRInput.Button continueButton = OVRInput.Button.PrimaryIndexTrigger;

    [Header("Transition")]
    public float transitionSpeed = 2f;

    private int index = 0;
    private bool sequenceStarted = false;
    private bool switching = false;
    private bool waitingForRelease = false;

    public System.Action OnCoinSequenceFinished;

    void Start()
    {
        for (int i = 1; i < coins.Length; i++)
        {
            coins[i].SetActive(false);
        }
    }

    void Update()
    {
        if (!sequenceStarted || switching)
            return;

        // Prevent same trigger press that started inspect from also immediately continuing.
        if (waitingForRelease)
        {
            if (!OVRInput.Get(continueButton, controller))
            {
                waitingForRelease = false;
            }

            return;
        }

        if (OVRInput.GetDown(continueButton, controller))
        {
            Next();
        }
    }

    public void StartSequence()
    {
        sequenceStarted = true;
        waitingForRelease = true;

        if (instructionPrompt != null)
        {
            instructionPrompt.ShowTrigger(
                "Press right trigger to continue"
            );
        }
    }

    void Next()
    {
        if (index >= coins.Length - 1)
        {
            FinishSequence();
            return;
        }

        StartCoroutine(SwitchCoin());
    }

    IEnumerator SwitchCoin()
    {
        switching = true;

        GameObject oldCoin = coins[index];
        Vector3 targetScale = oldCoin.transform.localScale;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;

            oldCoin.transform.localScale =
                Vector3.Lerp(
                    targetScale,
                    Vector3.zero,
                    t
                );

            yield return null;
        }

        oldCoin.SetActive(false);

        index++;

        GameObject newCoin = coins[index];

        newCoin.SetActive(true);

        newCoin.transform.position =
            oldCoin.transform.position;

        newCoin.transform.rotation =
            oldCoin.transform.rotation;

        newCoin.transform.localScale =
            Vector3.zero;

        if (infoManager != null)
        {
            infoManager.ShowInfo(
                names[index],
                types[index],
                descriptions[index]
            );
        }

        if (sceneManager != null)
        {
            sceneManager.NarrateKasu();
        }

        VRInspectRotate rotate =
            newCoin.GetComponent<VRInspectRotate>();

        if (rotate != null)
        {
            rotate.enabled = true;
        }

        t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;

            newCoin.transform.localScale =
                Vector3.Lerp(
                    Vector3.zero,
                    targetScale,
                    t
                );

            yield return null;
        }

        switching = false;
        waitingForRelease = true;

        if (instructionPrompt != null)
        {
            instructionPrompt.ShowTrigger(
                "Press right trigger to continue"
            );
        }
    }

    void FinishSequence()
    {
        Debug.Log("Coin tutorial finished");

        coins[index].SetActive(false);

        if (infoManager != null)
        {
            infoManager.Hide();
        }

        sequenceStarted = false;

        if (instructionPrompt != null)
        {
            instructionPrompt.Hide();
        }

        if (OnCoinSequenceFinished != null)
        {
            OnCoinSequenceFinished();
        }
    }
}