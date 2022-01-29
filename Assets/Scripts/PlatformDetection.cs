using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDetection : MonoBehaviour
{
    [SerializeField] FloatingPlatform floatingPlatformScript;

    private void OnTriggerEnter2D(Collider2D other) {
        floatingPlatformScript.LinkToPlayer(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other) {
        floatingPlatformScript.DeLinkPlayer();
    }
}
