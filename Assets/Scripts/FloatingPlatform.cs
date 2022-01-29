using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 pointA = new Vector3(-2, 0, 0);

    [SerializeField] private Vector3 pointB = new Vector3(2, 0, 0);

    [SerializeField][Range(0,5)] private float movingSpeed = 0;
    private float t;
    public Transform playersTransform;
    private Vector3 lastPos;

    public void LinkToPlayer(GameObject player) {
        playersTransform = player.transform;
    }

    void Update() {        
        t += Time.deltaTime * movingSpeed;
        
        transform.position = Vector3.Lerp(pointA, pointB, t);
        if(playersTransform) playersTransform.position = playersTransform.position + Vector3.Lerp(pointA, pointB, t)-lastPos;
            
        lastPos = Vector3.Lerp(pointA, pointB, t);

        if (t >= 1) {
        var b = pointB;
        var a = pointA;
        pointA = b;
        pointB = a;
        t = 0;
        }
    }
}
