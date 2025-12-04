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
                Statuslogic(hit, status, buff);
            }
        }

        base.Resolve(user, effected);
    }
}