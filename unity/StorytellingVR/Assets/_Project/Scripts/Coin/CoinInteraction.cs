using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinInteraction : MonoBehaviour
{
    [Header("Scene")]
    public CoinSceneManager sceneManager; 
    
    [Header("References")]
    public NPCAnimationController npc;
    public Transform inspectPoint;
    public CoinSequenceManager sequenceManager;

    [Header("Info")]
    public CoinInfoManager infoManager;

    public string coinName;
    public string coinType;

    [TextArea]
    public string coinDescription;


    [Header("Inspect Settings")]
    public Vector3 inspectScale =
        new Vector3(
            0.3548979f,
            0.02304457f,
            0.3307208f
        );

    public float transitionDuration = 1.2f;


    [HideInInspector]
    public bool isInspecting = false;


    private bool taken = false;

    private Collider coinCollider;


    void Awake()
    {
        coinCollider =
            GetComponent<Collider>();


        VRInspectRotate rotate =
            GetComponent<VRInspectRotate>();

        if (rotate != null)
            rotate.enabled = false;
    }


    void Update()
    {
        // right trigger
        if (
            OVRInput.GetDown(
            OVRInput.Button.SecondaryIndexTrigger)
        )
        {
            if (!taken)
            {
                TakeCoin();
            }
        }
    }


    public void TakeCoin()
    {
        taken = true;

        Debug.Log("Coin Taken");


        transform.SetParent(null);


        if (npc != null)
            npc.ResumeAnimation();


        if (coinCollider != null)
            coinCollider.enabled = false;


        StartCoroutine(
            MoveToInspectMode()
        );
    }



    IEnumerator MoveToInspectMode()
    {
        Vector3 startPos =
            transform.position;


        Quaternion startRot =
            transform.rotation;


        Vector3 startScale =
            transform.localScale;


        float elapsed = 0;


        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;


            float t =
                elapsed / transitionDuration;


            t = Mathf.SmoothStep(
                0,
                1,
                t
            );


            Vector3 pos =
                Vector3.Lerp(
                    startPos,
                    inspectPoint.position,
                    t
                );


            pos +=
                Vector3.up *
                Mathf.Sin(t * Mathf.PI)
                * 0.15f;


            transform.position = pos;


            transform.rotation =
                Quaternion.Slerp(
                    startRot,
                    inspectPoint.rotation,
                    t
                );


            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    inspectScale,
                    t
                );


            yield return null;
        }



        transform.position =
            inspectPoint.position;

        transform.rotation =
            inspectPoint.rotation;

        transform.localScale =
            inspectScale;



        if (coinCollider != null)
            coinCollider.enabled = true;



        VRInspectRotate rotate =
            GetComponent<VRInspectRotate>();

        if (rotate != null)
            rotate.enabled = true;



        if (infoManager != null)
        {
            infoManager.ShowInfo(
                coinName,
                coinType,
                coinDescription
            );
        }


        isInspecting = true;
        if (sceneManager != null)
        {
            sceneManager.NarrateVaraha();
        }

        if (sequenceManager != null)
        {
            sequenceManager.StartSequence();
        }


        Debug.Log(
            "Inspect Started"
        );
    }
}