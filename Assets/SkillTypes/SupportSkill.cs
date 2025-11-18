using UnityEngine;
using System.Collections.Generic;

public class SupportSkill : Skill
{
    public int SupportCost = 0;

    public override bool CheckUse(Combantant user)
    {
        if (!base.CheckUse(user))
            return false;

        if (user is not Boss { } boss)
            return false;
        
        if (SupportCost > boss.supportPoints)
            return false;

        return true;
    }

    public override void Resolve(Combantant user, List<Combantant> effected)
    {
        if (user is not Boss { } boss)
        {
            Debug.LogError($"{this} is not an boss but is using a skill that requires SP!");
            return;
        }

        boss.supportPoints -= SupportCost;
    }
}
