using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum Rarity { BRONZE, SILVER, GOLD }
public enum Type {RED,GREEN,YELLOW,PURPLE,BLUE }
public enum Rol { PIVOT, WINGER, LAST_MAN, GOALKEEPER } //PIVOT, ALA, CIERRE, PORTERO
public enum School { DEMONS, EGYPT, MUSIC, FLAVOR, ZODIAC}


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
    public Rol rol;
    public School school;

    //info drawable
    public GameObject animation2DObject;

    public Sprite artworkIcon, 
                  completeArtwork,
                  artworkConforntation,
                  artworkSelectorIcon,
                  artworkResult,
                  artworkPointsGameplay;

    //info special attack
    public SpecialAttackInfo specialAttackInfo;

    public RuntimeAnimatorController animator_character;
}
