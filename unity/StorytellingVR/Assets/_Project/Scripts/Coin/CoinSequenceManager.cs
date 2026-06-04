using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinSequenceManager : MonoBehaviour
{
    [Header("Scene")]
    public CoinSceneManager sceneManager;

    [Header("Coins")]
    public GameObject[] coins;

    [Header("Info")]
    public CoinInfoManager infoManager;

    [Header("Coin Data")]
    public string[] names;
    public string[] types;

    [TextArea]
    public string[] descriptions;


    [Header("Transition")]
    public float transitionSpeed = 2f;


    private int index = 0;

    private bool sequenceStarted = false;
    private bool switching = false;


    // tells CoinSceneManager tutorial is finished
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
        if (
            sequenceStarted &&
            !switching &&
            OVRInput.GetDown(
                OVRInput.Button.One)
        )
        {
            Next();
        }
    }



    public void StartSequence()
    {
        sequenceStarted = true;
    }



    void Next()
    {
        // already on last coin
        if (index >= coins.Length - 1)
        {
            FinishSequence();
            return;
        }


        StartCoroutine(
            SwitchCoin()
        );
    }



    IEnumerator SwitchCoin()
    {
        switching = true;


        GameObject oldCoin =
            coins[index];


        Vector3 targetScale =
            oldCoin.transform.localScale;


        float t = 0;


        // shrink old coin
        while (t < 1)
        {
            t +=
            Time.deltaTime *
            transitionSpeed;


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



        GameObject newCoin =
            coins[index];


        newCoin.SetActive(true);


        // same position as old coin
        newCoin.transform.position =
            oldCoin.transform.position;


        newCoin.transform.rotation =
            oldCoin.transform.rotation;


        newCoin.transform.localScale =
            Vector3.zero;



        // update info card
        infoManager.ShowInfo(
            names[index],
            types[index],
            descriptions[index]
        );

        if (sceneManager != null)
        {
            sceneManager.NarrateKasu();
        }

        // enable joystick rotate
        VRInspectRotate rotate =
            newCoin.GetComponent<VRInspectRotate>();

        if (rotate != null)
        {
            rotate.enabled = true;
        }



        t = 0;


        // grow new coin
        while (t < 1)
        {
            t +=
            Time.deltaTime *
            transitionSpeed;


            newCoin.transform.localScale =
                Vector3.Lerp(
                    Vector3.zero,
                    targetScale,
                    t
                );


            yield return null;
        }


        switching = false;
    }



    void FinishSequence()
    {
        Debug.Log(
            "Coin tutorial finished"
        );


        coins[index]
            .SetActive(false);


        infoManager.Hide();


        sequenceStarted = false;


        if (OnCoinSequenceFinished != null)
        {
            OnCoinSequenceFinished();
        }
    }
}