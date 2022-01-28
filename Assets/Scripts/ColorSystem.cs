using System.Collections.Generic;
using UnityEngine;

public class ColorSystem
{
    ColorComponent[] colorComponents = null;
    readonly List<ColorEnum> activeColors = new ();

    public void Setup()
    {
        colorComponents = Object.FindObjectsOfType<ColorComponent>();
    }

    public void ActivateColor(ColorEnum color)
    {
        if (activeColors.Contains(color)) return;
        activeColors.Add(color);
        
        ActivateColorComponents(color);
    }

    void ActivateColorComponents(ColorEnum color)
    {
        foreach (ColorComponent colorComponent in colorComponents)
        {
            colorComponent.Activate(color);
        }
    }

    public List<ColorEnum> ActiveColors { get { return activeColors; } }
}
