using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="New_BannerInfo", menuName ="Summons/NewBanner")]
public class BannerInfo : ScriptableObject
{
    [System.Serializable]
    public struct SpecialChance
    {
        public CharacterInfo character;
        public int customChance;
    }

    public string ID;
    public Sprite spriteSmallBanner,spriteBigBanner;
    //loquesea
    public List<SpecialChance> GoldPlayers,SilverPlayers,BronzePlayers;

    public int goldChance = 5;
    public int silverChance = 25;
    public int bronzeChance = 70;



}
