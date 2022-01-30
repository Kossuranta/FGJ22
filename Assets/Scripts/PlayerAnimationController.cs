using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("_playerAnimator")]
    Animator playerAnimator;

    [SerializeField]
    SpriteRenderer playersArtsSpriteRenderer;

    [SerializeField]
    Transform playersArtsTransform;
    
    static readonly int MOVING = Animator.StringToHash("Moving");
    static readonly int ROLLING = Animator.StringToHash("Rolling");
    static readonly int IN_AIR_UP = Animator.StringToHash("InAirUp");
    static readonly int IN_AIR_DOWN = Animator.StringToHash("InAirDown");
    static readonly int DEATH = Animator.StringToHash("Death");
    static readonly int RESPAWN = Animator.StringToHash("Respawn");
    static readonly int HANDS_UP = Animator.StringToHash("HandsUp");
    static readonly int IDLE = Animator.StringToHash("Idle");

    bool isMoving = false, isRolling = false, isDead = false, isFlipped = false;

    public void SetSpriteFlipX(bool flip)
    {
        if (flip != isFlipped)
        {
            if (flip)
            {
                playersArtsSpriteRenderer.flipX = true;
                isFlipped = true;
            }
            else
            {
                playersArtsSpriteRenderer.flipX = false;
                isFlipped = false;
            }
        }
    }

    public void StartMoving()
    {
        if (!isMoving)
        {
            playerAnimator.SetBool(MOVING, true);
            isMoving = true;
        }
    }

    public void StopMoving()
    {
        if (isMoving)
        {
            playerAnimator.SetBool(MOVING, false);
            isMoving = false;
        }
    }

    public void StartRolling()
    {
        if (!isRolling)
        {
            playerAnimator.SetBool(ROLLING, true);
            isRolling = true;
        }
    }

    public void StopRolling()
    {
        if (isRolling)
        {
            playerAnimator.SetBool(ROLLING, false);
            isRolling = false;
        }
    }

    void Update()
    {
        if (isRolling)
        {
            if (!isFlipped) playersArtsTransform.Rotate(Vector3.forward * -500f * Time.deltaTime);
            else playersArtsTransform.Rotate(Vector3.forward * 500f * Time.deltaTime);
        }
        else
        {
            playersArtsTransform.eulerAngles = Vector3.forward * 0f;
        }
    }

    bool isFlyingUp = false;

    public void PlayerFlyingUp(bool state)
    {
        if (state != isFlyingUp)
        {
            isFlyingUp = state;
            playerAnimator.SetBool(IN_AIR_UP, state);
        }
    }

    bool isFlyingDown = false;

    public void PlayerFlyingDown(bool state)
    {
        if (state != isFlyingDown)
        {
            isFlyingDown = state;
            playerAnimator.SetBool(IN_AIR_DOWN, state);
        }
    }

    public void PlayerDies()
    {
        if (!isDead)
        {
            playerAnimator.SetTrigger(DEATH);
            playerAnimator.ResetTrigger(RESPAWN);
            isDead = true;
        }
    }

    public void PlayerRespawns()
    {
        Debug.Log("PlayerRespawn");
        if (isDead)
        {
            playerAnimator.SetTrigger(RESPAWN);
            playerAnimator.ResetTrigger(DEATH);
            isDead = false;
        }
    }

    public void PlayerHasHandsUp()
    {
        playerAnimator.SetTrigger(HANDS_UP);
    }

    public void PlayerIdle()
    {
        playerAnimator.SetTrigger(IDLE);
    }
}