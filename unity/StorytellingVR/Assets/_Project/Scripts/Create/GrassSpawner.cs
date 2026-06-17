using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    [Header("Grass")]
    public GameObject grassPrefab;
    public int grassCount = 80;

    [Header("Area")]
    public Vector2 areaSize = new Vector2(50, 50);

    [Header("Randomization")]
    public Vector2 scaleRange = new Vector2(0.7f, 1.4f);

    [Header("Keep Center Clear")]
    public float clearPathWidth = 5f;


    [ContextMenu("Generate Grass")]
    void GenerateGrass()
    {
        if (grassPrefab == null)
        {
            Debug.LogError("Grass prefab missing");
            return;
        }

        // remove old generated grass
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }


        for (int i = 0; i < grassCount; i++)
        {
            float x = Random.Range(
                -areaSize.x / 2,
                 areaSize.x / 2
            );

            float z = Random.Range(
                -areaSize.y / 2,
                 areaSize.y / 2
            );


            // avoid walking path
            if (Mathf.Abs(x) < clearPathWidth)
                continue;


            Vector3 pos = new Vector3(
                x,
                0,
                z
            );


            GameObject grass = Instantiate(
                grassPrefab,
                pos,
                Quaternion.identity,
                transform
            );


            grass.transform.rotation =
                Quaternion.Euler(
                    0,
                    Random.Range(0, 360),
                    0
                );


            float s = Random.Range(
                scaleRange.x,
                scaleRange.y
            );

            grass.transform.localScale *= s;


            grass.isStatic = true;
        }


        Debug.Log("Grass generated");
    }
}
