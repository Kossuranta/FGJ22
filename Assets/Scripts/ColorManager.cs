using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    [SerializeField]
    float colorEnableDuration = 5f;
    
    float timer = 0;
    GameManager gameManager = null;

    readonly Dictionary<ColorEnum, List<SpriteFader>> spriteFadersDictionary = new();
    ColorEnum colorToBeFadedIn = ColorEnum.None;

    void Awake()
    {
        SpriteFader[] spriteFaders = FindObjectsOfType<SpriteFader>();

        foreach (SpriteFader spriteFader in spriteFaders)
        {
            if (!spriteFadersDictionary.ContainsKey(spriteFader.Color))
                spriteFadersDictionary.Add(spriteFader.Color, new List<SpriteFader>());
            
            spriteFadersDictionary[spriteFader.Color].Add(spriteFader);
        }
    }

    public void Setup(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    void Start()
    {
        UpdateEnabledColorMasks();
    }

    void UpdateEnabledColorMasks()
    {
        foreach (KeyValuePair<ColorEnum, List<SpriteFader>> kvp in spriteFadersDictionary)
        {
            if (gameManager.EnabledColors.HasFlag(kvp.Key))
            {
                foreach (SpriteFader spriteFader in kvp.Value)
                {
                    Color color = spriteFader.SpriteRenderer.color;
                    color.a = 0;
                    spriteFader.SpriteRenderer.color = color;
                }
            }
        }
    }

    public void EnableColor(ColorEnum color)
    {
        Debug.Log($"FadeIn color: {color}");
        colorToBeFadedIn = color;
        timer = 0;
    }

    public void Update()
    {
        if (colorToBeFadedIn == ColorEnum.None) return;
        
        timer += Time.deltaTime / colorEnableDuration;

        if (spriteFadersDictionary.TryGetValue(colorToBeFadedIn, out List<SpriteFader> spriteFaders))
        {
            foreach (SpriteFader spriteFader in spriteFaders)
            {
                Color color = spriteFader.SpriteRenderer.color;
                color.a = Mathf.Lerp(1f, 0f, timer);
                spriteFader.SpriteRenderer.color = color;
            }
        }

        if (timer >= 1)
            colorToBeFadedIn = ColorEnum.None;
    }
}
