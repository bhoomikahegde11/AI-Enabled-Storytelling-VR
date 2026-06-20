using UnityEngine;

public class ScaleVisualController : MonoBehaviour
{
    public GameObject scaleSpiceVisual;

    void Start()
    {
        scaleSpiceVisual.SetActive(false);
    }

    public void ShowSpices()
    {
        scaleSpiceVisual.SetActive(true);
    }

    public void HideSpices()
    {
        scaleSpiceVisual.SetActive(false);
    }
}