using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyingFromFallingOrRunningIntoWall : MonoBehaviour {
    [SerializeField] KillPlayer _playerKillingScript;
    
    private void Awake() {
        
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            collision.gameObject.SendMessage("ApplyDamage", 10);
        }
    }
}
