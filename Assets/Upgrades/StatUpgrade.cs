public class StatUpgrade : Upgrade //Handles both types
{
    public int healthUp;

    public int supportUp;

    public int powerUp;

    public int spiritUp;

    public int turnUp;

    public int speedUp;

    public override void OnObtain()
    {
        PlayerData.maxHealth += healthUp;
        PlayerData.maxSupportPoints += supportUp;
        PlayerData.power += powerUp;
        PlayerData.spirit += spiritUp;
        PlayerData.maxTurns += turnUp;
        PlayerData.speed += speedUp;
        
        base.OnObtain();
    }
}