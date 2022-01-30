using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField]
    ColorEnum enabledColors = ColorEnum.Red;

    [SerializeField]
    ColorEnum debugEnableColor = ColorEnum.None;

    ColorManager colorManager = null;
    ColorEnum colorToBeUnlocked = ColorEnum.None;

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

        if (scene.name.Equals("Level_01"))
            DisableCollectedVegetables();
    }
    
    void DisableCollectedVegetables() 
    {
        GameObject[] levelEndBases = GameObject.FindGameObjectsWithTag("LevelEndBase");

        foreach (GameObject levelEndBase in levelEndBases)
        {   
            ColorEnum flagColor = levelEndBase.GetComponent<LevelEnd>().GetCaptureTargetColor();
            Debug.Log("Verrataan " + flagColor);
            if(enabledColors.HasFlag(flagColor)) {
                //never gets here for some reason
                Debug.Log("disabloidaan " + flagColor);
                levelEndBase.SetActive(false);
            }
        }
    }

    public void EnterLevel(Levels level)
    {
        switch(level)
        {
            case Levels.Hub:
                CurrentLevel = Levels.Hub;
                SceneManager.LoadScene("Hub");
                break;
            
            case Levels.Level_01:
                CurrentLevel = Levels.Level_01;
                SceneManager.LoadScene("Level_01");
                break;
        }
    }

    public void OnBlenderCompleted()
    {
        Debug.Log("OnBlenderCompleted");
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
        
        Debug.Log($"EnableColor({color})");
        if (!enabledColors.HasFlag(color))
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

    public ColorEnum ColorToBeUnlocked
    {
        get
        {
            return colorToBeUnlocked;
        }
        set
        {
            Debug.Log($"ColorToBeUnlocked set to {colorToBeUnlocked}");
            colorToBeUnlocked = value;
        }
    }

    public ColorEnum EnabledColors { get { return enabledColors; } }
    public Levels CurrentLevel { get; private set; }
}

