using System;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    [SerializeField] float deathDelay = 3f;
    [SerializeField] private PlayerAnimationController _animatorController;
    private TarodevController.PlayerController _playerController;

    private float timer = 0;

    void Awake()
    {
        _playerController = gameObject.GetComponent<TarodevController.PlayerController>();
        enabled = false;
    }

    public void PlayerDies()
    {
        if (GameManager.Instance.CurrentLevel == Levels.Hub) return;
        Transform playerCarryPos = GameObject.FindWithTag("PlayerCarryPosition")?.transform;
        if (playerCarryPos != null && playerCarryPos.childCount > 0) return;
        if (enabled) return; //Already dead
        
        Debug.Log("Player dies :)");
        _animatorController.PlayerDies();
        timer = 0;
        enabled = true;
        _playerController.DisableInput();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > deathDelay)
        {
            enabled = false;
            LevelManager.Instance.RespawnPlayer();
        }
    }
}
