using UnityEngine;
using System.Collections;

public class MarketSpawner : MonoBehaviour
{
    public GameObject npcPrefab;

    public Transform leftSpawn;
    public Transform rightSpawn;

    public Transform leftTarget;
    public Transform rightTarget;

    public Transform[] stalls;

    private int activeNPCs = 0;

    public int maxNPCs = 3;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (activeNPCs < maxNPCs)
            {
                SpawnNPC();
            }

            yield return new WaitForSeconds(
                Random.Range(6f, 12f)
            );
        }
    }

    void SpawnNPC()
    {
        bool fromLeft = Random.value > 0.5f;

        Transform spawn =
            fromLeft ? leftSpawn : rightSpawn;

        Transform exitTarget =
            fromLeft ? rightTarget : leftTarget;

        GameObject npc = Instantiate(
            npcPrefab,
            spawn.position,
            Quaternion.identity
        );

        activeNPCs++;

        bool visitsStall = Random.value < 0.7f;

        Vector3 firstDestination;
Quaternion stallRotation = Quaternion.identity;

if (visitsStall && stalls.Length > 0)
{
    Transform chosenStall =
        stalls[Random.Range(0, stalls.Length)];

    firstDestination = chosenStall.position;
    stallRotation = chosenStall.rotation;
}
else
{
    firstDestination = exitTarget.position;
}

npc.GetComponent<NPCWalker>()
    .Initialize(
        firstDestination,
        visitsStall,
        exitTarget.position,
        stallRotation
    );
    }

    public void NPCRemoved()
    {
        activeNPCs--;
    }
}