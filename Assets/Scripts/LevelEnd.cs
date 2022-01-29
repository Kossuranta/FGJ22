using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelEnd : MonoBehaviour
{
    enum State
    {
        Pickup,
        CarryAway,
        TeleportToHub,
        End,
    }
    
    [SerializeField]
    new Collider2D collider2D = null;

    [SerializeField]
    float pickupDuration = 3f;

    [SerializeField]
    float carryAwayDuration = 7f;

    [SerializeField]
    Transform kidnapTarget = null;

    Transform playerCarryPosition = null;
    Vector2 kidnapTargetStartPos = Vector2.zero;

    LevelManager levelManager = null;
    float timer = 0f;

    State currentState = State.Pickup;

    readonly Quaternion targetRotation = Quaternion.Euler(0, 0, 90f);
    float playerEndTriggerPosX = 0;
    
    CameraRunner cameraRunner = null;

    void Awake()
    {
        if (collider2D == null) Debug.LogError("collider2D is null!", this);
        else if (!collider2D.isTrigger) Debug.LogError("level end has collider that isn't trigger!", this);
        if (kidnapTarget == null) Debug.LogError("kidnapTarget is null!", this);
        else kidnapTargetStartPos = kidnapTarget.position;
        
        cameraRunner = FindObjectOfType<CameraRunner>();
        if (cameraRunner == null) Debug.LogError("Couldn't find CameraRunner in scene!", this);

        playerCarryPosition = GameObject.FindWithTag("PlayerCarryPosition")?.transform;
        if (playerCarryPosition == null) Debug.LogError("Couldn't find playerCarryPosition in scene!", this);

        enabled = false;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Pickup:
            {
                timer += Time.deltaTime / pickupDuration;

                kidnapTarget.position = Vector2.Lerp(kidnapTargetStartPos, playerCarryPosition.position, timer);
                kidnapTarget.rotation = Quaternion.Lerp(Quaternion.identity, targetRotation, timer);

                if (timer >= 1)
                {
                    currentState = State.CarryAway;
                    timer = 0;
                }
                break;
            }

            case State.CarryAway:
            {
                timer += Time.deltaTime / carryAwayDuration;

                Vector2 pos = levelManager.Player.position;
                pos.x = Mathf.Lerp(playerEndTriggerPosX, playerEndTriggerPosX - 15f, timer);
                levelManager.Player.position = pos;

                if (timer >= 1)
                {
                    currentState = State.TeleportToHub;
                    timer = 0;
                }
                break;
            }
            
            case State.TeleportToHub:
            {
                enabled = false;
                GameManager.Instance.EnterLevel(Levels.Hub);
                currentState = State.End;
                break;
            }
            
            case State.End: break;
        }
    }

    public void Setup(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        LevelCompleted();
    }

    void LevelCompleted()
    {
        collider2D.enabled = false;
        enabled = true;
        cameraRunner.ToggleFollow(false);
        playerEndTriggerPosX = levelManager.Player.position.x;

        kidnapTarget.SetParent(playerCarryPosition, true);
    }
}
