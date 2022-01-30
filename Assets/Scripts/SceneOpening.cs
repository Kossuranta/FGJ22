using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOpening : MonoBehaviour
{
    [SerializeField]
    GameObject textCanvas = null;
    
    bool onDoor = false;

    void Awake()
    {
        textCanvas.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Blender.Instance.CurrentState != Blender.State.Idle) return;
        
        if (other.CompareTag("Player"))
        {
            onDoor = true;
            textCanvas.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onDoor = false;
            textCanvas.SetActive(false);
        }
    }
    void Update()
    {
        if(onDoor && Input.GetButton("Interact"))
        {
            Debug.Log("e pressed");
            SceneManager.LoadScene("Level_01");
            onDoor = false;
        }
    }
}

