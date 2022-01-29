using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void BtnStartGame()
    {
        GameManager.Instance.EnterLevel(Levels.Hub);
    }

    public void BtnQuitGame()
    {
        Application.Quit();
    }
}
