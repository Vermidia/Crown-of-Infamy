using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        SaveManager.DeleteFile(PlayerData.slot);
    }
    public void Leave()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
