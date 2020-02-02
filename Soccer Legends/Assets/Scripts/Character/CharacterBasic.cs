using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterBasic : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
        public int atq;
        public int teq;
        public int def;
    }

    public CharacterInfo basicInfo;

    //info user
    public Stats stats;
    public int level=1;
    public int levelMAX=1;
    public int currentExpAwakening = 0;
    public int currentExp=0;
    public int power=1;
    public bool owned=true;//no se inicializa a estos valores

    public CharacterBasic()
    {

    }
    public CharacterBasic(CharacterBasic _copy)
    {
        stats = _copy.stats;
        level = _copy.level;
        currentExp = _copy.currentExp;
        power = _copy.power;
        owned = _copy.owned;
        basicInfo = _copy.basicInfo;
        levelMAX = _copy.levelMAX;
    }

    public void SaveCharacter()
    {
        //Guarda el pj en base de datos


    }
    public void LoadCharacterStats()
    {
        //Recupera la info de base de datos a traves del basicInfo->ID

        stats = new Stats();//POR DEFECTO LE PONGO VALORES
        level = 1;
        power =1;
        owned = true;

    }
    

}

