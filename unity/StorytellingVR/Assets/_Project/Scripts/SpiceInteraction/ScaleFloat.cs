using UnityEngine;
using System.Collections;

public class ScaleFloat : MonoBehaviour
{
    public Transform floatTarget;

    public void FloatToPlayer()
    {
        StartCoroutine(FloatRoutine());
    }

    IEnumerator FloatRoutine()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            transform.position =
                Vector3.Lerp(
                    startPos,
                    floatTarget.position,
                    t
                );

            transform.rotation =
                Quaternion.Slerp(
                    startRot,
                    floatTarget.rotation,
                    t
                );

            yield return null;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FloatToPlayer();
        }
    }
}