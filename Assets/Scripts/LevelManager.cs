using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;
    
    [SerializeField]
    Transform player = null;

    [SerializeField]
    Checkpoint[] checkpoints = null;

    [SerializeField]
    LevelEnd levelEnd = null;
    
    Vector2 levelStartPos = Vector2.zero;

    void Awake()
    {
        if (player == null) Debug.LogError("player is null!", this);
        else levelStartPos = player.position;
        if (checkpoints == null || checkpoints.Length == 0) Debug.LogError($"checkpoints array is empty!", this);
        if (levelEnd == null) Debug.LogError("levelEnd is null!", this);

        Instance = this; //Stupid but fast stuff
    }

    public void Start()
    {
        levelEnd.Setup(this);
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.Setup(this);
        }
    }

    public void RespawnPlayer()
    {
        Vector3 respawnPos = GetRespawnPosition();
        player.position = respawnPos;
        player.gameObject.GetComponent<TarodevController.PlayerController>().EnableInput();
        player.gameObject.GetComponent<PlayerAnimationController>().PlayerRespawns();
    }

    Vector3 GetRespawnPosition()
    {
        for (int i = checkpoints.Length - 1; i >= 0; i--)
        {
            Checkpoint checkpoint = checkpoints[i];

            if (checkpoint != null && checkpoint.IsActive)
                return checkpoint.GetRespawnPosition();
        }

        return levelStartPos;
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

    public Transform Player { get { return player; } }
}
