using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOpening : MonoBehaviour
{
    private bool onDoor = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("it IS a door");
            onDoor = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("it WAS a door");
            onDoor = false;
        }
    }
    void Update()
    {
        if(onDoor && Input.GetButton("Interact"))
        {
            Debug.Log("e pressed");
            SceneManager.LoadScene("Level_01");
            // GameManager.Instance.EnterLevel(Levels.Level_01);
            onDoor = false;
        }
    }
}

