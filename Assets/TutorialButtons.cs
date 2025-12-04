using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButtons : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TutorialTwo()
    {
        SceneManager.LoadScene("TutorialTwo");
    }

    public void EndTutorial()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
