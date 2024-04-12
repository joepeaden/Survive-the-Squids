using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo
{
    public const int LEVEL_TWO_XP_THRESHOLD = 10;
    public const int LEVEL_THREE_XP_THRESHOLD = 20;
    public const int LEVEL_FOUR_XP_THRESHOLD = 30;
    public const int LEVEL_FIVE_XP_THRESHOLD = 40;

    public static bool staticsInitialized = false;

    public CharacterStatsData statsData;

    public enum CharTraits
    {
        Quick,
        Precise,
        Tough,
        Brutal,
        Smart
    }
    List<CharTraits> possibleTraits = new List<CharTraits>();

    // traits this  character has
    public List<CharTraits> traits = new List<CharTraits>();

    [HideInInspector]
    public string charName;
    [HideInInspector]
    public WeaponData weaponData;

    public float ReflexSpeed
    {
        get
        {
            return reflexSpeed + reflexSpeedBuff;
        }
    }
    private float reflexSpeed;
    private float reflexSpeedBuff;

    public int DamageBuff => damageBuff;
    private int damageBuff;

    private float xpGainMultiplier = 1;

    public float CritChance
    {
        get
        {
            return critChanceBuff + critChance;
        }
    }
    private float critChance;
    private float critChanceBuff;

    public int level = 1;
    private int xp;
    private int currentXPThreshold = LEVEL_TWO_XP_THRESHOLD;

    public int TotalHitPoints
    {
        get
        {
            return baseHitPoints + hitPointsBuff;
        }
    }
    private int baseHitPoints;
    private int hitPointsBuff;

    public CharacterInfo(CharacterStatsData newStatsData)
    {
        if (!staticsInitialized)
        {
            possibleTraits = new List<CharTraits>
            {
                CharTraits.Quick,
                CharTraits.Brutal,
                CharTraits.Precise,
                CharTraits.Smart,
                CharTraits.Tough
            };
        }

        statsData = newStatsData;

        AssignRandomName();
        SetWeapon(GameManager.instance.weapons[0]);
        reflexSpeed = statsData.reflexSpeed;
        critChance = statsData.critChance;
        baseHitPoints = statsData.totalHitPoints;
    }

    public void AddXP(int xpAmount)
    {
        xp += (int) (xpAmount * xpGainMultiplier);
        if (xp > currentXPThreshold)
        {
            LevelUp();
            xp = 0;
        }
    }

    public void LevelUp()
    {
        level++;

        switch (level)
        {
            case 2:
                currentXPThreshold = LEVEL_THREE_XP_THRESHOLD;
                break;
            case 3:
                currentXPThreshold = LEVEL_FOUR_XP_THRESHOLD;
                break;
            case 4:
                currentXPThreshold = LEVEL_FIVE_XP_THRESHOLD;
                break;
            default:
                break;
        }

        CharTraits newTrait;
        List<CharTraits> tempPossibleTraits = new List<CharTraits>(possibleTraits);
        do
        {
            newTrait = tempPossibleTraits[Random.Range(0, tempPossibleTraits.Count)];
            tempPossibleTraits.Remove(newTrait);
        } while (traits.Contains(newTrait) && tempPossibleTraits.Count > 0);

        // if it contains it then there are no more traits to give, the char has them all
        if (!traits.Contains(newTrait))
        {
            traits.Add(newTrait);

            if (newTrait == CharTraits.Quick)
            {
                reflexSpeedBuff = Player.instance.quickTraitData.value;
            }
            else if (newTrait == CharTraits.Brutal)
            {
                damageBuff = (int)Player.instance.brutalTraitData.value;
            }
            else if (newTrait == CharTraits.Smart)
            {
                xpGainMultiplier = Player.instance.smartTraitData.value;
            }
            else if (newTrait == CharTraits.Precise)
            {
                critChanceBuff = Player.instance.preciseTraitData.value;
            }
            else if (newTrait == CharTraits.Tough)
            {
                critChanceBuff = Player.instance.toughTraitData.value;
            }
        }

        // i could just update one. But whatever.
        Player.instance.UpdateCharBodies();
    }

    public void SetWeapon(WeaponData newWeapon)
    {
        weaponData = newWeapon;
    }

    private void AssignRandomName()
    {
        List<string> firstNameOptions = new List<string>
        {
            "Dan",
            "Steven",
            "Joe",
            "Bill",
            "Corey",
            "Dylan",
            "Mike",
            "Michah",
            "Chris",
            "Thomas",
            "Bob",
            "James",
            "Diego",
            "Jose",
            "Mary",
            "Pei-Chin",
            "Emily",
            "Jenna",
            "Stephanie",
            "Devyn",
            "Lisa",
            "Louise"
        };

        List<string> lastNameOptions = new List<string>
        {
            "Stevens",
            "Danson",
            "Smith",
            "Ramirez",
            "Vasquez",
            "Ogleson",
            "O'Donnel",
            "Williams",
            "Davis",
            "Miller",
            "Gonzales",
            "Anderson",
            "Lopez",
            "Thomas",
            "Brown",
            "Scott",
            "Torres",
            "Campbell",
            "Parker",
            "Hall",
            "Adams",
            "Rogers"
        };

        string firstName = firstNameOptions[Random.Range(0, firstNameOptions.Count)];
        string lastName = lastNameOptions[Random.Range(0, lastNameOptions.Count)];

        charName = firstName + " " + lastName;
    }
}
