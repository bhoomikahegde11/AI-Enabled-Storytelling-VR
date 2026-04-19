using UnityEngine;

public class InspectTriggerInput : MonoBehaviour
{
    public GameObject testcoin;
    public InspectManager manager;

    public GameObject testCoin;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.StartInspect(testCoin);
        }
    }
}