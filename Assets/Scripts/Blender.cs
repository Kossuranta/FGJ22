using System;
using TarodevController;
using UnityEngine;

public class Blender : MonoBehaviour
{
    enum State
    {
        Idle,
        MoveToBlender,
        AddIngredient,
        Blend,
        AddColors,
        End
    }

    [SerializeField]
    PlayerController player = null;

    [SerializeField]
    float moveToBlenderDuration = 5f;

    [SerializeField]
    float addIngredientDuration = 2f;

    [SerializeField]
    float blendDuration = 3f;

    [SerializeField]
    float addColorsDuration = 3f;

    [SerializeField]
    float playerStartPosX = 0;

    [SerializeField]
    float playerEndPosX = -11.5f;

    State currentState = State.Idle;
    float timer = 0;

    void Awake()
    {
        if (player == null) Debug.LogError("player is null!", this);
    }

    void Start()
    {
        ColorEnum colorToBeUnlocked = GameManager.Instance.ColorToBeUnlocked;

        if (colorToBeUnlocked != ColorEnum.None)
        {
            currentState = State.MoveToBlender;
            player.DisableInput();
        }
    }

    void Update()
    {
        switch (currentState)
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
                    currentState = State.AddIngredient;
                }
                break;
            }
            
            case State.AddIngredient:
            {
                timer += Time.deltaTime / addIngredientDuration;

                if (timer >= 1f)
                {
                    timer = 0;
                    currentState = State.Blend;
                }
                break;
            }
            
            case State.Blend:
            {
                timer += Time.deltaTime / blendDuration;

                if (timer >= 1f)
                {
                    timer = 0;
                    currentState = State.AddColors;
                }
                break;
            }
            
            case State.AddColors:
            {
                timer += Time.deltaTime / addColorsDuration;

                if (timer >= 1f)
                {
                    timer = 0;
                    currentState = State.End;
                }
                break;
            }
            
            case State.End:
                player.EnableInput();
                currentState = State.Idle;
                break;
        }
    }
}
