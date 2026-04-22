using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySceneLoader : MonoBehaviour
{
    public void LoadExperience()
    {
        SceneManager.LoadScene("TutorialScene");
    }
}