using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FreeRoamMarketSpawner : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private int maxNPCs = 5;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnDelay = 6f;
    [SerializeField] private float maxSpawnDelay = 12f;

    [Header("Left A <-> Right A")]
    [SerializeField] private Transform leftEntryA;
    [SerializeField] private Transform rightExitA;

    [SerializeField] private Transform rightEntryA;
    [SerializeField] private Transform leftExitA;

    [Header("Left B <-> Right B")]
    [SerializeField] private Transform leftEntryB;
    [SerializeField] private Transform rightExitB;

    [SerializeField] private Transform rightEntryB;
    [SerializeField] private Transform leftExitB;

    [Header("A Stalls")]
    [SerializeField] private StallPoint[] aStalls;

    [Header("B Stalls")]
    [SerializeField] private StallPoint[] bStalls;

    [Header("Stall Visit Chances")]
    [Range(0f, 1f)]
    [SerializeField] private float leftAStallChance = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] private float rightBStallChance = 0.5f;

    [Header("Visual Setup")]
    [SerializeField]
    private List<BackgroundNpcVisualEntry> backgroundVisualPool =
        new List<BackgroundNpcVisualEntry>();

    [SerializeField] private GameObject fallbackVisualPrefab;
    [SerializeField] private RuntimeAnimatorController backgroundWalkingController;

    private int activeNPCs = 0;
    private int lastVisualIndex = -1;


    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }


    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (activeNPCs < maxNPCs)
            {
                SpawnNPC();
            }

            yield return new WaitForSeconds(
                UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay)
            );
        }
    }


    private void SpawnNPC()
    {
        if (npcPrefab == null)
        {
            Debug.LogWarning("[FREE ROAM SPAWNER] No NPC prefab assigned.");
            return;
        }

        Transform spawnPoint;
        Transform exitPoint;

        bool goingToStall;
        StallPoint selectedStall = null;

        Quaternion stallRotation = Quaternion.identity;

        int route = UnityEngine.Random.Range(0, 4);

        /*
         * ROUTE 0
         * Left A -> Right A
         *
         * Can either:
         * - walk directly to Right A
         * - visit random A stall first
         */
        if (route == 0)
        {
            spawnPoint = leftEntryA;
            exitPoint = rightExitA;

            goingToStall =
                UnityEngine.Random.value < leftAStallChance;

            if (goingToStall)
            {
                selectedStall = GetRandomFreeStall(aStalls);

                if (selectedStall == null)
                {
                    // No A stalls available.
                    // Just walk directly to Right A.
                    goingToStall = false;
                }
            }
        }

        /*
         * ROUTE 1
         * Right A -> Left A
         *
         * Always direct.
         */
        else if (route == 1)
        {
            spawnPoint = rightEntryA;
            exitPoint = leftExitA;

            goingToStall = false;
        }

        /*
         * ROUTE 2
         * Left B -> Right B
         *
         * Always direct.
         */
        else if (route == 2)
        {
            spawnPoint = leftEntryB;
            exitPoint = rightExitB;

            goingToStall = false;
        }

        /*
         * ROUTE 3
         * Right B -> Left B
         *
         * Can either:
         * - walk directly to Left B
         * - visit random B stall first
         */
        else
        {
            spawnPoint = rightEntryB;
            exitPoint = leftExitB;

            goingToStall =
                UnityEngine.Random.value < rightBStallChance;

            if (goingToStall)
            {
                selectedStall = GetRandomFreeStall(bStalls);

                if (selectedStall == null)
                {
                    // No B stalls available.
                    // Just walk directly to Left B.
                    goingToStall = false;
                }
            }
        }


        // Safety checks
        if (spawnPoint == null || exitPoint == null)
        {
            Debug.LogWarning(
                "[FREE ROAM SPAWNER] Route has a missing entry or exit."
            );
            return;
        }


        if (goingToStall && selectedStall != null)
        {
            selectedStall.occupied = true;

            stallRotation =
                selectedStall.transform.rotation;
        }


        // Spawn NPC
        GameObject npc;

        try
        {
            npc = Instantiate(
                npcPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"[FREE ROAM SPAWNER] NPC spawn failed: {exception.Message}"
            );

            if (selectedStall != null)
                selectedStall.occupied = false;

            return;
        }


        if (npc == null)
        {
            if (selectedStall != null)
                selectedStall.occupied = false;

            return;
        }


        activeNPCs++;


        // Configure visual exactly like your existing MarketSpawner
        ConfigureSpawnedVisual(npc);


        NPCWalker walker =
            npc.GetComponent<NPCWalker>();

        if (walker == null)
        {
            Debug.LogWarning(
                "[FREE ROAM SPAWNER] NPC prefab has no NPCWalker.",
                npc
            );

            activeNPCs--;

            if (selectedStall != null)
                selectedStall.occupied = false;

            Destroy(npc);

            return;
        }


        Vector3 firstDestination;

        if (goingToStall)
        {
            firstDestination =
                selectedStall.transform.position;
        }
        else
        {
            firstDestination =
                exitPoint.position;
        }


        walker.Initialize(
            firstDestination,
            goingToStall,
            exitPoint.position,
            stallRotation,
            selectedStall
        );
    }


    private StallPoint GetRandomFreeStall(
        StallPoint[] stalls
    )
    {
        if (stalls == null || stalls.Length == 0)
            return null;


        List<StallPoint> freeStalls =
            new List<StallPoint>();


        foreach (StallPoint stall in stalls)
        {
            if (stall != null && !stall.occupied)
            {
                freeStalls.Add(stall);
            }
        }


        if (freeStalls.Count == 0)
            return null;


        return freeStalls[
            UnityEngine.Random.Range(
                0,
                freeStalls.Count
            )
        ];
    }


    public void NPCRemoved()
    {
        activeNPCs--;

        if (activeNPCs < 0)
            activeNPCs = 0;
    }


    private void ConfigureSpawnedVisual(
        GameObject npc
    )
    {
        if (
            backgroundVisualPool == null ||
            backgroundVisualPool.Count == 0
        )
        {
            return;
        }


        NPCWalker walker =
            npc.GetComponent<NPCWalker>();

        if (walker == null)
            return;


        Transform visualAnchor =
            walker.VisualAnchor;

        if (visualAnchor == null)
            return;


        int visualIndex =
            ChooseVisualIndex();


        BackgroundNpcVisualEntry entry =
            visualIndex >= 0
                ? backgroundVisualPool[visualIndex]
                : CreateFallbackVisualEntry();


        if (
            entry == null ||
            entry.visualPrefab == null
        )
        {
            return;
        }


        GameObject visual =
            InstantiateVisual(
                entry,
                visualAnchor
            );


        if (
            visual == null &&
            fallbackVisualPrefab != null &&
            entry.visualPrefab != fallbackVisualPrefab
        )
        {
            visual =
                InstantiateVisual(
                    CreateFallbackVisualEntry(),
                    visualAnchor
                );
        }


        if (visual == null)
            return;


        Animator visualAnimator =
            visual.GetComponentInChildren<Animator>(true);


        if (visualAnimator == null)
        {
            Debug.LogWarning(
                "[FREE ROAM SPAWNER] Visual has no Animator.",
                visual
            );

            return;
        }


        if (backgroundWalkingController != null)
        {
            visualAnimator.runtimeAnimatorController =
                backgroundWalkingController;
        }


        visualAnimator.applyRootMotion = false;


        walker.SetVisualAnimator(
            visualAnimator
        );
    }


    private BackgroundNpcVisualEntry
        CreateFallbackVisualEntry()
    {
        return fallbackVisualPrefab == null
            ? null
            : new BackgroundNpcVisualEntry
            {
                visualPrefab =
                    fallbackVisualPrefab
            };
    }


    private static GameObject InstantiateVisual(
        BackgroundNpcVisualEntry entry,
        Transform visualAnchor
    )
    {
        try
        {
            GameObject visual =
                Instantiate(
                    entry.visualPrefab,
                    visualAnchor
                );

            visual.name =
                entry.visualPrefab.name;

            visual.transform.localPosition =
                entry.localPosition;

            visual.transform.localRotation =
                Quaternion.Euler(
                    entry.localEulerAngles
                );

            visual.transform.localScale =
                entry.localScale;

            return visual;
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"[FREE ROAM SPAWNER] Visual instantiation failed: {exception.Message}"
            );

            return null;
        }
    }


    private int ChooseVisualIndex()
    {
        List<int> validIndices =
            new List<int>();


        for (
            int i = 0;
            i < backgroundVisualPool.Count;
            i++
        )
        {
            if (
                backgroundVisualPool[i] != null &&
                backgroundVisualPool[i].visualPrefab != null
            )
            {
                validIndices.Add(i);
            }
        }


        if (validIndices.Count == 0)
            return -1;


        if (
            validIndices.Count > 1 &&
            lastVisualIndex >= 0
        )
        {
            validIndices.Remove(
                lastVisualIndex
            );
        }


        int selectedIndex =
            validIndices[
                UnityEngine.Random.Range(
                    0,
                    validIndices.Count
                )
            ];


        lastVisualIndex =
            selectedIndex;


        return selectedIndex;
    }
}