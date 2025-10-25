using UnityEngine;
using System;
using System.Collections.Generic;

public class Skill
{
    public string name;

    public string id;

    public int turnCooldown = 0;

    public int currentCooldown = 0;

    public string animation;

    //Default is single enemy
    public TargetingType targeting = TargetingType.Enemy;

    // Group and Allies are mostly for readability, and don't mean anything really
    [Flags]
    public enum TargetingType : byte
    {
        None = 0, //Should never appear!
        IncludeSelf = 1,
        Group = 2,
        Ally = 4,
        Enemy = 8,
    }

    public virtual bool CheckUse(Combantant user)
    {
        if (currentCooldown > 0)
            return false;

        return true;
    }

    public virtual void Use(Combantant user, List<Combantant> effected)
    {
        if (!user.TryGetComponent<Animator>(out var animator))
        {
            Debug.LogError($"Missing Animator Component on {user}!");
            user.ErrorAttackComplete();
        }

        animator.Play(animation);
    }
}
