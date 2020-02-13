using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonLogic : MonoBehaviour
{

    [SerializeField] BannerBASE banner;

    public void CalculateSummon()
    {
        float rand = Random.Range(0, banner.basicInfo.goldChance + banner.basicInfo.silverChance+ banner.basicInfo.bronzeChance);
        if (rand < banner.basicInfo.goldChance)
            CalculateByRarity(Rarity.GOLD);
        else if (rand < banner.basicInfo.silverChance)
            CalculateByRarity(Rarity.SILVER);
        else if (rand < banner.basicInfo.bronzeChance)
            CalculateByRarity(Rarity.BRONZE);
    }

    CharacterInfo CalculateByRarity(Rarity rarity)
    {
        float aux = 0;
        List<BannerInfo.SpecialChance> list=null;
        switch (rarity)
        {
            case Rarity.GOLD:
                list = banner.basicInfo.GoldPlayers;
                break;
            case Rarity.SILVER:
                list = banner.basicInfo.SilverPlayers;
                break;
            case Rarity.BRONZE:
                list = banner.basicInfo.BronzePlayers;
                break;
             default:
                return null;
        }
        foreach (BannerInfo.SpecialChance character in list)
        {
            aux += character.customChance;
        }
        float rand = Random.Range(0.0f, aux);
        //banner.basicInfo.GoldPlayers.Find(x => rand<x.customChance&& x.customChance == rand);
        float counter = 0;
        foreach (BannerInfo.SpecialChance character in list)
        {
            counter += character.customChance;
            if (rand <= counter)
            {
                return character.character;
            }
        }

        //Error 
        return null;
    }


}
