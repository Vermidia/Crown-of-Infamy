using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveScreen : MonoBehaviour
{
    public TextMeshProUGUI upgradeList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(var upgrade in PlayerData.upgrades)
        {
            upgradeList.text += upgrade + "\n";
        }

        PlayerData.SaveStats();
        SaveManager.SaveToFile(PlayerData.slot);
    }

    public void Continue()
    {
        //TODO maybe don't immediately go back to battle
        SceneManager.LoadScene("BattleScene");
    }

    public void Leave()
    {
        //TODO maybe don't immediately go back to battle
        SceneManager.LoadScene("MainMenuScene");
    }
}
