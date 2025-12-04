using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ChoiceHandler : MonoBehaviour
{
    public UIDocument ui;

    //TODO have variable button amounts for difficulties
    public List<GameObject> buttons = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Dictionary<GameObject, Upgrade> buttonToChoice = new();
    void Start()
    {
        ui.rootVisualElement.Q<ProgressBar>().value = ((float)PlayerData.InfamyAtCurrentLevel() / 10) * 100;
        List<Upgrade> upgrades;
        if(PlayerData.lastInfamyLevel < PlayerData.InfamyLevel())
        {
            PlayerData.lastInfamyLevel = PlayerData.InfamyLevel();
            upgrades = PlayerData.GetPossibleUpgrades(true);
        }
        else
            upgrades = PlayerData.GetPossibleUpgrades();

        foreach (var button in buttons)
        {
            var random = Random.Range(0, upgrades.Count);

            var upgrade = upgrades[random];
            upgrades.RemoveAt(random);

            var text = button.GetComponentInChildren<TMP_Text>();
            text.text = $"{upgrade.name}\n\n{upgrade.description}";
            buttonToChoice.Add(button, upgrade);
        }
    }
    
    public void ChoiceChosen(GameObject button)
    {
        var upgrade = buttonToChoice[button];
        upgrade.OnObtain();
        //TODO maybe don't immediately go back to battle
        SceneManager.LoadScene("SaveScreen");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
