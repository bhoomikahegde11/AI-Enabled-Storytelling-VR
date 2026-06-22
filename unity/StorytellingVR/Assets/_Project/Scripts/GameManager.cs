using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [Header("Scene Order")]
    public string[] scenes;


    private int currentIndex = -1;


    public ScreenFader fader;
    [Header("Skip")]
    public float skipHoldDuration = 1f;

    private float skipHoldTimer = 0f;
    private bool isLoading = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;

        DontDestroyOnLoad(
            gameObject
        );

        Debug.Log("[SCENE FLOW] Bootstrap loaded");
    }


    void Start()
    {
        LoadNextScene();
    }

    void Update()
    {
        InputDevice leftHand =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        InputDevice rightHand =
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool xPressed = false;
        bool aPressed = false;

        // Left controller X button
        leftHand.TryGetFeatureValue(
            CommonUsages.primaryButton,
            out xPressed
        );

        // Right controller A button
        rightHand.TryGetFeatureValue(
            CommonUsages.primaryButton,
            out aPressed
        );

        if (xPressed && aPressed)
        {
            skipHoldTimer += Time.deltaTime;

            if (skipHoldTimer >= skipHoldDuration)
            {
                skipHoldTimer = 0f;
                SkipScene();
            }
        }
        else
        {
            skipHoldTimer = 0f;
        }
    }
    public void LoadNextScene()
    {
        if (isLoading)
            return;

        StartCoroutine(
            LoadRoutine()
        );
    }

    public void SkipScene()
    {
        if (isLoading)
            return;

        Debug.Log("[SCENE FLOW] Scene skipped");

        LoadNextScene();
    }

    IEnumerator LoadRoutine()
    {
        isLoading = true;
        if (fader != null)
            yield return fader.FadeOut();


        currentIndex++;


        if (currentIndex >= scenes.Length)
        {
            Debug.Log(
                "GAME COMPLETE"
            );

            isLoading = false;

            yield break;
        }

        Debug.Log("[SCENE FLOW] Loading " + scenes[currentIndex]);

        yield return SceneManager.LoadSceneAsync(
            scenes[currentIndex]
        );

        Debug.Log("[SCENE FLOW] " + scenes[currentIndex] + " loaded");

        if (fader != null)
            yield return fader.FadeIn();
    }
}