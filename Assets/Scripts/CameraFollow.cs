using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject Player;
    public GameObject Camera;

    public Vector3 Offset;

    private void LateUpdate()
    {
        Camera.transform.position = Player.transform.position + Offset;

    }

}
