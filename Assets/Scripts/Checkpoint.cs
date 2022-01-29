using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    Collider2D collider2D = null;

    [SerializeField]
    Transform respawnPosition = null;

    LevelManager levelManager = null;

    void Awake()
    {
        if (collider2D == null) Debug.LogError("collider2D is null!", this);
        if (respawnPosition == null) Debug.LogError("respawnPosition is null!", this);
    }

    public void Setup(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
       Activate();
    }

    public void Activate()
    {
        if (IsActive) return;
        
        IsActive = true;
        
        if (collider2D != null)
            collider2D.enabled = false;

        if (levelManager == null)
        {
            Debug.LogError("levelManager is null!", this);
        }
        else
        {
            levelManager.CheckpointActivated(this);
        }
    }

    public Vector3 GetRespawnPosition()
    {
        if (respawnPosition == null)
        {
            Debug.LogError($"respawnPosition is null!", this);
            Vector3 pos = transform.position;
            pos.y += 5;
            return pos;
        }

        return respawnPosition.position;
    }

    public bool IsActive { get; private set; }
}
