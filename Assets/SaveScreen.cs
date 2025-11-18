using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Continue()
    {
        //TODO maybe don't immediately go back to battle
        SceneManager.LoadScene("BattleScene");
    }
}
