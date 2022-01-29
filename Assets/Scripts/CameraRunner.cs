using TarodevController;
using UnityEngine;

public class CameraRunner : MonoBehaviour
{
    Transform player;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>().transform;
    }

    void Update ()
    {
        Vector2 playerPos = player.position;
        transform.position = new Vector3 (playerPos.x + 6, playerPos.y + 1 , -10);
    }
}
