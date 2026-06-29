using UnityEngine;

public class FloorTiler : MonoBehaviour
{
    public GameObject tilePrefab;

    public int rows = 6;
    public int columns = 6;

    public float tileSizeX = 2f;
    public float tileSizeZ = 2f;

    public bool randomRotate = true;

    [ContextMenu("Generate Floor")]
    public void GenerateFloor()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab is missing.");
            return;
        }

        // Clear old tiles
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        float startX = -(columns - 1) * tileSizeX / 2f;
        float startZ = -(rows - 1) * tileSizeZ / 2f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 pos = new Vector3(
                    startX + c * tileSizeX,
                    0,
                    startZ + r * tileSizeZ
                );

                GameObject tile = Instantiate(tilePrefab, transform);
                tile.transform.localPosition = pos;

                if (randomRotate)
                {
                    int rot = Random.Range(0, 4) * 90;
                    tile.transform.localRotation = Quaternion.Euler(0, rot, 0);
                }

                tile.name = $"StoneTile_{r}_{c}";
            }
        }
    }
}