using System;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    [SerializeField] float deathDelay = 3f;
    [SerializeField] private PlayerAnimationController _animatorController;

    private float timer = 0;

    void Awake()
    {
        enabled = false;
    }

    public void PlayerDies()
    {
        if (enabled) return; //Already dead
        
        Debug.Log("Player dies :)");
        _animatorController.playerDies();
        timer = 0;
        enabled = true;
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
