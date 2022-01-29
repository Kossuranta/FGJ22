using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    Collider2D collider2D = null;

    [SerializeField]
    Transform respawnPosition = null;

    [SerializeField]
    Transform flag = null;

    LevelManager levelManager = null;
    Quaternion flagStartRot = Quaternion.identity;
    Quaternion flagTargetRot = Quaternion.identity;
    float timer = 0f;

    void Awake()
    {
        if (collider2D == null) Debug.LogError("collider2D is null!", this);
        else if (!collider2D.isTrigger) Debug.LogError("checkpoint has collider that isn't trigger!", this);
        if (respawnPosition == null) Debug.LogError("respawnPosition is null!", this);
        
        if (flag == null)
        {
            Debug.LogError("flag is null!", this);
        }
        else
        {
            flagStartRot = Quaternion.Euler(0, 0, 90);
            flagTargetRot = Quaternion.identity;
            flag.transform.rotation = flagStartRot;
        }
        
        enabled = false;
    }

    void Update()
    {
        timer += Time.deltaTime;
        flag.transform.rotation = Quaternion.Lerp(flagStartRot, flagTargetRot, timer);

        if (timer >= 1f) enabled = false;
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
        timer = 0;
        enabled = true;
        
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
