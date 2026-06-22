using UnityEngine;
using System.Collections;

public class ScaleFloat : MonoBehaviour
{
    public Transform raisedPosition;

    Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    public void RaiseScale()
    {
        StopAllCoroutines();
        StartCoroutine(
            MoveToPosition(
                raisedPosition.position
            )
        );
    }

    public void LowerScale()
    {
        StopAllCoroutines();
        StartCoroutine(
            MoveToPosition(
                originalPosition
            )
        );
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        Vector3 start = transform.position;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;

            transform.position =
                Vector3.Lerp(
                    start,
                    target,
                    t
                );

            yield return null;
        }
    }

    void Update()
    {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("F PRESSED");

                RaiseScale();
            }

        if (Input.GetKeyDown(KeyCode.G))
        {
            LowerScale();
        }
    }
}