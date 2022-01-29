using TarodevController;
using UnityEngine;

public class CameraRunner : MonoBehaviour
{
    Transform player;

    void Awake()
    {
<<<<<<< Updated upstream
        player = FindObjectOfType<PlayerController>().transform;
    }

    void Update ()
    {
        Vector2 playerPos = player.position;
        transform.position = new Vector3 (playerPos.x + 6, playerPos.y + 3, -10);
=======
        transform.position = new Vector3 (player.position.x + 6, player.position.y + 3, -10);
>>>>>>> Stashed changes
    }
}
