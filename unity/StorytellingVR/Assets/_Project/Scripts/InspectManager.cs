using UnityEngine;

public class InspectManager : MonoBehaviour
{
    public Transform inspectPoint;
    public GameObject inspectLight;

    public void StartInspect(GameObject obj)
    {
        obj.transform.SetParent(null);
        obj.transform.position = inspectPoint.position;
        obj.transform.rotation = inspectPoint.rotation;
        obj.transform.SetParent(inspectPoint);

        inspectLight.SetActive(true); // TURN ON LIGHT
    }

    public void EndInspect(GameObject obj)
    {
        inspectLight.SetActive(false); // TURN OFF LIGHT
    }
}