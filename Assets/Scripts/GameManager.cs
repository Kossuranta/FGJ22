using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        DontDestroyOnLoad(this);
        Instance = this;
        Setup();
    }

    void Setup()
    {
        colorSystem = new ColorSystem();
        colorSystem.Setup();
    }
    public void EnterLevel(Levels level)
    {
        LastLevel = CurrentLevel;
        CurrentLevel = level;
        
        switch(level)
        {
            case Levels.Hub:
                SceneManager.LoadScene("Hub");
                break;
            
            case Levels.Level_01:
                SceneManager.LoadScene("Level_01");
                break;
        }
    }

    public Levels LastLevel { get; private set; } = Levels.Hub;
    public Levels CurrentLevel { get; private set; } = Levels.Hub;
}

