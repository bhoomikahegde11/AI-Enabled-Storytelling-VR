using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    [Header("Tutorial")]
    public bool tutorialMode = true;

    public SpiceType tutorialSpice = SpiceType.Cardamom;

    [Header("Gameplay")]
    public SpiceType requestedSpice;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (tutorialMode)
        {
            requestedSpice = tutorialSpice;
        }
    }

    public void SetRequestedSpice(SpiceType spice)
    {
        requestedSpice = spice;
    }
}