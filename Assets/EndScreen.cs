using UnityEngine;

public class EndScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SaveManager.DeleteFile(PlayerData.slot);
    }

    public void Goodbye()
    {
       Application.Quit(); 
    }
}
