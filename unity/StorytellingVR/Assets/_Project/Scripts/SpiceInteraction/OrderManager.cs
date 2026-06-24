using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    public SpiceType requestedSpice;

    void Awake()
    {
        Instance = this;
    }
}