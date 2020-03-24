using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum Rarity {BRONZE,SILVER,GOLD }
public enum Type {RED,GREEN,YELLOW,PURPLE,BLUE }
public enum Rol { PIVOT, TECHNICAL, DEFENSIVE }


[System.Serializable]
[CreateAssetMenu(fileName ="New_CharacterInfo", menuName ="Cards/NewInfo")]
public class CharacterInfo : ScriptableObject
{
    //info player
    public string ID;
    public int index;
    public string nameCharacter;
    public string description;

    public Rarity rarity;
    public Type type;

    //info drawable
    public Sprite artworkIcon,completeArtwork,artworkConforntation;

    //info special attack
    public SpecialAttackInfo specialAttackInfo;

    public RuntimeAnimatorController animator_character;

}
