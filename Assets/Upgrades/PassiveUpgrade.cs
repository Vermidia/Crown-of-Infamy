public class PassiveUpgrade : Upgrade
{
    public string passive;

    public override void OnObtain()
    {
        PlayerData.passives.Add(passive);
        
        base.OnObtain();
    }
}