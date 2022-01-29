using System;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    [SerializeField]
    float deathDelay = 3f;

    float timer = 0;

    void Awake()
    {
        enabled = false;
    }

    public void PlayerDies()
    {
        if (enabled) return; //Already dead
        
        Debug.Log("Player dies :)");
        //animation stuff
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
