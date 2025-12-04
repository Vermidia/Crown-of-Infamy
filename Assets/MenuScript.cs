using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void StartGame(int slot)
    {
        SaveManager.Flush();
        PlayerData.ResetBoss();
        PlayerData.slot = slot;
        if (SaveManager.CheckIfSlotExists(slot))
        {
            SaveManager.LoadFromFile(slot);
            PlayerData.LoadStats();
            SceneManager.LoadScene("SaveScreen");
        }
        else
            SceneManager.LoadScene("DifficultySelectScene");
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("TutorialOne");
    }

    public void StopGame()
    {
       Application.Quit();
    }
}
