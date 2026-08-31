using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class BackgroundNpcVisualEntry
{
    public GameObject visualPrefab;
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public Vector3 localScale = Vector3.one;
}

public class MarketSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    [SerializeField] private GameObject fallbackWalkerPrefab;
    [SerializeField] private List<BackgroundNpcVisualEntry> backgroundVisualPool = new List<BackgroundNpcVisualEntry>();
    [SerializeField] private GameObject fallbackVisualPrefab;
    [SerializeField] private RuntimeAnimatorController backgroundWalkingController;

    public Transform leftSpawn;
    public Transform rightSpawn;

    public Transform leftTarget;
    public Transform rightTarget;

    public StallPoint[] stalls;
    public Transform[] leftExits;
    public Transform[] rightExits;
    private int activeNPCs = 0;
    private int lastVisualIndex = -1;

    public int maxNPCs = 3;

    void Start()
    {
        Debug.Log("[BG NPC] MarketSpawner started; waiting for market day.");
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (!Level1GameState.Instance.MarketDayStarted && !Level1GameState.Instance.MarketDayEnded)
        {
            yield return null;
        }

        while (!Level1GameState.Instance.MarketDayEnded)
        {
            Debug.Log($"[BG NPC] Spawn tick: active={activeNPCs}, max={maxNPCs}.");
            if (activeNPCs < maxNPCs)
            {
                SpawnNPC();
            }

            yield return new WaitForSeconds(
                UnityEngine.Random.Range(6f, 12f)
            );
        }

        Debug.Log("[MARKET SPAWNER] Market day ended. Background spawning stopped.");
    }

    void SpawnNPC()
    {
        GameObject walkerPrefab = npcPrefab != null ? npcPrefab : fallbackWalkerPrefab;
        bool usingFallbackWalker = npcPrefab == null && walkerPrefab != null;
        if (walkerPrefab == null)
        {
            Debug.LogWarning("[BG NPC] Spawn skipped: no walker prefab is assigned.");
            return;
        }

        bool fromLeft = UnityEngine.Random.value > 0.5f;

        Transform spawn =
            fromLeft ? leftSpawn : rightSpawn;

        Transform exitTarget;

        if (fromLeft)
        {
            exitTarget =
                rightExits[
                    UnityEngine.Random.Range(0, rightExits.Length)
                ];
        }
        else
        {
            exitTarget =
                leftExits[
                    UnityEngine.Random.Range(0, leftExits.Length)
                ];
        }

        Debug.Log($"[BG NPC] Instantiating walker prefab: {walkerPrefab.name}.");
        GameObject npc;
        try
        {
            npc = Instantiate(walkerPrefab, spawn.position, Quaternion.identity);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[BG NPC] Spawn skipped: walker instantiation failed ({exception.Message}).");
            return;
        }

        if (npc == null)
        {
            Debug.LogWarning("[BG NPC] Spawn skipped: walker instantiation returned null.");
            return;
        }

        Debug.Log($"[BG NPC] Walker instantiated: {npc.name}.", npc);

        if (usingFallbackWalker)
        {
            Debug.Log("[BG NPC] Using the embedded visual on the fallback walker.", npc);
        }
        else
        {
            try
            {
                ConfigureSpawnedVisual(npc);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[BG NPC] Visual setup failed; walker will still move ({exception.Message}).", npc);
            }
        }

        activeNPCs++;

        bool visitsStall = UnityEngine.Random.value < 0.7f;

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
                        UnityEngine.Random.Range(0, freeStalls.Count)
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


        NPCWalker walker = npc.GetComponent<NPCWalker>();
        if (walker == null)
        {
            Debug.LogWarning("[MARKET SPAWNER] Spawned background NPC has no NPCWalker component.", npc);
            return;
        }

        walker.Initialize(
        firstDestination,
        visitsStall,
        exitTarget.position,
        stallRotation,
        chosenStall
    );

        Debug.Log($"[BG NPC] Destination assigned: {firstDestination}.", npc);
    }

    public void NPCRemoved()
    {
        activeNPCs--;
    }

    private void ConfigureSpawnedVisual(GameObject npc)
    {
        if (backgroundVisualPool == null || backgroundVisualPool.Count == 0)
        {
            return;
        }

        NPCWalker walker = npc.GetComponent<NPCWalker>();
        if (walker == null)
        {
            Debug.LogWarning("[MARKET SPAWNER] Cannot assign a background visual without NPCWalker.", npc);
            return;
        }

        Transform visualAnchor = walker.VisualAnchor;
        if (visualAnchor == null)
        {
            Debug.LogWarning("[MARKET SPAWNER] Background walker has no VisualAnchor.", npc);
            return;
        }

        int visualIndex = ChooseVisualIndex();
        BackgroundNpcVisualEntry entry = visualIndex >= 0
            ? backgroundVisualPool[visualIndex]
            : CreateFallbackVisualEntry();

        if (entry == null || entry.visualPrefab == null)
        {
            Debug.LogWarning("[BG NPC] No valid visual is assigned; walker will still move without a visual.", npc);
            return;
        }

        Debug.Log($"[BG NPC] Selected visual: {entry.visualPrefab.name}.", npc);
        GameObject visual = InstantiateVisual(entry, visualAnchor);
        if (visual == null && fallbackVisualPrefab != null && entry.visualPrefab != fallbackVisualPrefab)
        {
            Debug.LogWarning("[BG NPC] Selected visual failed; trying the fallback visual.", npc);
            visual = InstantiateVisual(CreateFallbackVisualEntry(), visualAnchor);
        }

        if (visual == null)
        {
            Debug.LogWarning("[BG NPC] Visual instantiation failed; walker will still move without a visual.", npc);
            return;
        }

        Debug.Log($"[BG NPC] Visual instantiated: {visual.name}.", visual);

        Animator visualAnimator = visual.GetComponentInChildren<Animator>(true);
        if (visualAnimator == null)
        {
            Debug.LogWarning("[MARKET SPAWNER] Background visual has no Animator; it will remain static while the walker moves.", visual);
            return;
        }

        if (backgroundWalkingController == null)
        {
            Debug.LogWarning("[MARKET SPAWNER] Background walking controller is not assigned; the visual will use its imported animation setup.", visual);
        }
        else
        {
            visualAnimator.runtimeAnimatorController = backgroundWalkingController;
        }

        DisableVisualMovementComponents(visual);
        visualAnimator.applyRootMotion = false;
        walker.SetVisualAnimator(visualAnimator);
    }

    private BackgroundNpcVisualEntry CreateFallbackVisualEntry()
    {
        return fallbackVisualPrefab == null
            ? null
            : new BackgroundNpcVisualEntry { visualPrefab = fallbackVisualPrefab };
    }

    private static GameObject InstantiateVisual(BackgroundNpcVisualEntry entry, Transform visualAnchor)
    {
        try
        {
            GameObject visual = Instantiate(entry.visualPrefab, visualAnchor);
            visual.name = entry.visualPrefab.name;
            visual.transform.localPosition = entry.localPosition;
            visual.transform.localRotation = Quaternion.Euler(entry.localEulerAngles);
            visual.transform.localScale = entry.localScale;
            return visual;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[BG NPC] Visual instantiation failed: {exception.Message}");
            return null;
        }
    }

    private int ChooseVisualIndex()
    {
        List<int> validIndices = new List<int>();
        for (int i = 0; i < backgroundVisualPool.Count; i++)
        {
            if (backgroundVisualPool[i] != null && backgroundVisualPool[i].visualPrefab != null)
            {
                validIndices.Add(i);
            }
        }

        if (validIndices.Count == 0)
        {
            return -1;
        }

        if (validIndices.Count > 1 && lastVisualIndex >= 0)
        {
            validIndices.Remove(lastVisualIndex);
        }

        int selectedIndex = validIndices[UnityEngine.Random.Range(0, validIndices.Count)];
        lastVisualIndex = selectedIndex;
        return selectedIndex;
    }

    private static void DisableVisualMovementComponents(GameObject visual)
    {
        foreach (NavMeshAgent agent in visual.GetComponentsInChildren<NavMeshAgent>(true))
        {
            agent.enabled = false;
        }

        foreach (CharacterController controller in visual.GetComponentsInChildren<CharacterController>(true))
        {
            controller.enabled = false;
        }

        foreach (Rigidbody body in visual.GetComponentsInChildren<Rigidbody>(true))
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
        }

        foreach (NPCWalker nestedWalker in visual.GetComponentsInChildren<NPCWalker>(true))
        {
            nestedWalker.enabled = false;
        }
    }
}
