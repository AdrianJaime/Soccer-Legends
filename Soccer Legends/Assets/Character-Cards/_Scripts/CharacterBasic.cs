using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterBasic : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
        public int shot;
        public int technique;
        public int defense;
    }

    public CharacterInfo basicInfo;

    //info user
    public Stats stats;
    public int level=1;
    public int power=1;
    public bool owned=true;//no se inicializa a estos valores

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
            level = 10;
            power = 10;
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

