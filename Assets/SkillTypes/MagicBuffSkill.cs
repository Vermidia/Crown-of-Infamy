using System;
using System.Collections.Generic;
/// <summary>
/// Skills that can cost MP and give a status to a target
/// </summary>
public class MagicBuffSkill : MagicSkill
{
    public List<(string, int)> givenStatuses = new();

    public bool buff = true;

    public override void Resolve(Combantant user, List<Combantant> effected)
    {
        foreach (var hit in effected)
        {
            foreach(var status in givenStatuses)
            {
                //If a status exists already, only make it last longer if possible
                if(hit.statusEffects.ContainsKey(status.Item1))
                    hit.statusEffects[status.Item1] = Math.Max(hit.statusEffects[status.Item1], status.Item2);
                else
                    hit.statusEffects.Add(status.Item1, status.Item2);

                hit.ReceiveStatus(buff);
            }
        }

        base.Resolve(user, effected);
    }
}