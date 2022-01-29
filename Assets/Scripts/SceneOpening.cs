using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneOpening : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Door_01"))
        {
            Debug.Log("asdasdasd");
            if (Input.GetButton("E"))
            {
                GameManager.Instance.EnterLevel(Levels.Level_01);
            }
        }
    }
}

