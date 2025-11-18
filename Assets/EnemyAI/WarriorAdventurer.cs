using UnityEngine;

public class WarriorAdventurer : Adventurer
{
    public override void SetupAdventurer(int infamyAllocation)
    {
        maxHealth = 50 + infamyAllocation * 25;

        maxMagicPoints = (int)(infamyAllocation * 2.5);

        power = 5 + infamyAllocation * 5;
        spirit = 1 + infamyAllocation * 2;

        speed = 2 + infamyAllocation * 4;

        skills = new() { 
            new MagicAttackSkill() { name = "Sword Slash", power = 10, usesSpirit = false, animation = "MeleeAttackSimple", tags = {"Attack"} },
            new MagicBuffSkill() {name = "Guard Stance", targeting = Skill.TargetingType.IncludeSelf, givenStatuses = new(){("Guard", 1)}, animation = "Guard", tags = {"Defense"} } 
        };

        health = maxHealth;
        magicPoints = maxMagicPoints;
    }

    //TODO warrior AI

    public override string GetName()
    {
        return "Warrior" + base.GetName();
    }
}
