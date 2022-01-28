using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    
    ColorSystem colorSystem = null;

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        
        Instance = this;
        Setup();
    }

    void Setup()
    {
        colorSystem = new ColorSystem();
        colorSystem.Setup();
    }
}
