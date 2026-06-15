using UnityEngine;
using System.Collections;

public class MarketSpawner : MonoBehaviour
{
    public GameObject npcPrefab;

    public Transform leftSpawn;
    public Transform rightSpawn;

    public Transform leftTarget;
    public Transform rightTarget;

    private GameObject currentNPC;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if(currentNPC == null)
            {
                SpawnNPC();
            }
            yield return new WaitForSeconds(10f);
        }
    }

    void SpawnNPC()
    {
        bool fromLeft = Random.value > 0.5f;

        Transform spawn =
            fromLeft ? leftSpawn : rightSpawn;

        Transform target =
            fromLeft ? rightTarget : leftTarget;

        currentNPC = Instantiate(npcPrefab, spawn.position, Quaternion.identity);

        currentNPC.GetComponent<NPCWalker>()
            .Initialize(target.position);
    }
}