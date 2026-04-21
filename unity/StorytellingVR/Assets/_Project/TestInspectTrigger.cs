using UnityEngine;

public class TestInspectTrigger : MonoBehaviour
{
    public GameObject coin;
    public InspectManager manager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.StartInspect(coin);
        }
    }
}