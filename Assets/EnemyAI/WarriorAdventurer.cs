using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarriorAdventurer : Adventurer
{
    public override void SetupAdventurer(int infamyAllocation)
    {
        maxHealth = 20 + infamyAllocation * 15;

        maxMagicPoints = (int)(infamyAllocation * 2.5);

        power = 2 + (int)(infamyAllocation * 2.5f);
        spirit = 1 + infamyAllocation * 2;

        speed = 2 + infamyAllocation * 4;

        skills = new() { 
            new MagicAttackSkill() { name = "Sword Slash", power = 10, usesSpirit = false, animation = "MeleeAttackSimple", sound = "Melee", tags = {"Attack"} },
            new MagicBuffSkill() {name = "Guard Stance", targeting = Skill.TargetingType.IncludeSelf, givenStatuses = new(){("Guard", 1)}, animation = "Guard", tags = {"Defense"} } 
        };

        if(infamyAllocation >= 10)
        {
            skills.Add(new MagicAttackSkill() { name = "Swordplay", power = 20, MagicCost = 5, usesSpirit = false, sound = "Melee", animation = "MeleeAttackSimple", tags = {"Attack"} });
            maxHealth += 20;
        }

        if (infamyAllocation >= 20)
        {
            skills.Add(new MagicBuffSkill() { name = "Pinpoint", targeting = Skill.TargetingType.IncludeSelf, givenStatuses = new(){("Fiesty", 3)}, animation = "Charge", sound = "Block", tags = {"SelfBuff"} });
            maxHealth += 100;
            power += 10;
        }

        health = maxHealth;
        magicPoints = maxMagicPoints;
        InitialStatus();
    }

    //Warrior is simple - either attack or guard or self buff
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

        var guardSkillsMarked = usableSkills.Where(x => x.tags.Contains("Defense")).ToList();
        List<MagicBuffSkill> guardSkills = new();

        foreach (var guard in guardSkillsMarked)
        {
            if (guard is MagicBuffSkill {} magicBuff)
            {
                guardSkills.Add(magicBuff);
            }
        }

        //Reverse so the last skill is at the front (as it's likely the best, maybe)
        guardSkills.Reverse();

        //Check guarding
        if(enemies.Any(x => x.statusEffects.ContainsKey("Deadly")))
        {          
            foreach(var guard in guardSkills)
            {    
                if (TryGuardSelf(guard, enemies, allies, out var guarded))
                {
                    targets = guarded;
                    return guard;
                }
            }
        }
        else if (health / (float)maxHealth <= 0.5 && Random.Range(0, 2) == 1) //half chance to attack instead
        {
            //If low and chance based, guard
            foreach(var guard in guardSkills)
            {    
                if (TryGuardSelf(guard, enemies, allies, out var guarded))
                {
                    targets = guarded;
                    return guard;
                }
            }
        }

        var buffskillsMarked = usableSkills.Where(x => x.tags.Contains("SelfBuff")).ToList();

        List<MagicBuffSkill> buffSkills = new();

        foreach (var buff in buffskillsMarked)
        {
            if (buff is MagicBuffSkill {} buffSkill)
            {
                buffSkills.Add(buffSkill);
            }
        }

        foreach (var buff in buffSkills)
        {
            foreach (var status in buff.givenStatuses)
            {
                if(statusEffects.ContainsKey(status.Item1))
                    continue;

                if(buff.DoTargeting(this, enemies, allies, out var possibleTargets))
                {
                    targets = possibleTargets;
                    return buff;
                }
                else
                {
                    targets = new()
                    {
                        this
                    };
                    return buff;
                }
            }
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
        return "Warrior " + base.GetName();
    }
}
