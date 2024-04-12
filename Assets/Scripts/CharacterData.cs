using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "MyScriptables/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;

    [Header("Weapon")]
    public string weaponName;
    public float attackInterval;
    public int damage;
    public bool useProjPhys;
    public int projectileVelocity;
    public int projPerShot;
    public float projSpreadAngle;
    public Sprite projSprite;
}
