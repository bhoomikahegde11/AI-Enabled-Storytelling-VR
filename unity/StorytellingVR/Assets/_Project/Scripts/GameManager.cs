using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [Header("Scene Order")]
    public string[] scenes;


    private int currentIndex = -1;


    public ScreenFader fader;


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
    }


    void Start()
    {
        LoadNextScene();
    }


    public void LoadNextScene()
    {
        StartCoroutine(
            LoadRoutine()
        );
    }



    IEnumerator LoadRoutine()
    {
        if (fader != null)
            yield return fader.FadeOut();


        currentIndex++;


        if (currentIndex >= scenes.Length)
        {
            Debug.Log(
                "GAME COMPLETE"
            );

            yield break;
        }


        yield return SceneManager.LoadSceneAsync(
            scenes[currentIndex]
        );


        if (fader != null)
            yield return fader.FadeIn();
    }
}