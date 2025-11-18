using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Adventurer : Combantant
{
    public int maxMagicPoints;
    public int magicPoints;

    public void Start()
    {
    }

#nullable enable
//TODO Temporary
    public override Skill? CheckAttacks(List<Combantant> enemies, List<Combantant> allies, out List<Combantant>? targets)
    {
        targets = null;
        if (skills.Count == 0)
            return null;
        targets = new List<Combantant>() { enemies[0] };
        return skills[0];
    }
#nullable disable

    public virtual void SetupAdventurer(int infamyAllocation)
    {
        maxHealth = 20 + infamyAllocation * 10;
        health = maxHealth;

        maxMagicPoints = 2 + infamyAllocation * 2;
        magicPoints = maxMagicPoints;

        power = 5 + infamyAllocation * 5;
        spirit = 2 + infamyAllocation * 5;

        speed = 10 + infamyAllocation * 5;

        skills = new() { new BasicAttackSkill() { name = "BasicHit", power = 10, animation = "MeleeAttackSimple" } };
    }

#nullable enable
    protected bool TryGuardSelf(MagicBuffSkill skill, List<Combantant> enemies, List<Combantant> allies,
    out List<Combantant>? guardTargets)
    {
        guardTargets = null;
        if(skill.DoTargeting(this, enemies, allies, out var posstargets))
        {
            guardTargets = posstargets;
            return true;
        }
        else
        {
            //Prioritize self
            if (posstargets.Contains(this))
                guardTargets = posstargets;
            else
            {
                return false;
            }
            return true;
        }
    }
#nullable disable
}
