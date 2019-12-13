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
    public int level;
    public int power;
    public bool owned;

    public void SaveCharacter()
    {
        Serializer.Save(basicInfo.ID, this);
    }
    public void LoadCharacterStats()
    {
        CharacterBasic aux = Serializer.Load<CharacterBasic>(basicInfo.ID);

        if (aux != null)
        {
            stats = aux.stats;
            level = aux.level;
            power = aux.power;
            owned = aux.owned;
        }
    }
}
