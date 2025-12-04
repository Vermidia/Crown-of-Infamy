using System.Collections.Generic;

public class Upgrade
{
    public string name;

    public string description;

    public int neededInfamyLevel = 1;

    public bool visibleUpgrade = true;

    public List<string> requirements = new();

    public virtual void OnObtain()
    {
        if (visibleUpgrade)
            PlayerData.upgrades.Add(name);
    }
}
