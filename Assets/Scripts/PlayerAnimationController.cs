using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private SpriteRenderer playersSpriteRenderer;
    private bool isMoving=false, isRolling=false, isDead=false, isFlipped=false;

    public void setSpriteFlipX(bool Flip) {
        if (Flip!=isFlipped) {
            if(Flip) {
                playersSpriteRenderer.flipX=true;
                isFlipped=true;
            } 
            else {
                playersSpriteRenderer.flipX=false;
                isFlipped=false;
            }
        }
    }

    public void startMoving() {
        if(!isMoving) {
            _playerAnimator.SetBool("Moving", true);
            isMoving = true;
        }
    }

    public void stopMoving() {
        if(isMoving) {
            _playerAnimator.SetBool("Moving", false);
            isMoving = false;
        }
    }

    public void startRolling() {
        if(!isRolling) {
            _playerAnimator.SetBool("Rolling", true);
            isRolling = true;
            Debug.Log("started rolling");
        }
    }  

    public void stopRolling() {
        if (isRolling) {
            _playerAnimator.SetBool("Rolling", false);
            isRolling = false;
        }
    }

    public void playerDies() {
        if (!isDead) {
            _playerAnimator.SetTrigger("Death");
            isDead = true;
        }
    }
}
