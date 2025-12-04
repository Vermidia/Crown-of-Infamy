using System.Collections.Generic;
/// <summary>
/// Generic magic attacking skill. Used for physical ones as well for easy power calcs.
/// </summary>
public class MagicAttackSkill : MagicSkill
{
    public int power;

    public bool usesSpirit = true;

    public override void Resolve(Combantant user, List<Combantant> effected)
    {
        int damage = GetCalculatedPower(user); //Combines user and  the user's spirit/power

        foreach (var hit in effected)
        {
            hit.TakeDamage(damage);
        }

        base.Resolve(user, effected);
    }

    public int GetCalculatedPower(Combantant user)
    {
        return power + (usesSpirit ? user.spirit : user.power) * (user.statusEffects.ContainsKey("Deadly") ? 3 : 1) * (user.statusEffects.ContainsKey("Fiesty") ? 2 : 1);
    }
}