using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PlayerData
{
    public static int infamy = 0;

    public static int maxHealth = 200;

    public static int maxSupportPoints = 10;

    public static int maxTurns = 1;

    public static int power = 10;

    public static int spirit = 0;

    public static int speed = 5;

    public static List<Skill> skills = new() { new BasicAttackSkill() {name = "Bite", power = 20, animation = "MeleeAttackSimple"} };

    public static List<SupportSkill> supportSkills = new();

    public static List<string> upgrades = new();

    public static int nextInfamyLevel = 10;

    //All upgrades obtainable, right now

    public static List<Upgrade> possibleUpgrades = new()
    {
        new StatUpgrade(){name = "More health", description = "Gives you more health", healthUp = 200},
        new StatUpgrade(){requirements = new(){"More health"}, name = "Even more health", description = "Gives you even more health", healthUp = 250 },

        new StatUpgrade(){name = "Power", description = "Gives you more power", powerUp = 10},

        new StatUpgrade(){name = "Swiftness", description = "Gives you more speed", speedUp = 10},

        new StatUpgrade(){name = "Surge", description = "Increases all stats slightly", powerUp = 5, spiritUp = 5, healthUp = 50, speedUp = 5, supportUp = 5},

        new SkillUpgrade(){name = "Cold Wind", description = "A skill that blows a chill wind", skill = new BasicAttackSkill()
        {name = "Cold Wind", power = 10, animation = "MeleeAttackSimple", targeting = Skill.TargetingType.Group | Skill.TargetingType.Enemy}},

        new SkillUpgrade(){name = "Howl", description = "Does nothing yet", skill = new SupportSkill()
        {name = "Howl", animation = "MeleeAttackSimple", targeting = Skill.TargetingType.IncludeSelf}},
    };

    //TODO make levels
    public static int InfamyLevel()
    {
        return 0;
    }

    public static List<Upgrade> GetPossibleUpgrades()
    {
        List<Upgrade> possibilities = possibleUpgrades.ToList(); //Dupe

        possibilities.RemoveAll(x => x.neededInfamy > InfamyLevel());
        //Remove ones we already have
        possibilities.RemoveAll(x => upgrades.Contains(x.name));
        //Remove any with invalid prerequisites
        for(int i = 0; i < possibilities.Count; i++)
        {
            bool success = true;
            //Probably a bit slow but works for now
            foreach (var req in possibilities[i].requirements)
                if(!upgrades.Contains(req))
                {
                    success = false;
                    break;
                }
            if (!success)
            {
                possibilities.RemoveAt(i);
                i--;
            }
        }

        return possibilities;
    }

    public static void TransferStats(Boss boss)
    {
        boss.maxHealth = maxHealth;
        boss.health = maxHealth;

        boss.maxSupportPoints = maxSupportPoints;
        boss.supportPoints = maxSupportPoints;

        boss.maxTurns = maxTurns;

        boss.power = power;
        boss.spirit = spirit;
        boss.speed = speed;

        boss.skills = skills;
        boss.supportSkills = supportSkills;
    }
}
