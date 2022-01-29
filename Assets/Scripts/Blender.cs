using System;
using TarodevController;
using UnityEngine;
using UnityEngine.Serialization;

public class Blender : MonoBehaviour
{
    [Serializable]
    public class KidnapTargetColorPair
    {
        public Transform kidnapTarget;
        public ColorEnum color;
    }
    
    enum State
    {
        Idle,
        MoveToBlender,
        AddIngredient,
        Blend,
        AddColors,
        End
    }

    public enum AnimationTrigger
    {
        Idle,
        AddIngredient,
        Blend,
        Fill,
        Clear,
    }

    [SerializeField]
    Animator blenderAnimator = null;

    [SerializeField]
    Animator smoothieAnimator = null;
    
    [Header("Phase durations")]
    [SerializeField]
    float moveToBlenderDuration = 5f;

    [SerializeField]
    float blendDuration = 3f;

    [SerializeField]
    float addColorsDuration = 3f;
    
    [Header("Animation settings")]
    [SerializeField]
    float playerStartPosX = 0;

    [SerializeField]
    float playerEndPosX = -11.5f;

    [Header("Kidnap target")]
    [SerializeField]
    Transform blendPos = null;
    
    [SerializeField]
    KidnapTargetColorPair[] kidnapTargetColorPairs = null;

    PlayerController player = null;
    State currentState = State.Idle;
    float timer = 0;
    Transform playerCarryPosition = null;
    Transform kidnapTarget = null;
    static readonly int IDLE = Animator.StringToHash("Idle");
    static readonly int ADD_INGREDIENT = Animator.StringToHash("AddIngredient");
    static readonly int BLEND = Animator.StringToHash("Blend");
    static readonly int FILL_START = Animator.StringToHash("FillStart");

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        if (player == null) Debug.LogError("player is null!", this);
        
        playerCarryPosition = GameObject.FindWithTag("PlayerCarryPosition")?.transform;
        if (playerCarryPosition == null) Debug.LogError("Couldn't find playerCarryPosition in scene!", this);
    }

    void Start()
    {
        ColorEnum colorToBeUnlocked = GameManager.Instance.ColorToBeUnlocked;

        if (colorToBeUnlocked != ColorEnum.None)
        {
            CurrentState = State.MoveToBlender;
            player.DisableInput();
            
            Vector3 playerPos = player.transform.localPosition;
            playerPos.x = playerStartPosX;
            player.transform.localPosition = playerPos;

            Transform kidnapTargetPrefab = null;
            foreach (KidnapTargetColorPair pair in kidnapTargetColorPairs)
            {
                if (pair.color == colorToBeUnlocked)
                    kidnapTargetPrefab = pair.kidnapTarget;
            }
            
            if (kidnapTargetPrefab != null)
            {
                kidnapTarget = Instantiate(kidnapTargetPrefab, playerCarryPosition, true);
                kidnapTarget.localPosition = Vector3.zero;
                kidnapTarget.localRotation = Quaternion.Euler(0, 0, 90f);
            }
        }
    }

    void Update()
    {
        switch (CurrentState)
        {
            case State.Idle:
                break;
            
            case State.MoveToBlender:
            {
                timer += Time.deltaTime / moveToBlenderDuration;
                Vector3 playerPos = player.transform.localPosition;
                playerPos.x = Mathf.Lerp(playerStartPosX, playerEndPosX, timer);
                player.transform.localPosition = playerPos;

                if (timer >= 1f)
                {
                    timer = 0;
                    
                    if (kidnapTarget.parent != blendPos)
                    {
                        kidnapTarget.parent = blendPos;
                    }
                    
                    SetAnimatorTrigger(AnimationTrigger.AddIngredient);
                    CurrentState = State.AddIngredient;
                }
                break;
            }
            
            case State.AddIngredient:
                break;
            
            case State.Blend:
            {
                timer += Time.deltaTime / blendDuration;
                
                if (timer >= 1f)
                {
                    timer = 0;
                    CurrentState = State.AddColors;
                    Destroy(kidnapTarget.gameObject);
                }
                break;
            }
            
            case State.AddColors:
            {
                timer += Time.deltaTime / addColorsDuration;

                if (timer >= 1f)
                {
                    timer = 0;
                    CurrentState = State.End;
                }
                break;
            }
            
            case State.End:
                player.EnableInput();
                CurrentState = State.Idle;
                SetAnimatorTrigger(AnimationTrigger.Idle);
                break;
        }
    }

    public void SetAnimatorTrigger(AnimationTrigger trigger)
    {
        switch (trigger)
        {
            case AnimationTrigger.Idle:
                blenderAnimator.SetTrigger(IDLE);
                break;
            
            case AnimationTrigger.AddIngredient:
                blenderAnimator.SetTrigger(ADD_INGREDIENT);
                break;
            
            case AnimationTrigger.Blend:
                blenderAnimator.SetTrigger(BLEND);
                smoothieAnimator.SetTrigger(FILL_START);
                CurrentState = State.Blend;
                break;
        }
    }
    
    State CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            Debug.Log($"Blender state: {value}");
            currentState = value;
        }
    }
}
