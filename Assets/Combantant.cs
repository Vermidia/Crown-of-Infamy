using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
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

    //If there is no animator
    public void ErrorAttackComplete()
    {
        SendMessageUpwards("AttackComplete", this);
    }

    public virtual void TakeDamage(int damage)
    {
        health = Math.Max(0, health - damage);
        if(health <= 0)
            GetComponent<Animator>().Play("Downed");
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
#nullable disable
}
