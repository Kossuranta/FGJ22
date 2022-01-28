using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRunner : MonoBehaviour
{
    public Transform player;
    void Update () 
    {
        transform.position = new Vector3 (player.position.x + 6, player.position.y + 3, -10);

    }
}
