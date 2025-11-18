using System.Collections.Generic;
public class BasicAttackSkill : Skill
{
    public int power;

    public override void Resolve(Combantant user, List<Combantant> effected)
    {
        base.Resolve(user, effected);
        int damage = (power + user.power) * 
        (user.statusEffects.ContainsKey("Deadly") ? 3 : 1); //Combines user and skill power

        foreach (var hit in effected)
        {
            hit.TakeDamage(damage);
        }
    }
}