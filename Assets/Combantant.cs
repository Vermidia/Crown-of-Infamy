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

    public ParticleSystem hurt;
    public ParticleSystem heal;
    public ParticleSystem debuff;
    public ParticleSystem buff;

    public string differentiator;

    void Start()
    {

    }

    void OnMouseUpAsButton()
    {
        SendMessageUpwards("TargetSelected", this);
    }

    void OnAttackComplete()
    {
        SendMessageUpwards("AttackComplete", this);
        GetComponent<Animator>().Play("Idle");
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
        
        health = Math.Max(0, health - damage);
        hurt.Play();
        if(health <= 0)
            GetComponent<Animator>().Play("Downed");
    }

    public virtual void HealDamage(int heal)
    {
        if(!IsActive()) //No heal on downed
            return;
        
        health = Math.Min(maxHealth, health + heal);
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
