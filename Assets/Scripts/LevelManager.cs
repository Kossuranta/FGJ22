using System;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;
    
    [SerializeField]
    Transform player = null;

    Vector2 levelStartPos = Vector2.zero;
    Checkpoint[] checkpoints = null;

    Checkpoint lastCheckPoint = null;

    void Awake()
    {
        if (player == null) Debug.LogError("player is null!", this);
        else levelStartPos = player.position;

        checkpoints = FindObjectsOfType<Checkpoint>();
        if (checkpoints == null || checkpoints.Length == 0) Debug.LogError($"checkpoints array is empty!", this);

        Instance = this; //Stupid but fast stuff
    }

    public void Start()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.Setup(this);
        }
    }

    void Update()
    {
        if (player.localPosition.y < -60f)
        {
            KillPlayer killPlayer = player.GetComponentInChildren<KillPlayer>();
            
            if (killPlayer != null)
                killPlayer.PlayerDies();
            else
                Debug.LogError("KillPlayer script not found!");
        }
    }

    public void RespawnPlayer()
    {
        Vector3 respawnPos = GetRespawnPosition();
        player.position = respawnPos;
        PlayerController playerController = player.gameObject.GetComponent<TarodevController.PlayerController>();
        playerController.EnableInput();
        playerController.PlayerRespawned();
        player.gameObject.GetComponent<PlayerAnimationController>().PlayerRespawns();
    }

    Vector3 GetRespawnPosition()
    {
        if (lastCheckPoint != null && lastCheckPoint.IsActive)
            return lastCheckPoint.GetRespawnPosition();

        return levelStartPos;
    }

    public void CheckpointActivated(Checkpoint activatedCheckpoint)
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint != activatedCheckpoint)
                checkpoint.Deactivate();
        }
        
        lastCheckPoint = activatedCheckpoint;
    }

    public Transform Player
    {
        get
        {
            if (player == null)
                player = FindObjectOfType<PlayerController>().transform;
            return player;
        }
    }
}
