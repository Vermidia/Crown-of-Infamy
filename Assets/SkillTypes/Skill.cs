using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Skill
{
    public string name;

    public string id;

    public int roundCooldown = 0;

    public int currentCooldown = 0;

    public string animation;

    //Default is single enemy
    public TargetingType targeting = TargetingType.Enemy;

    public List<string> tags = new(); //For AI

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

/// <summary>
/// Returns true if all possible targets are guaranteed targets (for group moves or self targeting moves)
/// </summary>
/// <param name="user"></param>
/// <param name="enemies"></param>
/// <param name="allies"></param>
/// <param name="FinalTargets"></param>
/// <returns></returns>
    public bool DoTargeting(Combantant user, List<Combantant> enemies, List<Combantant> allies, out List<Combantant> possibleTargets)
    {
        possibleTargets = new();
        //Can only target self
        if ((targeting ^ TargetingType.IncludeSelf) == 0)
        {
            possibleTargets.Add(user);
            return true;
        }
        else if(targeting.HasFlag(TargetingType.IncludeSelf))
        {
            possibleTargets.Add(user);
        }

        if(targeting.HasFlag(TargetingType.Ally))
            possibleTargets.AddRange(allies);

        if(targeting.HasFlag(TargetingType.Enemy))
            possibleTargets.AddRange(enemies);

        if(targeting.HasFlag(TargetingType.Group))
        {
            return true;
        }

        return false;
    }

    public virtual void Resolve(Combantant user, List<Combantant> effected)
    {
        
    }
}
