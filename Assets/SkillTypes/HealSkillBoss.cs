using System.Collections.Generic;
/// <summary>
/// Healing skill meant for the boss
/// </summary>
public class HealSkillBoss : SupportSkill
{
    public int healPower;

    public override void Resolve(Combantant user, List<Combantant> effected)
    {
        int damage = healPower + user.spirit / 2; //Combines user and part of the user's spirit

        foreach (var hit in effected)
        {
            hit.HealDamage(damage);
        }

        base.Resolve(user, effected);
    }
}