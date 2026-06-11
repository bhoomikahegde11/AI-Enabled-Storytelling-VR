using UnityEngine;
using System.Collections;

public class MarketSpawner : MonoBehaviour
{
    public GameObject npcPrefab;

    public Transform leftSpawn;
    public Transform rightSpawn;

    public Transform leftTarget;
    public Transform rightTarget;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnNPC();

            yield return new WaitForSeconds(
                Random.Range(3f, 8f)
            );
        }
    }

    void SpawnNPC()
    {
        bool fromLeft = Random.value > 0.5f;

        Transform spawn =
            fromLeft ? leftSpawn : rightSpawn;

        Transform target =
            fromLeft ? rightTarget : leftTarget;

        GameObject npc =
            Instantiate(
                npcPrefab,
                spawn.position,
                Quaternion.identity
            );

        npc.GetComponent<NPCWalker>()
            .Initialize(target.position);
    }
}