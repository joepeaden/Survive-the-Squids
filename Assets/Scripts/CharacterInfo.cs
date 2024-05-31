using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    public enum StatType
    {
        HP,
        CRD,
        AIM,
        STR
    }

    public class CharacterInfo
    {
        public const int LEVEL_TWO_XP_THRESHOLD = 10;
        public const int LEVEL_THREE_XP_THRESHOLD = 20;
        public const int LEVEL_FOUR_XP_THRESHOLD = 30;
        public const int LEVEL_FIVE_XP_THRESHOLD = 40;

        public static bool staticsInitialized = false;

        public CharacterStatsData statsData;

        public string ID;

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
                float crdStatReflexBuff = CrdStat * 5;
                return reflexSpeed + crdStatReflexBuff + weaponData.rotationSpeedPenalty;
            }
        }
        private float reflexSpeed;
        //private float reflexSpeedBuff;

        float reloadBuff;
        public float ReloadTimeReduction
        {
            get
            {
                return reloadBuff;//CrdStat * .1f;
            }
        }

        float projNumBuff;
        public float ProjNumBuff => projNumBuff;

        float projSpreadMod;
        public float ProjSpreadMod => projSpreadMod;

        float rangeBuff;
        public float RangeBuff => rangeBuff;

        float fireRateBuff;
        public float FireRateBuff => fireRateBuff;

        public float DamageBuff => damageBuff;
        private float damageBuff;

        private float xpGainMultiplier = 1;

        public float CritChance
        {
            get
            {
                float critChanceFromAim = AimStat * .1f; 
                return  critChanceFromAim + baseCritChance;
            }
        }
        private float baseCritChance;
        //private float critChanceAim;

        public int level = 1;
        public int xp;
        public int kills;
        public int currentXPThreshold = LEVEL_TWO_XP_THRESHOLD;

        public int TotalHitPoints
        {
            get
            {
                return baseHitPoints + HpStat;
            }
        }
        private int baseHitPoints;

        public bool hasPenetratorRounds;
        public bool hasStunRounds;
        public bool hasSlamRounds;

        public int HpStat;
        public int StrStat;
        public int AimStat;
        public int CrdStat;

        public int pendingLevelUps;

        public CharacterBody currentBody;

        public HashSet<CharacterUpgrade> upgrades = new();

        
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

            ID = Guid.NewGuid().ToString();

            statsData = newStatsData;

            AssignRandomName();
            SetWeapon(GameplayManager.Instance.weapons[0]);
            reflexSpeed = statsData.reflexSpeed;
            baseCritChance = statsData.critChance;
            baseHitPoints = statsData.totalHitPoints;
        }

        public void TallyKill(EnemyData enemy)
        {
            AddXP(enemy.xpReward);
            kills++;

            GameplayManager.Instance.AddPlayerScore(10);
        }

        public void AddXP(int xpAmount)
        {
            xp += (int)(xpAmount * xpGainMultiplier);
            if (xp > currentXPThreshold)
            {
                //LevelUp();
                xp = 0;
            }
        }

        public void UpgradeStat(StatType stat, int value)
        {
            switch (stat)
            {
                case StatType.HP:
                    HpStat += value;
                    break;
                case StatType.STR:
                    StrStat += value;
                    break;
                case StatType.AIM:
                    AimStat += value;
                    break;
                case StatType.CRD:
                    CrdStat += value;
                    break;
            }

            //pendingLevelUps--;
            UpdateBody();
        }

        public void LevelUp()
        {
            //Debug.Log("Character leveled up:  " + charName + " LVL " + level);
            level++;
            //pendingLevelUps++;

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

            UpgradeStat(StatType.HP, 1);


            Color textColor;
            ColorUtility.TryParseHtmlString("#b4d8f7", out textColor);

            //currentBody = Player.instance.GetCharBodyByID(ID);
            currentBody.PlayLevelUpSound();

            GameplayUI.Instance.AddTextFloatup(currentBody.transform.position, "Level Up!", textColor);

            //CharTraits newTrait;
            //List<CharTraits> tempPossibleTraits = new List<CharTraits>(possibleTraits);
            //do
            //{
            //    newTrait = tempPossibleTraits[UnityEngine.Random.Range(0, tempPossibleTraits.Count)];
            //    tempPossibleTraits.Remove(newTrait);
            //} while (traits.Contains(newTrait) && tempPossibleTraits.Count > 0);

            //// if it contains it then there are no more traits to give, the char has them all
            //if (!traits.Contains(newTrait))
            //{
            //    traits.Add(newTrait);

            //    if (newTrait == CharTraits.Quick)
            //    {
            //        reflexSpeedBuff = Player.instance.quickTraitData.value;
            //    }
            //    else if (newTrait == CharTraits.Brutal)
            //    {
            //        damageBuff = (int)Player.instance.brutalTraitData.value;
            //    }
            //    else if (newTrait == CharTraits.Smart)
            //    {
            //        xpGainMultiplier = Player.instance.smartTraitData.value;
            //    }
            //    else if (newTrait == CharTraits.Precise)
            //    {
            //        critChanceBuff = Player.instance.preciseTraitData.value;
            //    }
                //else if (newTrait == CharTraits.Tough)
                //{
                //    critChanceBuff = Player.instance.toughTraitData.value;
                //}
            //}

            UpdateBody();
        }

        public void AddCharacterUpgrade(CharacterUpgradeData upgrade)
        {
            switch (upgrade.upgradeType)
            {
                case CharacterUpgrade.Damage:
                    damageBuff += upgrade.value;
                    break;
                case CharacterUpgrade.FireRate:
                    fireRateBuff += upgrade.value;
                    break;
                case CharacterUpgrade.Range:
                    rangeBuff += upgrade.value;
                    break;
                case CharacterUpgrade.ReloadSpeed:
                    reloadBuff += upgrade.value;
                    break;
                case CharacterUpgrade.ShotgunRounds:
                    projNumBuff += upgrade.value;
                    projSpreadMod = 15;
                    break;
            }

            upgrades.Add(upgrade.upgradeType);
        }

        /// <summary>
        /// Set the wapon for hte chereacter. TYPOS FTW!
        /// </summary>
        /// <param name="newWeapon"></param>
        /// <param name="updateBody">Should we trigger an update for the character?</param>
        public bool SetWeapon(WeaponData newWeapon, bool updateBody = false)
        {
            // no level requirement for now
            //if (newWeapon.levelReq > level)
            //{
            //    return false;
            //}

            weaponData = newWeapon;
            if (updateBody)
            {
                UpdateBody();
            }

            return true;
        }

        public void UpdateBody()
        {
            currentBody.RefreshCharacter();
            //Player.instance.UpdateCharBody(ID);
        }

        // HITCHYAHITCHYAHITCHYA WITDA HARDPUNK TACTIX! KYUH!

        public void AddWeaponUpgrade(WeaponUpgradeData weaponUpgrade)
        {
            if (weaponUpgrade.id == "penetrator")
            {
                hasPenetratorRounds = true;
            }
            else if (weaponUpgrade.id == "stun")
            {
                hasStunRounds = true;
            }
            else if (weaponUpgrade.id == "slam")
            {
                hasSlamRounds = true;
            }
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
                "Cary",
                "Kim",
                "PC",
                "Emily",
                "Jenna",
                "Stephanie",
                "Steven",
                "Jeff",
                "Jimmy",
                "Hannuar",
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
                "Adams",
                "Rogers",
                "Peterson"
            };

            string firstName = firstNameOptions[UnityEngine.Random.Range(0, firstNameOptions.Count)];
            string lastName = lastNameOptions[UnityEngine.Random.Range(0, lastNameOptions.Count)];

            charName = firstName + " " + lastName;
        }
    }
}