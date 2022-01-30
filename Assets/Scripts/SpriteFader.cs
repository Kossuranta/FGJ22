using UnityEngine;

public class SpriteFader : MonoBehaviour
{
    [SerializeField]
    ColorEnum color = ColorEnum.None;

    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public ColorEnum Color { get { return color; } }
    public SpriteRenderer SpriteRenderer { get; private set; }
}
