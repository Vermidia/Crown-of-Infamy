using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HealerAdventurer : Adventurer
{
    public override void SetupAdventurer(int infamyAllocation)
    {
            maxHealth = 20 + infamyAllocation * 10;

            maxMagicPoints = 20 + infamyAllocation * 5;

            power = 1 + infamyAllocation * 2;
            spirit = 3 + infamyAllocation * 3;

            speed = 5 + infamyAllocation * 7;

            skills = new(){
                new MagicAttackSkill() { name = "Staff Swing", power = 5, usesSpirit = false, animation = "MeleeAttackSimple", tags = {"Attack"}},
                new MagicBuffSkill() {name = "Guard Stance", targeting = Skill.TargetingType.IncludeSelf, givenStatuses = new(){("Guard", 1)}, animation = "Guard", tags = {"Defense"}}
            };

            if (infamyAllocation >= 100)
            {
                skills.Add(new HealSkillAdv{ name = "Angel Light", healPower = 100, MagicCost = 100, animation = "MagicCast", tags = {"Heal", "Single"}, targeting = Skill.TargetingType.Ally | Skill.TargetingType.IncludeSelf});
                //TODO buffing skill?
                skills.Add(new MagicAttackSkill {name = "Sun Ray", power = 50, MagicCost = 25, animation = "MagicCast", tags = {"Attack"}});
            }
            else if(infamyAllocation >= 10)
            {
                skills.Add(new HealSkillAdv{ name = "Prayer", healPower = 50, MagicCost = 20, animation = "MagicCast", tags = {"Heal", "Single"}, targeting = Skill.TargetingType.Ally | Skill.TargetingType.IncludeSelf});
                skills.Add(new HealSkillAdv{ name = "Sooth", healPower = 10, MagicCost = 50, animation = "MagicCast", tags = {"Heal", "Group"}, targeting = Skill.TargetingType.Ally | Skill.TargetingType.Group | Skill.TargetingType.IncludeSelf});
            }

            skills.Add(new HealSkillAdv{ name = "Blessing", healPower = 10, MagicCost = 10, animation = "MagicCast", tags = {"Heal", "Single"}, targeting = Skill.TargetingType.Ally | Skill.TargetingType.IncludeSelf});

            health = maxHealth;
            magicPoints = maxMagicPoints;
    }
    #nullable enable
    public override Skill? CheckAttacks(List<Combantant> enemies, List<Combantant> allies, out List<Combantant>? targets)
    {
        targets = null;

        var usableSkills = skills.ToList();

        foreach (var skill in skills)
        {
            if (skill.CheckUse(this))
                continue;
            
            usableSkills.Remove(skill);
        }

        List<Combantant> healables = allies;
        healables.Add(this);
        var targetingHealList = healables.ToList();

        int healsWanted = healables.Count(x => x.health / (float)x.maxHealth <= 0.75 && x.IsActive());

        //Don't heal if someone can heal before you UNLESS there's other people that also need heals
        //This is done by removing the top priorities from our healable list as we assume someone else will take
        //care of them

        var fasterHealers = allies.Where(x => x.IsActive() && x is HealerAdventurer {} y && y.speed > speed).ToList();

        bool activeHealers = fasterHealers.Count() > 0;

        var guardSkillsMarked = usableSkills.Where(x => x.tags.Contains("Defense")).ToList();;
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
        else if (health / (float)maxHealth <= 0.5 && activeHealers)
        {
            //If low and someones faster than you, guard
            foreach(var guard in guardSkills)
            {    
                if (TryGuardSelf(guard, enemies, allies, out var guarded))
                {
                    targets = guarded;
                    return guard;
                }
            }
        }

        if(activeHealers)
        {
            targetingHealList.Sort((x,y) => (x.health / (float)x.maxHealth).CompareTo(y.health / (float)y.maxHealth));
            for(int i = 0; i < fasterHealers.Count(); i++)
            {
                if (targetingHealList.Count > 0)
                    targetingHealList.RemoveAt(0);
                else
                    break;
            }
        }

        //If heals are wanted, there are no faster active healers,
        if ((healsWanted > 0 && !activeHealers) || healsWanted > 1)
        {
            var healMarked = usableSkills.Where(x => x.tags.Contains("Heal")).ToList();
            List<HealSkillAdv> healSkills = new();

            foreach(var heal in healMarked)
            {
                if (heal is HealSkillAdv {} healingSkill)
                    healSkills.Add(healingSkill);
            }

            //Group heals tried first, then heals
            if (healsWanted > 2)
            {
                //Get group heals
                var groupHeals = healSkills.Where(x => x.tags.Contains("Group")).ToList();
                
                if (groupHeals.Count() > 0)
                {
                    groupHeals.Sort((x, y) => y.healPower.CompareTo(x.healPower));
                    foreach (var groupHeal in groupHeals)
                    {
                        if(TryUseHealSkill(groupHeal, enemies, allies, targetingHealList, out var chosenTargs))
                        {
                            targets = chosenTargs;
                            return groupHeal;
                        }
                    }
                }
            }

            healSkills.Sort((x, y) => y.healPower.CompareTo(x.healPower));
            foreach (var heal in healSkills)
            {
                if(TryUseHealSkill(heal, enemies, allies, targetingHealList, out var chosenTargs))
                {
                    targets = chosenTargs;
                    return heal;
                }
            }
        }

        //TODO buff skill checking?

        //If nothing else is prioritized, do attacks
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
    

    private bool TryUseHealSkill(HealSkillAdv skill, List<Combantant> enemies, List<Combantant> allies, List<Combantant> healList,
    out List<Combantant>? healTargets)
    {
        healTargets = null;
        if (skill.DoTargeting(this, enemies, allies, out var possibleTargets))
        {
            healTargets = possibleTargets;
            foreach(var targ in possibleTargets)
            {
                if(targ.IsActive())
                    continue;

                healTargets.Remove(targ);
            }

            if(healTargets.Count > 0)
                return true;
            else
                return false;
        }
        else
        {
            Combantant? chosenTarget = null;
            foreach(var targ in healList)
                {
                if(!targ.IsActive())
                    continue;
                                
                //Choose target with lowest percent health
                if(chosenTarget == null ||
                (targ.health / (float)targ.maxHealth) < (chosenTarget.health / (float)chosenTarget.maxHealth))
                    chosenTarget = targ;

                }

                if(chosenTarget != null)
                {
                    healTargets = new()
                    {
                        chosenTarget
                    };
                    return true;
                }
        }
        return false;
    }
    #nullable disable

    public override string GetName()
    {
        return "Healer" + base.GetName();
    }
}
