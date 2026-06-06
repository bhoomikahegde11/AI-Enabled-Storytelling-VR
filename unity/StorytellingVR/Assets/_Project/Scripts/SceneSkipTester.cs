using UnityEngine;

public class SceneSkipTester : MonoBehaviour
{
    void Update()
    {
        // Press N for Next Scene
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("DEV SKIP: Loading next scene");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadNextScene();
            }
            else
            {
                Debug.LogError("No GameManager found");
            }
        }
    }
}