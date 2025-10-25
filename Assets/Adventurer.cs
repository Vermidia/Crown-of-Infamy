using System.Collections.Generic;
using UnityEngine;

public class Adventurer : Combantant
{
    public int maxMagicPoints;
    public int magicPoints;
    public List<MagicSkill> magicSkills = new();

    public void Start()
    {
    }

#nullable enable
//TODO Temporary
    public override Skill? CheckAttacks(List<Combantant> enemies, List<Combantant> allies, out List<Combantant>? targets)
    {
        targets = null;
        if (skills.Count == 0)
            return null;
        targets = new List<Combantant>() { enemies[0] };
        return skills[0];
    }
#nullable disable
}
