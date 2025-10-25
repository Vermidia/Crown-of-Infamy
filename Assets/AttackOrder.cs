using System.Collections.Generic;

public struct AttackOrder
{
    public Combantant user;

    public List<Combantant> effected;

    public Skill usedSkill;

    public AttackOrder(Combantant attacker, List<Combantant> defenders, Skill skill)
    {
        user = attacker;
        effected = defenders;
        usedSkill = skill;
    }
}
