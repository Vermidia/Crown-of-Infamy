using System.Linq;

public class SkillRemover : Upgrade //Handles both types
{
    public string skillName;

    public override void OnObtain()
    {
        PlayerData.supportSkills.RemoveAll(x => x.name == skillName);
        PlayerData.skills.RemoveAll(x => x.name == skillName);

        base.OnObtain();
    }
}