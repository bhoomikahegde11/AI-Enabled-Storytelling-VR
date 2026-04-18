using UnityEngine;

public class InspectManager : MonoBehaviour
{
    public Transform inspectPoint;
    private GameObject currentObject;

    public void StartInspect(GameObject obj)
    {
        currentObject = obj;

        obj.transform.SetParent(inspectPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    public void EndInspect()
    {
        currentObject.transform.SetParent(null);
    }
}