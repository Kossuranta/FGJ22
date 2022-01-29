using TarodevController;
using UnityEngine;

public class CameraRunner : MonoBehaviour
{
    Transform player;

    bool follow = true;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>().transform;
    }

    void Update ()
    {
        if (!follow) return;
        
        Vector2 playerPos = player.position;
        transform.position = new Vector3 (playerPos.x + 6, playerPos.y + 1 , -10);
    }

    public void ToggleFollow(bool enabled)
    {
        follow = enabled;
    }
}
