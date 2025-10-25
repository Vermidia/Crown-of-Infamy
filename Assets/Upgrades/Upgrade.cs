using System.Collections.Generic;

public class Upgrade
{
    public string name;

    public string description;

    public int neededInfamy = 0;

    public List<string> requirements = new();

    public virtual void OnObtain()
    {
        PlayerData.upgrades.Add(name);
    }
}
