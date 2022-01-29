using TarodevController;
using UnityEngine;

public class CameraRunner : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager = null;
    
    [SerializeField]
    float dampTime = 0.15f;

    [SerializeField]
    Vector3 cameraOffset = new Vector3(6f, 0, -10f);

    [SerializeField]
    Vector3 blenderAnimationPosition = new Vector3(-7f, 2.5f, -10f);
    
    PlayerController player = null;
    new Camera camera = null;
    
    bool follow = true;
    
    Vector3 velocity = Vector3.zero;

    void Awake()
    {
        camera = GetComponent<Camera>();
        player = FindObjectOfType<PlayerController>();
        
        if (camera == null) Debug.LogError("Camera not found!", this);
        if (player == null) Debug.LogError("Player not found!", this);

        if (GameManager.Instance == null)
        {
            Instantiate(gameManager);
        }
    }

    void Update()
    {
        if (!follow) return;
        if (player == null) return;
        if (camera == null) return;

        if (GameManager.Instance.ColorToBeUnlocked != ColorEnum.None) //During blender animation
        {
            transform.localPosition = blenderAnimationPosition;
            return;
        }
        else if (cameraOffset.x != 0)
        {
            if (player.Input.X < -0.1f && player.Velocity.x < -0.1f)
            {
                if (cameraOffset.x > 0)
                    cameraOffset.x = -cameraOffset.x;
            }
            else if (player.Input.X > 0.1f && player.Velocity.x > 0.1f)
            {
                if (cameraOffset.x < 0)
                    cameraOffset.x = -cameraOffset.x;
            }
        }

        Vector3 playerPos = player.transform.localPosition + cameraOffset;
        Vector3 point = camera.WorldToViewportPoint(playerPos);
        Vector3 delta = playerPos - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        Vector3 cameraPos = transform.localPosition;
        Vector3 destination = cameraPos + delta;
        cameraPos = Vector3.SmoothDamp(cameraPos, destination, ref velocity, dampTime);

        transform.localPosition = cameraPos;
    }

    public void ToggleFollow(bool enabled)
    {
        follow = enabled;
    }
}
