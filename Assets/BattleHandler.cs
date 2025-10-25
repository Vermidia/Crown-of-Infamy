using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.Properties;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BattleHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Boss boss;

    public List<Adventurer> adventurers = new();

    public List<Combantant> others = new();

    enum BattleStages
    {
        SelectSkill,
        SelectTargeting,
        PlayBattle
    }

    BattleStages stage = BattleStages.SelectSkill;
#nullable enable
    Skill? chosenSkill = null;
#nullable disable

    List<AttackOrder> orders = new();

    void Start()
    {
        PlayerData.TransferStats(boss);
        PlayerData.RampStats(adventurers);
        NewRound();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Round order
    //NewRound - Enemy AI chooses moves here
    //SkillChosen
    //CheckTargeting - Skips to CreatePlayerAttack on moves that don't need targeting
    //Targeting Selected
    //CreatePlayerAttack
    //RunBattle - Goes back to Waiting for SkillChosen if Boss has more turns
    //ResolveBattle
    //Then it loops

    public void SkillChosen(TMP_Dropdown dropdown)
    {
        if (stage != BattleStages.SelectSkill)
        {
            dropdown.value = 0;
            return;
        }

        var skillName = dropdown.options[dropdown.value].text;
        dropdown.value = 0;
        if (!boss.ValidateSkill(skillName, out var skill))
            return;

        chosenSkill = skill;
        CheckTargeting();
    }

    public void SupportSkillChosen(TMP_Dropdown dropdown)
    {
        if (stage != BattleStages.SelectSkill)
        {
            dropdown.value = 0;
            return;
        }

        var skillName = dropdown.options[dropdown.value].text;
        dropdown.value = 0;
        if (!boss.ValidateSkill(skillName, out var skill))
            return;

        chosenSkill = skill;
        CheckTargeting();
    }

    private void CheckTargeting()
    {
        Debug.Log("Skill chosen, targeting check");
        if ((chosenSkill.targeting ^ Skill.TargetingType.IncludeSelf) == 0)
        {
            //Only self targeting
            CreatePlayerAttack(boss);
            return;
        }

        if ((chosenSkill.targeting & Skill.TargetingType.Group) == Skill.TargetingType.Group)
        {
            //Group targeting
            if (chosenSkill.targeting.HasFlag(Skill.TargetingType.Ally))
                CreatePlayerAttack(others);
            else
                CreatePlayerAttack(adventurers.ToList<Combantant>());
            return;
        }

        stage = BattleStages.SelectTargeting;
        Debug.Log("Targeting allowed");
    }

    public void TargetSelected(Combantant target)
    {
        if (stage != BattleStages.SelectTargeting)
            return;

        Debug.Log("Validating Target");

        if (target == boss && chosenSkill.targeting.HasFlag(Skill.TargetingType.IncludeSelf))
        {
            CreatePlayerAttack(target);
            return;
        }

        if (chosenSkill.targeting.HasFlag(Skill.TargetingType.Ally))
        {
            if (others.Contains(target))
                CreatePlayerAttack(target);
            return;
        }
        else
        {
            if (target is not Adventurer { } enemy)
                return;
            
            if (adventurers.Contains(enemy))
                CreatePlayerAttack(target);
            return;
        }
    }

    private void CreatePlayerAttack(Combantant target)
    {
        Debug.Log("Attack Created");
        orders.Add(new AttackOrder(boss, new() { target }, chosenSkill));
        RunBattle();
    }

    private void CreatePlayerAttack(List<Combantant> targets)
    {
        orders.Add(new AttackOrder(boss, targets, chosenSkill));
        RunBattle();
    }

    private void RunBattle()
    {

        if (--boss.turns <= 0)
        {
            ResolveBattle();
            return;
        }

        Debug.Log("Turns remaining, select skill");
        //Still has turns, next one
        stage = BattleStages.SelectSkill;
    }

    private void ResolveBattle()
    {
        Debug.Log("Now resolving moves");
        stage = BattleStages.PlayBattle;

        orders.Sort((x, y) => x.user.speed.CompareTo(y.user.speed));

        DoAttack();
    }

    private void DoAttack()
    {
        if (orders.Count == 0)
        {
            NewRound();
            return;
        }

        var attack = orders[0];

        if (!attack.user.IsActive())
        {
            orders.Remove(attack);
            DoAttack();
            return;
        }

        attack.usedSkill.Use(attack.user, attack.effected);
    }

    private void AttackComplete(Combantant fighter)
    {
        if (orders.Count == 0)
        {
            NewRound();
            return;
        }

        //Ignore it if it's from the wrong orders
        if (orders[0].user != fighter)
            return;

        orders.RemoveAt(0);

        DoAttack();
    }

    private void NewRound()
    {
        Debug.Log("New round started");
        if (adventurers.All(x => x.health <= 0))
        {
            //TODO make infamy gained variable
            PlayerData.infamy++;
            SceneManager.LoadScene("VictoryScene");
            return;
        }
        else if (boss.health <= 0)
        {
            SceneManager.LoadScene("GameOver");
            return;
        }

        boss.turns = boss.maxTurns;

        //TODO AI here
        var enemy = new List<Combantant> { boss };
        enemy.AddRange(others);
        foreach (var adv in adventurers)
        {
            var allies = adventurers.ToList<Combantant>();
            allies.Remove(adv);
            var skill = adv.CheckAttacks(enemy, allies, out var targets);

            if (skill != null)
                orders.Add(new AttackOrder(adv, targets, skill));
        }

        stage = BattleStages.SelectSkill;
    }

}
