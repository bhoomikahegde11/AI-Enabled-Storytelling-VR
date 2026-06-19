using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarketSpawner : MonoBehaviour
{
    public GameObject npcPrefab;

    public Transform leftSpawn;
    public Transform rightSpawn;

    public Transform leftTarget;
    public Transform rightTarget;

    public StallPoint[] stalls;
    public Transform[] leftExits;
    public Transform[] rightExits;
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

        Vector3 firstDestination;
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
                    chosenStall.transform.position;

                stallRotation =
                    chosenStall.transform.rotation;
            }
            else
            {
                visitsStall = false;

                firstDestination =
                    exitTarget.position;
            }
        }
        else
        {
            firstDestination =
                exitTarget.position;
        }


        npc.GetComponent<NPCWalker>()
        .Initialize(
        firstDestination,
        visitsStall,
        exitTarget.position,
        stallRotation,
        chosenStall
    );
    }

    public void NPCRemoved()
    {
        activeNPCs--;
    }
}