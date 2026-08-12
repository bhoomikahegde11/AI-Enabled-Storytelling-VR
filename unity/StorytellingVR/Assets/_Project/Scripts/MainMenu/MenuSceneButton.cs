using UnityEngine;

public class MenuSceneButton : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneName;
    [Header("Save Flow")]
    [SerializeField] private MainMenuSaveFlow mainMenuSaveFlow;

    public void BeginJourney()
    {
        if (mainMenuSaveFlow != null)
        {
            Debug.Log($"{nameof(MenuSceneButton)} on '{gameObject.name}' opening save select flow.");
            mainMenuSaveFlow.OpenSaveSelect();
            return;
        }

        PlayMenuClick();
        Debug.Log($"{nameof(MenuSceneButton)} on '{gameObject.name}' advancing with GameManager scene flow.");
        GameManager.Instance.LoadNextScene();
    }

    public void LoadScene()
    {
        PlayMenuClick();
        LoadSceneInternal();
    }

    public void QuitGame()
    {
        PlayMenuClick();
        QuitGameInternal();
    }

    private void LoadSceneInternal()
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"{nameof(MenuSceneButton)} on '{gameObject.name}' cannot load a scene because sceneName is empty.");
            return;
        }

        Debug.Log($"{nameof(MenuSceneButton)} on '{gameObject.name}' requesting GameManager load scene '{sceneName}'.");
        GameManager.Instance.LoadSceneByName(sceneName);
    }

    private void QuitGameInternal()
    {
        Debug.Log($"{nameof(MenuSceneButton)} on '{gameObject.name}' quitting the application.");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private static void PlayMenuClick()
    {
        MainMenuAudioController.Instance?.PlayClick();
    }
}
