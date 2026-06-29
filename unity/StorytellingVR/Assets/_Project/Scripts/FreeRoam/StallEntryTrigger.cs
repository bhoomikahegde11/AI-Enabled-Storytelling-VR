using UnityEngine;
using UnityEngine.SceneManagement;

public class StallEntryTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptCanvas;

    [Header("Scene")]
    public string nextSceneName = "TutorialScene";

    private bool playerInside = false;
    private bool isLoading = false;

    private void Start()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }

    private void Update()
    {
        if (!playerInside || isLoading)
            return;

        // X button on left Meta/Oculus controller
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            isLoading = true;

            if (promptCanvas != null)
                promptCanvas.SetActive(false);

            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        if (promptCanvas != null)
            promptCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }
}