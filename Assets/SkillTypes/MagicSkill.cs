using UnityEngine;
using System.Collections.Generic;

public class MagicSkill : Skill
{
    public int MagicCost = 0;

    public override bool CheckUse(Combantant user)
    {
        if (!base.CheckUse(user))
            return false;

        if (user is not Adventurer { } adventurer)
            return false;
        
        if (MagicCost > adventurer.magicPoints)
            return false;

        return true;
    }

    public override void Resolve(Combantant user, List<Combantant> effected)
    {
        if (user is not Adventurer { } adventurer)
        {
            Debug.LogError($"{this} is not an adventurer but is using a skill that requires MP!");
            return;
        }

        adventurer.magicPoints -= MagicCost;
    }
}
