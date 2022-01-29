using System;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    [Serializable]
    public class ColorMaskData
    {
        [SerializeField]
        Transform transform = null;

        [SerializeField]
        ColorEnum color = ColorEnum.None;

        public Transform Transform { get { return transform; } }
        public ColorEnum Color { get { return color; } }
    }
    
    [SerializeField]
    ColorMaskData[] colorMasks = null;

    [SerializeField]
    float colorEnableDuration = 5f;

    float timer = 0;
    ColorMaskData colorToBeEnabled = null;
    GameManager gameManager = null;

    Vector3 currentResolution = new Vector3(1930, 1090, 1);

    public void Setup(GameManager gameManager)
    {
        this.gameManager = gameManager;
        
        //Some extra size to be safe
        int width = Screen.width + 10;
        int height = Screen.height + 10;
        currentResolution = new Vector3(width, height, 1);

        UpdateEnabledColorMasks();
    }

    void UpdateEnabledColorMasks()
    {
        foreach (ColorMaskData colorMaskData in colorMasks)
        {
            if (gameManager.EnabledColors.HasFlag(colorMaskData.Color))
                colorMaskData.Transform.localScale = currentResolution;
        }
    }

    public void EnableColor(ColorEnum color)
    {
        if (gameManager.EnabledColors.HasFlag(color)) return;
        
        colorToBeEnabled = GetColorMaskData(color);
        timer = 0;
    }

    ColorMaskData GetColorMaskData(ColorEnum color)
    {
        foreach (ColorMaskData colorMaskData in colorMasks)
        {
            if (colorMaskData.Color.HasFlag(color))
                return colorMaskData;
        }

        return null;
    }

    public void Update()
    {
        int width = Screen.width + 10;
        int height = Screen.height + 10;

        //If resolution has changed
        if (currentResolution.x != width || currentResolution.y != height)
        {
            UpdateEnabledColorMasks();
        }
        
        if (colorToBeEnabled != null)
        {
            timer += Time.deltaTime / colorEnableDuration;
            colorToBeEnabled.Transform.localScale = Vector3.Lerp(Vector3.zero, currentResolution, timer);

            if (timer >= 1)
                colorToBeEnabled = null;
        }
    }
}
