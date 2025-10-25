using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.Properties;
using UnityEngine.UIElements;

public class Boss : Combantant
{
    public int maxSupportPoints;
    public int supportPoints;
    public List<SupportSkill> supportSkills = new();

    public TMP_Dropdown skillUI;

    public TMP_Dropdown supportUI;

    public int maxTurns = 1;
    public int turns = 1;

    public UIDocument ui;

    public void Start()
    {
        ui.rootVisualElement.Q<ProgressBar>().value = 100;
        if (skills.Count != 0)
        {

            List<TMP_Dropdown.OptionData> options = new();
            foreach (var skill in skills)
            {
                TMP_Dropdown.OptionData newOption = new();
                newOption.text = skill.name;
                options.Add(newOption);
            }

            skillUI.AddOptions(options);
        }
        else
            skillUI.interactable = false;

        if (supportSkills.Count != 0)
        {
            List<TMP_Dropdown.OptionData> sOptions = new();
            foreach (var skill in supportSkills)
            {
                TMP_Dropdown.OptionData newOption = new();
                newOption.text = skill.name;
                sOptions.Add(newOption);
            }

            supportUI.AddOptions(sOptions);
        }
        else
            supportUI.interactable = false;
    }
#nullable enable
    public bool ValidateSkill(string skill, out Skill? foundSkill)
    {
        foundSkill = null;
        if (skills.Find(x => x.name == skill) is { } classicSkill)
            foundSkill = classicSkill;
        else if (supportSkills.Find(x => x.name == skill) is { } supportSkill)
            foundSkill = supportSkill;
        else
            return false;

        return foundSkill.CheckUse(this);

    }
#nullable disable

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        ui.rootVisualElement.Q<ProgressBar>().value = (((float) health) / maxHealth) * 100;
    }
}
