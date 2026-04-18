using UnityEngine;

public class InspectTriggerInput : MonoBehaviour
{
    public GameObject coin;
    public InspectManager manager;

    void Update()
    {
        Debug.Log("Update running");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SPACE PRESSED");
        }
    }
}