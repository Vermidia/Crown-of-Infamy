using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Boss boss;

    public List<Adventurer> adventurers = new();

    public List<Combantant> others = new();

    public List<GameObject> possibleAdventurers = new();

    public List<GameObject> spawnSpot = new();

    public Timeline timeline;

    public List<Combantant> lastOrder = new();

    public TextMeshProUGUI battleLog;

    public TextMeshProUGUI playerHelper;

    public TextMeshProUGUI roundTracker;

    public Light mainLight;

    public int roundNumber = 0;

    public AudioSource audioSource;

    public AudioSource buttonSuccess;

    enum BattleStages
    {
        SelectSkill,
        SelectTargeting,
        PlayBattle
    }

    BattleStages stage = BattleStages.SelectSkill;
#nullable enable
    Skill? chosenSkill = null;

    List<Combantant>? possibleTargets = null;
#nullable disable

    List<AttackOrder> orders = new();

    float difficultyDeviation = 0.2f;

    void Start()
    {
        PlayerData.TransferStats(boss);

        char differ = 'A';
        
        foreach (var spot in spawnSpot)
        {
            //TODO balance parties
            var adventurer = Instantiate(possibleAdventurers[UnityEngine.Random.Range(0, possibleAdventurers.Count)], spot.transform);
            var adven = adventurer.GetComponent<Adventurer>();
            adventurers.Add(adven);
            adven.differentiator = $"{differ++}";
        }

        var allocatedInfamy = PlayerData.infamy;

        var minInfamy = (int)(allocatedInfamy - Math.Floor(allocatedInfamy * difficultyDeviation));

        var maxInfamy = (int)(allocatedInfamy + Math.Ceiling(allocatedInfamy * difficultyDeviation));

        if(allocatedInfamy < 10)
        {
            mainLight.colorTemperature = 20000;
            mainLight.intensity = 2;
            mainLight.color = Color.white;
        }
        else if(allocatedInfamy < 20)
        {
            mainLight.colorTemperature = 20000;
            mainLight.intensity = 1;
            mainLight.color = Color.cyan;
        }
        else
        {
            mainLight.colorTemperature = 1500;
            mainLight.intensity = 5;
            mainLight.color = Color.white;
        }
        
        foreach(var adv in adventurers)
        {
            adv.SetupAdventurer(UnityEngine.Random.Range(minInfamy, maxInfamy + 1));
        }

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
        if (!boss.ValidateSkill(skillName, out var skill, out var reason))
        {
            playerHelper.text = reason;
            return;
        }

        chosenSkill = skill;
        buttonSuccess.Play();
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
        if (!boss.ValidateSkill(skillName, out var skill, out var reason))
        {
            playerHelper.text = reason;
            return;
        }

        chosenSkill = skill;
        buttonSuccess.Play();
        CheckTargeting();
    }

    private void CheckTargeting()
    {
        //Debug.Log("Skill chosen, targeting check");

        if(chosenSkill.DoTargeting(boss, adventurers.ToList<Combantant>(), others, out var possibleTargets))
        {
            CreatePlayerAttack(possibleTargets);
            return;
        }

        this.possibleTargets = possibleTargets;

        stage = BattleStages.SelectTargeting;
        playerHelper.text = "Select Target(s)";
        //Debug.Log("Targeting allowed");
    }

    public void TargetSelected(Combantant target)
    {
        if (stage != BattleStages.SelectTargeting)
            return;

        //Debug.Log("Validating Target");

        if(!possibleTargets.Contains(target))
            return;

        buttonSuccess.Play();
        CreatePlayerAttack(target);
    }

    private void CreatePlayerAttack(Combantant target)
    {
        //Debug.Log("Attack Created");
        orders.Add(new AttackOrder(boss, new() { target }, chosenSkill));
        chosenSkill.currentCooldown = chosenSkill.roundCooldown;
        RunBattle();
    }

    private void CreatePlayerAttack(List<Combantant> targets)
    {
        orders.Add(new AttackOrder(boss, targets, chosenSkill));
        chosenSkill.currentCooldown = chosenSkill.roundCooldown;
        RunBattle();
    }

    private void RunBattle()
    {

        if (--boss.turns <= 0)
        {
            ResolveBattle();
            return;
        }

        //Debug.Log("Turns remaining, select skill");
        //Still has turns, next one
        stage = BattleStages.SelectSkill;
        playerHelper.text = $"Select a Skill, {(boss.maxTurns - boss.turns) + 1}!" ;
    }

    private void ResolveBattle()
    {
        //Debug.Log("Now resolving moves");
        stage = BattleStages.PlayBattle;
        playerHelper.text = "Watch and learn...";

        orders.Sort((x, y) => y.user.GetSpeed().CompareTo(x.user.GetSpeed()));

        foreach(var order in orders)
        {
            //Debug.Log(order.user.GetName());
        }

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

        if(!lastOrder.Contains(attack.user))
            lastOrder.Add(attack.user);

        //Cannot do a move under the effects of fear
        if (!attack.user.IsActive() || !attack.usedSkill.CheckUse(attack.user, out _, false) 
        || attack.user.statusEffects.ContainsKey("Fear"))
        {
            orders.Remove(attack);
            DoAttack();
            return;
        }

        attack.user.UseSkill(attack.usedSkill, attack.effected);
        //Debug.Log($"{attack.user.GetName()} used {attack.usedSkill.name}!");
        battleLog.text = $"{attack.user.GetName()} used {attack.usedSkill.name}!";
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

    private void RoundTrackerUpdate()
    {
        //TODO tick tock sounds for those abilities
        roundTracker.text = "Round " + roundNumber;
    }

    private void NewRound()
    {
        roundNumber++;
        RoundTrackerUpdate();
        //Debug.Log("New round started");
        if (adventurers.All(x => x.health <= 0))
        {
            PlayerData.infamy += 2 * PlayerData.difficultyMultiplier;
            if(PlayerData.InfamyLevel() >= 4)
            {
                //Victory
                SceneManager.LoadScene("FinaleScene");
                return;
            }
            else
                SceneManager.LoadScene("VictoryScene");
            return;
        }
        else if (boss.health <= 0 || (boss.passives.Contains("Tick Tock") && roundNumber > 5)
        || (boss.passives.Contains("No Time") && roundNumber > 3))
        {
            SceneManager.LoadScene("GameOver");
            return;
        }

        boss.OnRoundEnd();
        foreach (var adv in adventurers)
        {
            adv.OnRoundEnd();
        }

        boss.turns = boss.maxTurns;

        if(boss.passives.Contains("Tick Tock"))
            audioSource.Play();

        //Make list for timeline

        List<string> timings = new();

        foreach (var order in lastOrder)
        {
            timings.Add(order.differentiator);
        }

        timeline.UpdateTimeline(timings);
        lastOrder.Clear();

        var enemy = new List<Combantant> { boss };
        enemy.AddRange(others);
        foreach (var adv in adventurers)
        {
            var allies = adventurers.ToList<Combantant>();
            allies.Remove(adv);
            var skill = adv.CheckAttacks(enemy, allies, out var targets);

            if (skill != null)
            {
                orders.Add(new AttackOrder(adv, targets, skill));
                string targetNames = "";
                foreach (var targ in targets)
                {
                    targetNames += targ.GetName() + " ";
                }
                //Debug.Log($"{adv.GetName()} is using {skill.name} on {targetNames}");
            }
        }

        stage = BattleStages.SelectSkill;
        playerHelper.text = "Select a Skill";
    }

}
