using UnityEngine;

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
}
