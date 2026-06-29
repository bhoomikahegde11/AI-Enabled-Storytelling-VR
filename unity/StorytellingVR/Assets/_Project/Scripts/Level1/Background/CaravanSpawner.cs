using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Route")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] routePoints;
    [SerializeField] private Transform despawnPoint;

    [Header("Timing")]
    [SerializeField] private float minSpawnInterval = 18f;
    [SerializeField] private float maxSpawnInterval = 40f;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Limits")]
    [SerializeField] private int maxActiveCaravans = 1;

    private int activeCaravans = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        if (spawnOnStart)
        {
            TrySpawnCaravan();
        }

        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(
                    Mathf.Min(minSpawnInterval, maxSpawnInterval),
                    Mathf.Max(minSpawnInterval, maxSpawnInterval)
                )
            );

            TrySpawnCaravan();
        }
    }

    void TrySpawnCaravan()
    {
        if (prefab == null)
            return;

        if (spawnPoints == null || spawnPoints.Length == 0)
            return;

        if (activeCaravans >= Mathf.Max(1, maxActiveCaravans))
            return;

        List<Transform> route = BuildRoute();

        if (route.Count == 0)
            return;

        Transform spawnPoint =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject caravanObject = Instantiate(
            prefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        CaravanWalker walker =
            caravanObject.GetComponent<CaravanWalker>();

        if (walker == null)
        {
            walker = caravanObject.AddComponent<CaravanWalker>();
        }

        activeCaravans++;
        walker.Initialize(this, route);
    }

    public void CaravanRemoved()
    {
        activeCaravans = Mathf.Max(0, activeCaravans - 1);
    }

    private List<Transform> BuildRoute()
    {
        List<Transform> route = new List<Transform>();

        if (routePoints != null)
        {
            for (int i = 0; i < routePoints.Length; i++)
            {
                if (routePoints[i] != null)
                {
                    route.Add(routePoints[i]);
                }
            }
        }

        if (despawnPoint != null)
        {
            route.Add(despawnPoint);
        }

        return route;
    }
}
