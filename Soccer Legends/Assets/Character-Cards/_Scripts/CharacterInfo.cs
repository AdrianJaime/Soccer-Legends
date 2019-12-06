using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Stats
{
    public int shot;
    public int technique;
    public int defense;
}

public enum Rarity {BRONZE,SILVER,GOLD }
public enum Type {RED,GREEN,YELLOW,PURPLE,BLUE }
public enum Rol { defense, TECHNICAL, DEFENSIVE }


[CreateAssetMenu(fileName ="New_CharacterInfo", menuName ="Cards/NewInfo")]
public class CharacterInfo : ScriptableObject
{
    //info player
    public string ID;
    public string nameCharacter;
    public string description;
    public int level;
    public Rarity rarity;
    public Type type;

    //info drawable
    public Sprite artwork;

    //info stats
    public Stats stats;

    //info special attack
    public SpecialAttackInfo specialAttackInfo;

}
