using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class InspectManager : MonoBehaviour
{
    public Volume volume;              // Assign in Inspector
    private DepthOfField dof;
    public GameObject infoPanel;
    public Transform inspectPoint;
    public float moveDuration = 0.6f;

    void Start()
    {
        infoPanel.SetActive(false);
        // Get Depth of Field from Volume
        if (volume.profile.TryGet(out dof))
        {
            dof.active = false; // start with blur OFF
        }
        else
        {
            Debug.LogError("Depth of Field not found in Volume!");
        }
    }

    public void StartInspect(GameObject obj)
    {
        StartCoroutine(MoveToInspect(obj));
    }

    IEnumerator MoveToInspect(GameObject obj)
    {
        Transform t = obj.transform;

        Transform cam = Camera.main.transform;

        // 🔥 Capture FINAL values (what you set manually)
        Vector3 endPos = t.position;
        Vector3 endScale = t.localScale;
        Quaternion endRot = t.rotation;

        // 🔥 Define start values
        Vector3 startPos = cam.position + cam.forward * 1.5f;
        Vector3 startScale = Vector3.one * 0.05f;

        // Apply start state
        t.position = startPos;
        t.localScale = startScale;

        float time = 0;

        while (time < moveDuration)
        {
            float tVal = Mathf.SmoothStep(0, 1, time / moveDuration);

            // Move
            t.position = Vector3.Lerp(startPos, endPos, tVal);

            // Scale
            t.localScale = Vector3.Lerp(startScale, endScale, tVal);

            time += Time.deltaTime;
            yield return null;
        }

        // Final snap (exact)
        t.position = endPos;
        t.localScale = endScale;
        t.rotation = endRot;
        infoPanel.SetActive(true);
    }
    void EnableBlur()
    {
        if (dof != null)
        {
            dof.active = true;
        }
    }
}