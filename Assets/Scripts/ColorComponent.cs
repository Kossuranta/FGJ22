using UnityEngine;

public class ColorComponent : MonoBehaviour
{
    [SerializeField]
    ColorEnum colorSetting = ColorEnum.None;

    bool isActive = false;

    public void Activate(ColorEnum color)
    {
        if (isActive) return;
        if (!colorSetting.HasFlag(color)) return;

        isActive = true;

        // Do stuff to show colored version
        
        return;
    }

    public ColorEnum ColorSetting { get { return colorSetting; } }
}
