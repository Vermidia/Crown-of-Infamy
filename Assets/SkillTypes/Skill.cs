using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using UnityEngine.Audio;

public class Skill
{
    public string name;

    public int roundCooldown = 0;

    public int startingCooldown = 0;

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

    public string sound = string.Empty;

    public virtual bool CheckUse(Combantant user, out string reason, bool checkCooldown = true)
    {
        reason = string.Empty;
        if (checkCooldown && currentCooldown > 0)
        {
            reason = $"{name} is on cooldown for {currentCooldown} more round(s)!";
            return false;
        }

        return true;
    }

    public virtual void Use(Combantant user, List<Combantant> effected)
    {
        if (!user.TryGetComponent<Animator>(out var animator))
        {
            Debug.LogError($"Missing Animator Component on {user}!");
            user.ErrorAttackComplete();
        }
        if(sound != string.Empty)
        {
            user.audioSource.resource = Resources.Load<AudioResource>($"Sounds/{sound}");
            user.audioSource.pitch = UnityEngine.Random.Range(0.8f,1.2f);
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
        if(sound != string.Empty)
        {
            user.audioSource.Play();
        }
    }

    protected void Statuslogic(Combantant hit, (string, int) status, bool buff)
    {
        if(hit.statusEffects.ContainsKey(status.Item1))
            hit.statusEffects[status.Item1] = Math.Max(hit.statusEffects[status.Item1], status.Item2);
        else
            hit.statusEffects.Add(status.Item1, status.Item2);

        hit.ReceiveStatus(buff);
    }
}
