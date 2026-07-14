using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (GameManager.Instance != null)
        {
            Debug.Log($"{nameof(MenuSceneButton)} on '{gameObject.name}' advancing with GameManager scene flow.");
            GameManager.Instance.LoadNextScene();
            return;
        }

        Debug.LogError($"{nameof(MenuSceneButton)} on '{gameObject.name}' could not find GameManager.Instance. Falling back to direct scene load only if sceneName is set.");

        if (!string.IsNullOrWhiteSpace(sceneName))
            LoadSceneInternal();
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

        Debug.Log($"{nameof(MenuSceneButton)} on '{gameObject.name}' loading scene '{sceneName}'.");

        SceneManager.LoadScene(sceneName);
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
