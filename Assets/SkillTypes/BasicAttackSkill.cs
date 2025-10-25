using System.Collections.Generic;
public class BasicAttackSkill : Skill
{
    public int power;

    public override void Use(Combantant user, List<Combantant> effected)
    {
        base.Use(user, effected);
        int damage = power + user.power; //Combines user and skill power

        foreach (var hit in effected)
        {
            hit.TakeDamage(damage);
        }
    }
}