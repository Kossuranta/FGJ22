using TarodevController;
using UnityEngine;

public class CameraRunner : MonoBehaviour
{
    Transform player;
    public float cameraAdjustX = 6;
    public float cameraAdjustY = 0;
    public float cameraAdjustZ = -10;
    bool follow = true;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>().transform;
    }

    void Update ()
    {
        if (!follow) return;
        
        Vector2 playerPos = player.position;
        transform.position = new Vector3 (playerPos.x + cameraAdjustX, playerPos.y + cameraAdjustY, cameraAdjustZ);
    }

    public void ToggleFollow(bool enabled)
    {
        follow = enabled;
    }
}
