using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;

    void Start()
    {
        InvokeRepeating("SpawnCustomer", 2f, 10f);
    }

    void SpawnCustomer()
    {
        Instantiate(customerPrefab, transform.position, Quaternion.identity);
    }
}