using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {
    [SerializeField] private Animator _playerAnimator;
    private bool isMoving=false, isRolling=false;


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
}
