using TMPro;
using UnityEngine;

public class SlotButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    public int slot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SaveManager.CheckIfSlotExists(slot))
            buttonText.text = "Load Slot " + slot;
        else
            buttonText.text = "Start Slot " + slot;
    }
}
