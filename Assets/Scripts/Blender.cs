using System;
using TarodevController;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Blender : MonoBehaviour
{
    public AudioSource scream;  
    public static Blender Instance = null;
    
    [Serializable]
    public class KidnapTargetColorPair
    {
        public Transform kidnapTarget;
        public ColorEnum color;
    }

    [Serializable]
    public class ColorEnumColor
    {
        public ColorEnum colorEnum;
        public Color color;
    }
    
    public enum State
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
    ColorEnumColor[] colorPairs = null;

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
    
    static readonly int IDLE = Animator.StringToHash("Idle");
    static readonly int ADD_INGREDIENT = Animator.StringToHash("AddIngredient");
    static readonly int BLEND = Animator.StringToHash("Blend");
    static readonly int FILL_START = Animator.StringToHash("FillStart");

    PlayerController player = null;
    State currentState = State.Idle;
    float timer = 0;
    Transform playerCarryPosition = null;
    Transform kidnapTarget = null;
    PlayerAnimationController playerAnimator = null;

    void Awake()
    {
        Instance = this;
        
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

            playerAnimator = player.GetComponentInChildren<PlayerAnimationController>();
            playerAnimator.PlayerHasHandsUp();
            playerAnimator.SetSpriteFlipX(true);
            
            Vector3 playerPos = player.transform.localPosition;
            playerPos.x = playerStartPosX;
            player.transform.localPosition = playerPos;

            Transform kidnapTargetPrefab = null;
            foreach (KidnapTargetColorPair pair in kidnapTargetColorPairs)
            {
                if (pair.color == colorToBeUnlocked)
                    kidnapTargetPrefab = pair.kidnapTarget;
            }
            
            Color smoothieColor = Color.white;
            foreach (ColorEnumColor colorPair in colorPairs)
            {
                if (colorPair.colorEnum == colorToBeUnlocked)
                    smoothieColor = colorPair.color;
            }

            SpriteRenderer smoothieSpriteRenderer = smoothieAnimator.GetComponent<SpriteRenderer>();
            smoothieSpriteRenderer.color = smoothieColor;
            
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
                    GameManager.Instance.OnBlenderCompleted();
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
            {
                player.EnableInput();
                CurrentState = State.Idle;
                SetAnimatorTrigger(AnimationTrigger.Idle);
                
                if(GameManager.Instance.EnabledColors.HasFlag(ColorEnum.Green) &&
                GameManager.Instance.EnabledColors.HasFlag(ColorEnum.Orange) &&
                GameManager.Instance.EnabledColors.HasFlag(ColorEnum.Purple))
                {
                    Invoke(nameof(EndGame), 0.7f); 
                }

                break;
            }
        }
    }

    private void EndGame() {
        SceneManager.LoadScene("EndGameScene");
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
                playerAnimator.PlayerIdle();
                break;
            
            case AnimationTrigger.Blend:
                blenderAnimator.SetTrigger(BLEND);
                smoothieAnimator.SetTrigger(FILL_START);
                CurrentState = State.Blend;
                scream.Play();
                break;

        }
    }
    
    public State CurrentState
    {
        get
        {
            return currentState;
        }
        private set
        {
            Debug.Log($"Blender state: {value}");
            currentState = value;
        }
    }
}
