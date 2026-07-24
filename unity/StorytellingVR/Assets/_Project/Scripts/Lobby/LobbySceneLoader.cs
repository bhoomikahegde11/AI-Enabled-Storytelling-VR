using UnityEngine;

public class LobbySceneLoader : MonoBehaviour
{
    public void LoadExperience()
    {
        GameManager.Instance.LoadSceneByName("TutorialScene");
    }
}
