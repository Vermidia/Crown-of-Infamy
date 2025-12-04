using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class PlayerData
{
    public static double infamy = 0;

    public static int lastInfamyLevel = 1;

    public static int maxHealth = 600;

    public static int maxSupportPoints = 10;

    public static int maxTurns = 1;

    public static int power = 20;

    public static int spirit = 10;

    public static int speed = 30;

    public static List<Skill> skills = new() { new BasicAttackSkill() {name = "Bite", power = 20, animation = "MeleeAttackSimple", sound = "Melee"} };

    public static List<SupportSkill> supportSkills = new();

    public static List<string> passives = new();

    public static List<string> upgrades = new();

    public static double difficultyMultiplier = 1.0;

    public static int slot = 1;

    //All upgrades obtainable, right now
    
    public static List<Upgrade> possibleCapstoneUpgrades = new()
    {
        //More turns
        new StatUpgrade(){name = "One more turn", description = "You get to use another skill per round", turnUp = 1},
        new StatUpgrade(){name = "One last turn", description = "You get to use another skill per round (3 total!)", turnUp = 1,
        requirements = new(){"One more turn"}},

        //Healing
        new MultiUpgrade
        {
            name = "Regeneration",
            description = "A support skill that heals you. Uses spirit. Cost 10 SP. You also gain more spirit and max health.",
            upgrades =
            {
                new StatUpgrade(){name = "Regeneration stats", spiritUp = 30, healthUp = 1000},
                new SkillUpgrade(){name = "Regeneration skill", 
        skill = new HealSkillBoss(){name = "Regeneration", animation = "Howl", sound = "Heal", healPower = 200, SupportCost = 10, targeting = Skill.TargetingType.IncludeSelf}},
            }
            
        },
        
        //First Strike
        new MultiUpgrade
        {
            name = "First Blood",
            description = "For the first round, you always go first. Gain much more speed and some power.",
            upgrades =
            {
                new StatUpgrade(){name = "First Blood stats", speedUp = 40, powerUp = 15},
                new PassiveUpgrade(){name = "First Blood passive", passive = "First Blood"},
            }
        },

        //Moon Strike
        new MultiUpgrade
        {
            name = "Full Moon Strike",
            description = "A super strong strike that mirrors a cresecent moon hitting all enemies, becomes available on round 5. 5 round cooldown. You also gain a ton of max SP for taking this.",
            upgrades =
            {
                new StatUpgrade(){name = "First Blood stats", supportUp = 50},
                new SkillUpgrade(){name = "Full Moon Strike attack",
        skill = new BasicAttackSkill(){name = "Full Moon Strike", animation = "Crescent", power = 100, startingCooldown = 5, roundCooldown = 5, sound = "Melee", targeting = Skill.TargetingType.Group | Skill.TargetingType.Enemy}}
            }
        }
    };
    public static List<Upgrade> possibleUpgrades = new()
    {
        //Infamy Level 1 Upgrades
        new MultiUpgrade()
        {
            name = "Swift jaws",
            description = "Your bite becomes more powerful, and you become a little faster",
            upgrades = {
            new SkillRemover(){name = "Swift jaws remover", skillName = "Bite"},
            new SkillUpgrade()
            {
                name = "Swift jaws skill",
                skill = new BasicAttackSkill() {name = "Bite", power = 30, animation = "MeleeAttackSimple", sound = "Melee"}
            },
            new StatUpgrade(){name = "Swift jaws stats", speedUp = 20},
            },
        },

        new StatUpgrade(){name = "Inner focus", description = "Gain more max SP, spirit, and max health.", spiritUp = 10, healthUp = 200, supportUp = 10},

        new SkillUpgrade(){name = "Wolf's Curse", description = "Makes enemies regain less health from healing for 4 rounds. 3 round cooldown, 5 SP",
        skill = new SupportBuffSkill() 
        {name = "Wolf's Curse", buff = false, givenStatuses = {("Curse", 4)}, animation = "Howl", sound = "HowlBasic", roundCooldown = 3, SupportCost = 5, targeting = Skill.TargetingType.Group | Skill.TargetingType.Enemy}},
        new StatUpgrade(){name = "Power of Curses", description = "Gain more spirit and power", requirements = new(){"Wolf's Curse"}, powerUp = 15, spiritUp = 15},

        new StatUpgrade(){name = "Forest runner", description = "Become faster and stronger", powerUp = 10, speedUp = 15},

        new SkillUpgrade(){name = "Wide slash", description = "A weak claw attack that hits all enemies. 3 round cooldown.",
        skill = new BasicAttackSkill() {name = "Wide Slash", power = 10, animation = "Slash", roundCooldown = 3, sound = "Melee", targeting = Skill.TargetingType.Group | Skill.TargetingType.Enemy}},
        new StatUpgrade(){name = "Sharpened claws", description = "Sharper claws make your attacks stronger", powerUp = 20, requirements = new(){"Wide slash"}},

        new StatUpgrade(){name = "Tough hide", description = "Gain more max health", healthUp = 300},
        new StatUpgrade(){name = "Tougher hide", description = "Gain even more max health", healthUp = 600, requirements = new(){"Tough hide"}},
        new StatUpgrade(){name = "Toughest hide", description = "Gain lots of max health!", healthUp = 800, requirements = new(){"Tougher hide"}},

        new SkillUpgrade(){name = "Mighty Howl", description = "Makes your attacks do 2x damage for 4 turns. 3 round cooldown, 7 SP",
        skill = new SupportBuffSkill()
        {name = "Mighty Howl", givenStatuses = {("Fiesty", 4)}, animation = "Howl", sound = "HowlBasic", roundCooldown = 3, SupportCost = 7, targeting = Skill.TargetingType.IncludeSelf}},
        new SkillUpgrade(){name = "Raging vines", description = "Attack with fragile magical vines, hitting all enemies.", skill = new BasicAttackSkill()
        {name = "Raging vines", usesSpirit = true, power = 10, animation = "Howl", sound = "MagicAttack", targeting = Skill.TargetingType.Group | Skill.TargetingType.Enemy}},
        new MultiUpgrade()
        {
            name = "Raging forest",
            description = "Attack with magical vines, hitting everyone on the field. Also increases your spirit.",
            requirements = new() {"Raging vines"},
            upgrades = new()
            {
                new SkillRemover() {name = "Raging forest remover", skillName = "Raging vines"},
                new StatUpgrade() {name = "Raging forest stats", spiritUp = 20},
                new SkillUpgrade() {name = "Raging forest skill", skill = new BasicAttackSkill()
                {name = "Raging forest", usesSpirit = true, power = 30, animation = "Howl", sound = "MagicAttack", targeting = Skill.TargetingType.Group | Skill.TargetingType.IncludeSelf | Skill.TargetingType.Enemy | Skill.TargetingType.Ally}
                }
            }
        },

        //Infamy Level 2 Upgrades
        new SkillUpgrade(){name = "Anticipation", neededInfamyLevel = 2, description = "Prepare a strong strike. Next round do x3 damage, but enemies may try to block your attacks! Cooldown 3 rounds.",
        skill = new SupportBuffSkill(){name = "Anticipation", animation = "Howl",
        roundCooldown = 3, SupportCost = 5, givenStatuses = new() {("Preparing", 1)}, sound = "Ticktock", targeting = Skill.TargetingType.IncludeSelf}},

        new StatUpgrade(){name = "Terrifying Strength", neededInfamyLevel = 2, description = "Gain more max health and power", healthUp = 300, powerUp = 25},

        new StatUpgrade(){name = "Forest sprinter", neededInfamyLevel = 2, description = "Become much faster", speedUp = 25, requirements = new(){"Forest runner"}},

        new PassiveUpgrade(){name = "Desperation", neededInfamyLevel = 2, description = "Gain 2x power when below 1/2 max health.", passive = "Desperation"},

        new StatUpgrade(){name = "True focus", neededInfamyLevel = 2, description = "Gain alot more max SP", supportUp = 40},

        new MultiUpgrade()
        {
            name = "Spirit howl", neededInfamyLevel = 2, description = "A deep howl that damages a single enemy. Uses spirit. You get spirit when you take this.",
            upgrades = new()
            {
                new StatUpgrade(){name = "Spirit howl stats", spiritUp = 20},
                new SkillUpgrade(){name = "Spirit howl skill", skill = new BasicAttackSkill()
                {name = "Spirit howl", animation = "Howl",  sound = "HowlMagic", power = 30, usesSpirit = true}}
            }
        },

        new MultiUpgrade()
        {
            name = "Wide slash perfection", description = "Wide slash becomes much stronger.",
            neededInfamyLevel = 2,
            requirements = new(){"Wide slash"},
            upgrades = new()
            {
                new SkillRemover(){name = "Wide slash perfection remover",
                skillName = "Wide Slash"},
                new SkillUpgrade()
                {name = "Wide slash perfection skill",
                skill = new BasicAttackSkill() {name = "Wide Slash", power = 30, animation = "Slash", sound = "Melee", roundCooldown = 3, targeting = Skill.TargetingType.Group | Skill.TargetingType.Enemy}},
            }
        },

        new PassiveUpgrade(){name = "Twin Lives", neededInfamyLevel = 2, description = "The first time you would die, you come back with half health. You lose this passive afterwards.", passive = "Twin Lives"},

        new PassiveUpgrade(){name = "Tick Tock", neededInfamyLevel = 2, description = "You no longer have to spend SP for Support Skills, but you die after round 5. There is no way to prevent this.", passive = "Tick Tock"},

        new SkillUpgrade(){name = "Murder", neededInfamyLevel = 2, description = "A careful strike that does a lot of damage to one target. 20 round cooldown", skill =
        new BasicAttackSkill() {name = "Murder", power = 500, animation = "MeleeAttackSimple", sound = "Melee", roundCooldown = 20}},

        //Infamy Level 3 Upgrades

        new MultiUpgrade()
        {
            name = "Rejuvenation",
            neededInfamyLevel = 3,
            description = "Your Regeneration becomes Rejuvenation, healing twice as much but requiring 20 SP",
            requirements = new() {"Regeneration"},
            upgrades = new()
            {
                new SkillRemover(){name = "Rejuvenation remover", skillName = "Regeneration"},
new SkillUpgrade(){name = "Regeneration skill", 
        skill = new HealSkillBoss(){name = "Rejuvenation", animation = "Howl", sound = "Heal", healPower = 400, SupportCost = 20, targeting = Skill.TargetingType.IncludeSelf}},
            }
        },

        new StatUpgrade(){name = "Horror", neededInfamyLevel = 3, requirements = new(){"Terrifying Strength"}, description = "Gain max health, power, and spirit", healthUp = 500, powerUp = 20, spiritUp = 20},

        new MultiUpgrade()
        {
            name = "Jaws of terror",
            neededInfamyLevel = 3,
            description = "Your bite becomes even stronger, and your single target attacks cause targets to stop attacking in fear.",
            requirements = new(){"Swift jaws"},
            upgrades = new()
            {
                new SkillRemover(){name = "Jaws of terror remover", skillName = "Bite"},
                new SkillUpgrade()
                {
                    name = "Jaws of terror skill",
                    skill = new BasicAttackSkill() {name = "Bite", power = 50, sound = "Melee", animation = "MeleeAttackSimple"}
                },
                new PassiveUpgrade(){name = "Jaws of terror passive", passive = "Jaws of Terror"}
            }
        },

        new MultiUpgrade()
        {
            name = "Moon's Curse",
            neededInfamyLevel = 3,
            description = "Your Wolf's Curse becomes Moon's Curse, halving enemy healing and reducing their speed to half for 4 rounds. 3 Round cooldown. 7 SP. Also increases your spirit.",
            requirements = new(){"Wolf's Curse"},
            upgrades = new()
            {
                new SkillRemover(){name = "Moon's Curse remover", skillName = "Wolf's Curse"},
                new SkillUpgrade(){name = "Moon's Curse skill",
                skill = new SupportBuffSkill() 
                {name = "Moon's Curse", buff = false, givenStatuses = {("Curse", 4), ("Sluggish", 4)}, sound = "HowlBasic", animation = "Howl", roundCooldown = 3, SupportCost = 7, targeting = Skill.TargetingType.Group | Skill.TargetingType.Enemy}},

                new StatUpgrade(){name = "Moon's Curse stats", spiritUp = 30}
            }
        },

        new StatUpgrade(){name = "Forest hunter", neededInfamyLevel = 3, requirements = new(){"Forest sprinter"}, description = "Increases your power greatly", powerUp = 30},

        new StatUpgrade(){name = "Finality", neededInfamyLevel = 3, requirements = new(){"Regeneration"}, description = "Increases your max health greatly", healthUp = 1000},

        new PassiveUpgrade(){name = "No Escape", neededInfamyLevel = 3, description = "Round Cooldown for moves goes down by 2 at the end of rounds", passive = "No Escape"},

        new PassiveUpgrade(){name = "Immovable", neededInfamyLevel = 3, description = "You passively gain some SP at the end of the round.", passive = "Immovable"},

        new MultiUpgrade()
        {
            name = "No Time",
            neededInfamyLevel = 3,
            description = "You gain an extra turn and alot of power and spirit, but you die after round 3. There is no way to prevent this.",
            requirements = new(){"Tick Tock"},
            upgrades = new()
            {
                new PassiveUpgrade(){name = "No Time passive", passive = "No Time"},
                new StatUpgrade(){name = "No Time stats", spiritUp = 30, powerUp = 30, turnUp = 1}
            }
        },

        new MultiUpgrade
        {
            name = "Total Eclipse of the Sun",
            neededInfamyLevel = 3,
            requirements = new(){"Full Moon Strike"},
            description = "An extremely strong strike that mirrors the sun's corona, hitting all enemies, becomes available on round 7. 7 round cooldown.",
            upgrades =
            {
                new SkillRemover(){name = "Total Eclipse of the Sun remover", skillName = "Full Moon Strike"},
                new SkillUpgrade(){name = "Total Eclipse of the Sun attack",
        skill = new BasicAttackSkill(){name = "Total Eclipse of the Sun", animation = "Crescent", sound = "Melee", power = 200, startingCooldown = 7, roundCooldown = 7, targeting = Skill.TargetingType.Group | Skill.TargetingType.Enemy}}
            }
        }

    };

    public static int InfamyLevel()
    {
        return (int)(infamy / 10) + 1;
    }

    public static int InfamyAtCurrentLevel()
    {
        return (int)(infamy - ((InfamyLevel() - 1) * 10));
    }

    public static void ResetBoss()
    {
        infamy = 0;
        lastInfamyLevel = 1;
        maxHealth = 600;
        maxSupportPoints = 10;
        maxTurns = 1;
        power = 20;
        spirit = 10;
        speed = 30;
        skills = new() { new BasicAttackSkill() {name = "Bite", power = 20, animation = "MeleeAttackSimple", sound = "Melee"} };
        supportSkills = new();
        upgrades = new();
        passives = new();
    }

    public static List<Upgrade> GetPossibleUpgrades(bool capstone = false)
    {
        List<Upgrade> possibilities = capstone ? possibleCapstoneUpgrades.ToList() : possibleUpgrades.ToList(); //Dupe

        possibilities.RemoveAll(x => x.neededInfamyLevel > InfamyLevel());
        //Remove ones we already have
        possibilities.RemoveAll(x => upgrades.Contains(x.name));
        //Remove any with invalid prerequisites
        for(int i = 0; i < possibilities.Count; i++)
        {
            bool success = true;
            //Probably a bit slow but works for now
            foreach (var req in possibilities[i].requirements)
                if(!upgrades.Contains(req))
                {
                    success = false;
                    break;
                }
            if (!success)
            {
                possibilities.RemoveAt(i);
                i--;
            }
        }

        return possibilities;
    }

    public static void TransferStats(Boss boss)
    {
        boss.maxHealth = maxHealth;
        boss.health = maxHealth;

        boss.maxSupportPoints = maxSupportPoints;
        boss.supportPoints = maxSupportPoints;

        boss.maxTurns = maxTurns;

        boss.power = power;
        boss.spirit = spirit;
        boss.speed = speed;

        boss.skills = skills;
        boss.supportSkills = supportSkills;
        boss.passives = passives;

        foreach(var skill in skills)
        {
            skill.currentCooldown = skill.startingCooldown;
        }

        foreach(var support in supportSkills)
        {
            support.currentCooldown = support.startingCooldown;
        }
    }

    public static void SaveStats()
    {
        SaveManager.SaveDouble("infamy", infamy);
        SaveManager.SaveInt("lastInfamy", lastInfamyLevel);
        SaveManager.SaveDouble("difficultyMult", difficultyMultiplier);

        int totalUpgrades = 0;

        foreach(var up in upgrades)
        {
            string name = "upgrade" + totalUpgrades++;
            SaveManager.SaveString(name, up);
        }

        SaveManager.SaveInt("totalUpgrades", totalUpgrades);
    }

    public static void LoadStats()
    {
        infamy = SaveManager.LoadDouble("infamy");
        lastInfamyLevel = SaveManager.LoadInt("lastInfamy");
        difficultyMultiplier = SaveManager.LoadDouble("difficultyMult");

        List<string> tempList = new();

        for (int i = 0; i < SaveManager.LoadInt("totalUpgrades"); i++)
        {
            string key = "upgrade" + i;
            tempList.Add(SaveManager.LoadString(key));
        }

        //Now load them all!

        foreach(var up in tempList)
        {
            if (possibleUpgrades.Any(x => x.name == up))
            {
                var uppy = possibleUpgrades.Find(x => x.name == up);
                uppy.OnObtain();
            }

            if (possibleCapstoneUpgrades.Any(x => x.name == up))
            {
                var uppy = possibleCapstoneUpgrades.Find(x => x.name == up);
                uppy.OnObtain();
            }
        }
    }
}
