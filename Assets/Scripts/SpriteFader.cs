using UnityEngine;

public class SpriteFader : MonoBehaviour
{
    [SerializeField]
    ColorEnum color = ColorEnum.None;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();

        if (GameManager.Instance.CurrentLevel != Levels.Hub)
            SpriteRenderer.enabled = false;
    }

    public ColorEnum Color { get { return color; } }

    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            return spriteRenderer;
        }
        private set
        {
            spriteRenderer = value;
        }
    }
}
