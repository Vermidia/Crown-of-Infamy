using System.Collections.Generic;
using System.Linq;

public class MageAdventurer : Adventurer
{
    public override void SetupAdventurer(int infamyAllocation)
    {
        maxHealth = 10 + infamyAllocation * 10;

        maxMagicPoints = 10 + (infamyAllocation * 5);

        power = 1 + infamyAllocation * 1;
        spirit = 2 + (int)(infamyAllocation * 2.5);

        speed = 3 + infamyAllocation * 5;

        skills = new() { 
            new MagicAttackSkill() { name = "Wand Smack", power = 5, usesSpirit = false, animation = "MeleeAttackSimple", sound = "Melee", tags = {"Attack"} },
            new MagicAttackSkill {name = "Spark", power = 15, MagicCost = 10, animation = "MagicCast", sound = "MagicAttack", tags = {"Attack"}}
        };

        if(infamyAllocation >= 10)
        {
            skills.Add(new MagicAttackSkill {name = "Spark Storm", power = 35, MagicCost = 20, animation = "MagicCast", sound = "MagicAttack", tags = {"Attack"}});
            speed += 10;
        }

        if (infamyAllocation >= 20)
        {
            skills.Add(new MagicAttackSkill {name = "Spark Storm Sparrow", power = 65, MagicCost = 30, animation = "MagicCast", sound = "MagicAttack", tags = {"Attack"}});
            speed += 20;
            spirit += 40;
        }

        health = maxHealth;
        magicPoints = maxMagicPoints;
        InitialStatus();
    }

    //Mage has big moves and not much else
    #nullable enable
    public override Skill? CheckAttacks(List<Combantant> enemies, List<Combantant> allies, out List<Combantant>? targets)
    {
        targets = null;

        var usableSkills = skills.ToList();

        foreach (var skill in skills)
        {
            if (skill.CheckUse(this, out _))
                continue;
            
            usableSkills.Remove(skill);
        }

        var damageSkillsMarked = usableSkills.ToList();
        List<MagicAttackSkill> damageSkills = new();

        foreach(var skill in damageSkillsMarked)
        {
            if (skill is MagicAttackSkill {} attackSkill)
                    damageSkills.Add(attackSkill);
        }

        if(damageSkills.Count > 0)
        {
            damageSkills.Sort((x, y) => y.GetCalculatedPower(this).CompareTo(x.GetCalculatedPower(this)));

            foreach (var damaging in damageSkills)
            {
                if(damaging.DoTargeting(this, enemies, allies, out var possibleTargets))
                {
                    targets = possibleTargets;
                    return damaging;
                }
                else
                {
                    targets = new()
                    {
                        enemies[0] //TODO if minions are real change targeting
                    };
                    return damaging;
                }
            }
        }

        return null;
    }
    #nullable disable

    public override string GetName()
    {
        return "Mage " + base.GetName();
    }
}
