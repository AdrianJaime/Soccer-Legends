using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_InventoryInfo", menuName = "Equip/NewInventory")]
public class InventoryData :ScriptableObject
{

    public List<CharacterInfo> listOfCharacters=new List<CharacterInfo>();


    public void SortByAtribute(string _aux)
    {
        listOfCharacters.Sort(new SortCharacter(_aux));
        listOfCharacters.Reverse();

    }

    public class SortCharacter : IComparer<CharacterInfo>
    {
        public string characterAtributeToSort;

        int IComparer<CharacterInfo>.Compare(CharacterInfo _objA, CharacterInfo _objB)
        {
            switch (characterAtributeToSort)
            {

                case "level":
                    {
                        Debug.Log("level");

                        int t1 = _objA.level;
                        int t2 = _objB.level;
                        return t1.CompareTo(t2);
                    }
                case "rarity":
                    {
                        int t1 = (int)_objA.rarity;
                        int t2 = (int)_objB.rarity;
                        return t1.CompareTo(t2);
                    }
                case "power":
                    {
                        int t1 = _objA.power;
                        int t2 = _objB.power;
                        return t1.CompareTo(t2);
                    }
                case "stats-shot":
                    {
                        int t1 = _objA.stats.shot;
                        int t2 = _objB.stats.shot;
                        return t1.CompareTo(t2);
                    }
                case "stats-defense":
                    {
                        int t1 = _objA.stats.defense;
                        int t2 = _objB.stats.defense;
                        return t1.CompareTo(t2);
                    }
                case "stats-technique":
                    {
                        int t1 = _objA.stats.technique;
                        int t2 = _objB.stats.technique;
                        return t1.CompareTo(t2);
                    }
                case "type":
                    {

                        int t1 = (int)_objA.type;
                        int t2 = (int)_objB.type;
                        return t1.CompareTo(t2);
                    }
                default:
                    Debug.Log("level");
                    return 0;


            }

        }
        public SortCharacter(string _characterAtributeToSort)
        {
            characterAtributeToSort = _characterAtributeToSort;
        }
    }


}
