using System.Collections.Generic;
public class BasicAttackSkill : Skill
{
    public int power;

    public bool usesSpirit = false;

    public override void Resolve(Combantant user, List<Combantant> effected)
    {
        int damage = GetCalculatedPower(user);

        foreach (var hit in effected)
        {
            hit.TakeDamage(damage);
            if(user.passives.Contains("Jaws of Terror") &&
            !targeting.HasFlag(TargetingType.Group) && targeting.HasFlag(TargetingType.Enemy))
            {
                //Jaws of terror debuffs :D
                Statuslogic(hit, ("Fear", 1), false);
            }
        }

        base.Resolve(user, effected);
    }

    public int GetCalculatedPower(Combantant user)
    {
        return power + (usesSpirit ? user.spirit : user.power) * (user.statusEffects.ContainsKey("Deadly") ? 3 : 1) * (user.statusEffects.ContainsKey("Fiesty") ? 2 : 1)
        * (user.passives.Contains("Desperation") && (float)user.health / user.maxHealth < 0.5f ? 2 : 1);
    }
}