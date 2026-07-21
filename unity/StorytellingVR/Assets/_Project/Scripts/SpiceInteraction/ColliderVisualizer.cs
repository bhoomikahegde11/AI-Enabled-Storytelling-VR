using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ColliderVisualizer : MonoBehaviour
{
    public Color cubeColor = new Color(1f, 0f, 0f, 0.3f);

    private void Start()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Collider Debug";

        // Parent it to the collider object
        cube.transform.SetParent(transform, false);

        // Match the Box Collider
        cube.transform.localPosition = box.center;
        cube.transform.localRotation = Quaternion.identity;
        cube.transform.localScale = box.size;

        // Don't let the debug cube interfere with physics
        Destroy(cube.GetComponent<BoxCollider>());

        // Make it visible
        Renderer r = cube.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        r.material.color = cubeColor;
    }
}