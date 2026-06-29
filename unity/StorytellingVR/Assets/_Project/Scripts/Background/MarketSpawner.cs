using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarketSpawner : MonoBehaviour
{
    public GameObject npcPrefab;

    public Transform leftSpawn;
    public Transform rightSpawn;

    public StallPoint[] stalls;
    public Transform[] leftExits;
    public Transform[] rightExits;

    [Header("NPC Motion Variation")]
    [SerializeField] private float minSpeedMultiplier = 0.9f;
    [SerializeField] private float maxSpeedMultiplier = 1.1f;
    [SerializeField] private float minWaitTime = 6f;
    [SerializeField] private float maxWaitTime = 14f;
    [SerializeField] private float destinationOffsetRadius = 0.35f;

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

        Transform exitTarget;

        if (fromLeft)
        {
            exitTarget =
                rightExits[
                    Random.Range(0, rightExits.Length)
                ];
        }
        else
        {
            exitTarget =
                leftExits[
                    Random.Range(0, leftExits.Length)
                ];
        }

        GameObject npc = Instantiate(
            npcPrefab,
            spawn.position,
            Quaternion.identity
        );

        activeNPCs++;

        bool visitsStall = Random.value < 0.7f;
        float speedMultiplier = Random.Range(
            Mathf.Min(minSpeedMultiplier, maxSpeedMultiplier),
            Mathf.Max(minSpeedMultiplier, maxSpeedMultiplier)
        );
        float stallWaitTime = Random.Range(
            Mathf.Min(minWaitTime, maxWaitTime),
            Mathf.Max(minWaitTime, maxWaitTime)
        );

        Vector3 firstDestination;
        Vector3 exitDestination =
            GetOffsetPosition(exitTarget.position);

        Quaternion stallRotation = Quaternion.identity;
        StallPoint chosenStall = null;

        if (visitsStall)
        {
            List<StallPoint> freeStalls =
            new List<StallPoint>();

            foreach (StallPoint stall in stalls)
            {
                if (!stall.occupied)
                {
                    freeStalls.Add(stall);
                }
            }
            if (freeStalls.Count > 0)
            {
                chosenStall =
                    freeStalls[
                        Random.Range(0, freeStalls.Count)
                    ];

                chosenStall.occupied = true;

                firstDestination =
                    GetOffsetPosition(chosenStall.transform.position);

                stallRotation =
                    chosenStall.transform.rotation;
            }
            else
            {
                visitsStall = false;

                firstDestination =
                    exitDestination;
            }
        }
        else
        {
            firstDestination =
                exitDestination;
        }


        npc.GetComponent<NPCWalker>()
        .Initialize(
        this,
        firstDestination,
        visitsStall,
        exitDestination,
        stallRotation,
        chosenStall,
        speedMultiplier,
        stallWaitTime
    );
    }

    public void NPCRemoved()
    {
        activeNPCs--;
    }

    private Vector3 GetOffsetPosition(Vector3 basePosition)
    {
        Vector2 offset =
            Random.insideUnitCircle *
            Mathf.Max(0f, destinationOffsetRadius);

        return basePosition + new Vector3(
            offset.x,
            0f,
            offset.y
        );
    }
}
