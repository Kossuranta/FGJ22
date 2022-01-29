using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField]
    ColorEnum enabledColors = ColorEnum.Red;

    [SerializeField]
    ColorEnum debugEnableColor = ColorEnum.None;

    ColorManager colorManager = null;

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        
        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
        
        DontDestroyOnLoad(this);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        colorManager = FindObjectOfType<ColorManager>();
        if (colorManager != null)
            colorManager.Setup(this);
    }
    
    public void EnterLevel(Levels level)
    {
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

    public void OnBlenderCompleted()
    {
        if (ColorToBeUnlocked == ColorEnum.None) return;
        
        EnableColor(ColorToBeUnlocked);
        ColorToBeUnlocked = ColorEnum.None;
    }

    void EnableColor(ColorEnum color)
    {
        if (colorManager == null)
        {
            Debug.LogError($"Trying to enabled color {color}, but colorManager is null!");
            return;
        }
        
        if (enabledColors.HasFlag(color))
        {
            enabledColors |= color;
            colorManager.EnableColor(color);
        }
    }

    void Update()
    {
        if (debugEnableColor != ColorEnum.None)
        {
            EnableColor(debugEnableColor);
            debugEnableColor = ColorEnum.None;
        }
    }

    public ColorEnum ColorToBeUnlocked { get; set; } = ColorEnum.None;
    public ColorEnum EnabledColors { get { return enabledColors; } }
}

