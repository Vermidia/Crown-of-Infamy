using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.Properties;
using UnityEngine.UIElements;
using System;

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

    public UIDocument support;

    public void Start()
    {
        differentiator = "boss";
        ui.rootVisualElement.Q<ProgressBar>().value = 100;
        support.rootVisualElement.Q<ProgressBar>().value = 100;
        
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
            skillUI.gameObject.SetActive(false);

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
            supportUI.gameObject.SetActive(false);

        InitialStatus();
    }
#nullable enable
    public bool ValidateSkill(string skill, out Skill? foundSkill, out string reason)
    {
        foundSkill = null;
        reason = string.Empty;

        if (skills.Find(x => x.name == skill) is { } classicSkill)
            foundSkill = classicSkill;
        else if (supportSkills.Find(x => x.name == skill) is { } supportSkill)
            foundSkill = supportSkill;
        else
            return false;

        var found = foundSkill.CheckUse(this, out reason);

        return found;

    }
#nullable disable

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        ui.rootVisualElement.Q<ProgressBar>().value = (((float) health) / maxHealth) * 100;
    }

    public override void HealDamage(int heal)
    {
        base.HealDamage(heal);
        ui.rootVisualElement.Q<ProgressBar>().value = (((float) health) / maxHealth) * 100;
    }

    public void ChangeSupportPoints()
    {
        support.rootVisualElement.Q<ProgressBar>().value = (((float) supportPoints) / maxSupportPoints) * 100;
    }

    public override void OnRoundEnd()
    {
        base.OnRoundEnd();

        //Cooldown support
        int cooldownReducer = passives.Contains("No Escape") ? 2 : 1;

        foreach(var skill in skills)
        {
            if (skill.currentCooldown > 0)
                skill.currentCooldown = Math.Max(0, skill.currentCooldown - cooldownReducer);
        }

        foreach(var supports in supportSkills)
        {
            if (supports.currentCooldown > 0)
                supports.currentCooldown = Math.Max(0, supports.currentCooldown - cooldownReducer);
        }

        if(passives.Contains("Immovable"))
        {
            supportPoints = Math.Min(maxSupportPoints, supportPoints / 20);
            ChangeSupportPoints();
        }
    }

    public override string GetName()
    {
        return "Boss";
    }
}
