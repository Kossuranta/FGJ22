using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    ColorSystem colorSystem = null;

    void Awake()
    {
        Setup();
    }

    void Setup()
    {
        colorSystem = new ColorSystem();
        colorSystem.Setup();
    }
}
