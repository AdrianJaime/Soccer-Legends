using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;

[System.Serializable]
public class CharacterBasic : MonoBehaviour
{
    [System.Serializable]
    public struct data
    {
        public int atk;
        public int teq;
        public int def;
        public int level;
        public bool owned;

        public data(int _atq, int _teq, int _def, int _level = 1, bool _owned = false)
        {
            atk = _atq;
            teq = _teq;
            def = _def;
            level = _level;
            owned = _owned;
        }
    }

    public CharacterInfo basicInfo;

    //info user
    public data info;
    public int levelMAX=1;
    public int currentExpAwakening = 0;
    public int currentExp=0;
    public int power=1;

    public CharacterBasic()
    {

    }

    public CharacterBasic(CharacterInfo _basicInfo, data _info, int _levelMAX = 1, int _currentExpAwakening = 0, 
        int _currentExp = 0, int _power = 1)
    {
        basicInfo = _basicInfo;
        info = _info;
        levelMAX = _levelMAX;
        currentExpAwakening = _currentExpAwakening;
        currentExp = _currentExp;
        power = _power;
    }
    public CharacterBasic(CharacterBasic _copy)
    {
        info = _copy.info;
        currentExp = _copy.currentExp;
        power = _copy.power;
       // owned = _copy.owned;
        basicInfo = _copy.basicInfo;
        levelMAX = _copy.levelMAX;
    }

    public void SaveCharacter()
    {
        //Guarda el pj en base de datos


    }
    public void LoadCharacterStats(string id)
    {
        //Recupera la info de base de datos a traves del basicInfo->ID
        Debug.Log("Tus muertos 23");

        RestClient.Get<data>("https://soccer-legends-db.firebaseio.com/player/0/characters/"+id+".json").Then(response =>
        {
            info = response;
        });

        ///ALFA CHANGE
        info.owned = true;


    }
    

}

