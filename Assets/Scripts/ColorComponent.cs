using System;
using UnityEngine;

public class ColorComponent : MonoBehaviour
{
    [SerializeField]
    ColorEnum colorSetting = ColorEnum.None;

    [SerializeField]
    Transform colorMask = null;

    bool isActive = false;
    float colorMaskFill = 0;
    float colorMaskStartPosY = 0;
    
    void Awake()
    {
        enabled = false;
        colorMaskStartPosY = colorMask.localPosition.y;
    }

    public void Activate(ColorEnum color)
    {
        if (isActive) return;
        if (!colorSetting.HasFlag(color)) return;

        isActive = true;
        enabled = true;
    }

    
    void Update()
    {
        colorMaskFill += Time.deltaTime / 5f;
        Vector3 colorMaskPos = colorMask.localPosition;
        colorMaskPos.y = Mathf.Lerp(colorMaskStartPosY, 0f, colorMaskFill);
        colorMask.localPosition = colorMaskPos;

        if (colorMaskFill >= 1)
        {
            enabled = false;
        }
    }

    public ColorEnum ColorSetting { get { return colorSetting; } }
}
