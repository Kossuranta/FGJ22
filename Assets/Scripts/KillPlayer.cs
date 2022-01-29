using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    public void PlayerDies()
    {
        Debug.Log("Player dies :)");
        //animation stuff
        Invoke(nameof(Respawn), 3f);
    }

    void Respawn()
    {
        LevelManager.Instance.RespawnPlayer();
    }
}
