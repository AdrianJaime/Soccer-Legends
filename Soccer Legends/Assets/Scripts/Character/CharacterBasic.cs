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

        string ecryptedKey = EncriptScript.Encrypt(basicInfo.ID);
        Debug.Log(ecryptedKey);
        StoredCharacterData dataCharacter = new StoredCharacterData(stats, level, power, true);
        Serializer.Save(ecryptedKey, dataCharacter);


    }
    public bool LoadCharacterStats()
    {

        string encryptedKey = EncriptScript.Encrypt(basicInfo.ID);
        Debug.Log(encryptedKey);
        StoredCharacterData aux = Serializer.Load<StoredCharacterData>(encryptedKey);

        if (aux != null)
        {
            stats = aux.stats;
            level = aux.level;
            power = aux.power;
            owned = aux.owned;
            return true;
        }
        
        return false;
    }
    
    [System.Serializable]
    public class StoredCharacterData
    {
        //info user
        public Stats stats;
        public int level;
        public int power;
        public bool owned;

        public StoredCharacterData(Stats _stats, int _level, int _power, bool _owned)
        {
            stats = _stats;
            level = _level;
            power = _power;
            owned = _owned;
        }
    }
}

