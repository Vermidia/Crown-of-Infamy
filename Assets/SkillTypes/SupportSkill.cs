using UnityEngine;
using System.Collections.Generic;

public class SupportSkill : Skill
{
    public int SupportCost = 0;

    public override bool CheckUse(Combantant user, out string reason, bool checkCooldown = false)
    {
        if (!base.CheckUse(user, out reason, checkCooldown))
            return false;

        if (user is not Boss { } boss)
            return false;
        
        if (SupportCost > boss.supportPoints)
        {
            reason = $"{name} costs too much SP!";
            return false;
        }

        return true;
    }

    public override void Resolve(Combantant user, List<Combantant> effected)
    {
        if (user is not Boss { } boss)
        {
            Debug.LogError($"{this} is not an boss but is using a skill that requires SP!");
            return;
        }

        boss.supportPoints -=  user.passives.Contains("Tick Tock") ? 0 : SupportCost;
        boss.ChangeSupportPoints();
        base.Resolve(user, effected);
    }
}
