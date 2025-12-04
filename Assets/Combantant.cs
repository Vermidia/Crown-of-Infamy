using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Combantant : MonoBehaviour
{
    public int maxHealth;

    public int health;

    public int speed;

    public int power;

    public int spirit;

    public List<Skill> skills = new();

#nullable enable
    public Skill? queuedMove;

    public List<Combantant>? targets;
#nullable disable

    public Dictionary<string, int> statusEffects = new();

    public List<string> passives = new();

    public ParticleSystem hurt;
    public ParticleSystem heal;
    public ParticleSystem debuff;
    public ParticleSystem buff;

    public AudioSource audioSource;

    public string differentiator;

    void Start()
    {

    }

    public void InitialStatus()
    {
        if(passives.Contains("First Blood"))
            statusEffects.Add("First Blood", 2);
    }

    public int GetSpeed()
    {
        return (int)((speed + (statusEffects.ContainsKey("First Blood") ? 5000 : 0)) * (statusEffects.ContainsKey("Sluggish") ? 0.5f : 1));
    }

    void OnMouseUpAsButton()
    {
        SendMessageUpwards("TargetSelected", this);
    }

    void OnAttackComplete()
    {
        GetComponent<Animator>().Play("Idle");
        SendMessageUpwards("AttackComplete", this);    
    }

    //Does not reset animation
    void OnAttackCompleteSpecial()
    {
        SendMessageUpwards("AttackComplete", this);
    }

    //If there is no animator
    public void ErrorAttackComplete()
    {
        SendMessageUpwards("AttackComplete", this);
    }

    public virtual void TakeDamage(int damage)
    {
        //Guarding halves damage
        if(statusEffects.ContainsKey("Guard"))
            damage /= 2;
        
        if(health > 0)
        {
            health = Math.Max(0, health - damage);
            hurt.Play();
        }

        if(health <= 0)
        {
            if(!passives.Contains("Twin Lives"))
                GetComponent<Animator>().Play("Downed");
            else
            {
                heal.Play();
                passives.Remove("Twin Lives");
                PlayerData.passives.Remove("Twin Lives");
                health = maxHealth / 2;
            }
        }
    }

    public virtual void HealDamage(int heal)
    {
        if(!IsActive()) //No heal on downed
            return;
        
        health = Math.Min(maxHealth, health + (int)(heal * (statusEffects.ContainsKey("Curse") ? 0.5f : 1)));
        this.heal.Play();
    }

    public virtual void ReceiveStatus(bool statusBuff)
    {
        if(statusBuff)
            buff.Play();
        else
            debuff.Play();
    }

    /// <summary>
    /// Returns if the combantant can use skills
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        if (health <= 0)
            return false;

        return true;
    }

#nullable enable
    public virtual Skill? CheckAttacks(List<Combantant> enemies, List<Combantant> allies, out List<Combantant>? targets)
    {
        targets = null;
        return null;
    }

    public void UseSkill(Skill skill, List<Combantant> effected)
    {
        queuedMove = skill;
        targets = effected;
        skill.Use(this, effected);
    }

    public void ResolveSkill()
    {
        if (queuedMove != null)
            queuedMove.Resolve(this, targets);

        queuedMove = null;
        targets = null;
    }
#nullable disable

    public virtual void OnTurnEnd()
    {
        
    }

    public virtual void OnRoundEnd()
    {
        //Status handling
        List<string> toRemove = new();
        var statusEffectsCopy = statusEffects.ToDictionary(x => x.Key, x => x.Value);
        foreach (var key in statusEffectsCopy.Keys)
        {
            statusEffects[key]--;
            if(statusEffects[key] <= 0)
            {
                statusEffects.Remove(key);
            //"Preparing" becomes a new status called Deadly when removed!
            if (key == "Preparing")
                statusEffects.Add("Deadly", 1);
            }
        }

        if(IsActive())
            GetComponent<Animator>().Play("Idle"); //Revert to idle anim if we aren't for some reason (guarding etc)
    }

    public virtual string GetName()
    {
        return differentiator;
    }
}
