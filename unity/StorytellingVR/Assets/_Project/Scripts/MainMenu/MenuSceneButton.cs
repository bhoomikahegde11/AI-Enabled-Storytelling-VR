using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneButton : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneName;

    public void BeginJourney()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log($"{nameof(MenuSceneButton)} on '{gameObject.name}' advancing with GameManager scene flow.");
            GameManager.Instance.LoadNextScene();
            return;
        }

        Debug.LogError($"{nameof(MenuSceneButton)} on '{gameObject.name}' could not find GameManager.Instance. Falling back to direct scene load only if sceneName is set.");

        if (!string.IsNullOrWhiteSpace(sceneName))
            LoadScene();
    }

    public void LoadScene()
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"{nameof(MenuSceneButton)} on '{gameObject.name}' cannot load a scene because sceneName is empty.");
            return;
        }

        Debug.Log($"{nameof(MenuSceneButton)} on '{gameObject.name}' loading scene '{sceneName}'.");

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log($"{nameof(MenuSceneButton)} on '{gameObject.name}' quitting the application.");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
