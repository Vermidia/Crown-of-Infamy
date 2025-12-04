using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Continue()
    {
        //Flush stats to not take save data
        PlayerData.ResetBoss();
        SceneManager.LoadScene("VictoryScene");
    }

    public void Easy()
    {
        PlayerData.difficultyMultiplier = 0.75f;
        Continue();
    }

    public void Normal()
    {
        PlayerData.difficultyMultiplier = 1.0f;
        Continue();
    }

    public void Hard()
    {
        PlayerData.difficultyMultiplier = 1.5f;
        Continue();
    }
}
