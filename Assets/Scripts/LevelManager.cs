using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    Transform levelStartPos = null;
    
    [SerializeField]
    Checkpoint[] checkpoints = null;

    void Awake()
    {
        if (levelStartPos == null) Debug.LogError($"levelStartPos is null!", this);
        if (checkpoints == null || checkpoints.Length == 0) Debug.LogError($"checkpoints array is empty!", this);
    }

    public void Start()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.Setup(this);
        }
    }

    public Vector3 GetRespawnPosition()
    {
        for (int i = checkpoints.Length - 1; i >= 0; i--)
        {
            Checkpoint checkpoint = checkpoints[i];

            if (checkpoint != null && checkpoint.IsActive)
                return checkpoint.GetRespawnPosition();
        }

        return levelStartPos?.position ?? Vector3.zero;
    }

    public void CheckpointActivated(Checkpoint activatedCheckpoint)
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.Activate();
            
            if (checkpoint == activatedCheckpoint)
                return;
        }
    }
}
