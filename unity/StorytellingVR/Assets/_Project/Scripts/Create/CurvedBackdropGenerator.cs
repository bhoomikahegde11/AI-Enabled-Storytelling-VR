using UnityEngine;

[ExecuteInEditMode]
public class CurvedBackdropGenerator : MonoBehaviour
{
    public float radius = 14f;
    public float height = 7f;
    public int segments = 64;
    public float arcDegrees = 220f;

    public Material backdropMaterial;

    [ContextMenu("Generate Curved Backdrop")]
    public void Generate()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();

        if (mf == null) mf = gameObject.AddComponent<MeshFilter>();
        if (mr == null) mr = gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.name = "Curved_Backdrop_Mesh";

        int vertCount = (segments + 1) * 2;
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] triangles = new int[segments * 6];

        float startAngle = -arcDegrees / 2f;
        float step = arcDegrees / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (startAngle + step * i);

            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;

            vertices[i * 2] = new Vector3(x, 0, z);
            vertices[i * 2 + 1] = new Vector3(x, height, z);

            float u = (float)i / segments;
            uvs[i * 2] = new Vector2(u, 0);
            uvs[i * 2 + 1] = new Vector2(u, 1);
        }

        int t = 0;
        for (int i = 0; i < segments; i++)
        {
            int a = i * 2;
            int b = a + 1;
            int c = a + 2;
            int d = a + 3;

            // inward-facing triangles
            triangles[t++] = a;
            triangles[t++] = b;
            triangles[t++] = c;

            triangles[t++] = c;
            triangles[t++] = b;
            triangles[t++] = d;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        mf.sharedMesh = mesh;

        if (backdropMaterial != null)
            mr.sharedMaterial = backdropMaterial;

        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
    }
}
