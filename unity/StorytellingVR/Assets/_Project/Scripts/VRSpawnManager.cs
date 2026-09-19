using System.Collections;
using UnityEngine;

public class VRSpawnManager : MonoBehaviour
{
    public Transform playerRig;
    public Transform spawnPoint;


    IEnumerator Start()
    {
        // wait for Quest tracking initialization
        yield return null;
        yield return null;


        ResetPosition();
    }


    void ResetPosition()
    {
        Transform camera =
            Camera.main.transform;


        // rotate rig to match spawn direction
        float rotationOffset =
            spawnPoint.eulerAngles.y -
            camera.eulerAngles.y;


        playerRig.Rotate(
            0,
            rotationOffset,
            0
        );


        // move rig so headset ends at spawn point
        Vector3 positionOffset =
            spawnPoint.position -
            camera.position;


        positionOffset.y = 0;


        playerRig.position +=
            positionOffset;


        Debug.Log(
            "VR Spawn Reset"
        );
    }
}