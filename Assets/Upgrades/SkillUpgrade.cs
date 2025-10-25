public class SkillUpgrade : Upgrade //Handles both types
{
    public Skill skill;

    public override void OnObtain()
    {
        if (skill is SupportSkill { } support)
            PlayerData.supportSkills.Add(support);
        else
            PlayerData.skills.Add(skill);

        base.OnObtain();
    }
}