using System.Collections.Generic;

//Can hold multiple types of upgrades
public class MultiUpgrade : Upgrade
{
    public List<Upgrade> upgrades = new();

    public override void OnObtain()
    {
        foreach(var upgrade in upgrades)
        {
            upgrade.visibleUpgrade = false;
            upgrade.OnObtain();
        }
        base.OnObtain();
    }
}
