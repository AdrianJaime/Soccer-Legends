using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public Transform locationCharacterSpawn;
    public GameObject inventoryPrefabUnit;

    public List<GameObject> listOfSlots = new List<GameObject>();//list of slots with the info of player

    public List<CharacterInfo> listOfCharacters = new List<CharacterInfo>();


    private void Start()
    {
        CreateInventory();
    }


    public void CreateInventory()
    {
        int counterIdentifier = 0;

        //por cada jgador en la lista de jugadores (CharacterInfo)
        foreach (CharacterInfo characterUnit in listOfCharacters)
        {
            if (characterUnit != null)
            {
                //vamos a crear un objeto "Carta" que contiene la info completa de un cvharacter (characterBasic)
                GameObject actualCard = Instantiate(inventoryPrefabUnit, locationCharacterSpawn);
                //Le asignamos una informacion basica de jugador
                actualCard.GetComponent<CharacterBasic>().basicInfo = listOfCharacters[counterIdentifier];
                //con la infromación basica cargamos los datos con el ID del personaje.
                if (!actualCard.GetComponent<CharacterBasic>().LoadCharacterStats())
                {
                    actualCard.GetComponent<CharacterBasic>().SaveCharacter();
                    actualCard.GetComponent<CharacterBasic>().LoadCharacterStats();

                }

                listOfSlots.Add(actualCard);
            }
            counterIdentifier++;
        }
    }




    public void SortByAtribute(string _aux)
    {
        listOfSlots.Sort(new SortCharacter(_aux));
        listOfSlots.Reverse();

    }

    public class SortCharacter : IComparer<GameObject>
    {
        public string characterAtributeToSort;

        int IComparer<GameObject>.Compare(GameObject _objA, GameObject _objB)
        {
            switch (characterAtributeToSort)
            {

                case "level":
                    {
                        Debug.Log("level");

                        int t1 = _objA.GetComponent<CharacterBasic>().level;
                        int t2 = _objB.GetComponent<CharacterBasic>().level;
                        return t1.CompareTo(t2);
                    }
                case "rarity":
                    {
                        int t1 = (int)_objA.GetComponent<CharacterBasic>().basicInfo.rarity;
                        int t2 = (int)_objB.GetComponent<CharacterBasic>().basicInfo.rarity;
                        return t1.CompareTo(t2);
                    }
                case "power":
                    {
                        int t1 = _objA.GetComponent<CharacterBasic>().power;
                        int t2 = _objB.GetComponent<CharacterBasic>().power;
                        return t1.CompareTo(t2);
                    }
                case "stats-shot":
                    {
                        int t1 = _objA.GetComponent<CharacterBasic>().stats.shot;
                        int t2 = _objB.GetComponent<CharacterBasic>().stats.shot;
                        return t1.CompareTo(t2);
                    }
                case "stats-defense":
                    {
                        int t1 = _objA.GetComponent<CharacterBasic>().stats.defense;
                        int t2 = _objB.GetComponent<CharacterBasic>().stats.defense;
                        return t1.CompareTo(t2);
                    }
                case "stats-technique":
                    {
                        int t1 = _objA.GetComponent<CharacterBasic>().stats.technique;
                        int t2 = _objB.GetComponent<CharacterBasic>().stats.technique;
                        return t1.CompareTo(t2);
                    }
                case "type":
                    {

                        int t1 = (int)_objA.GetComponent<CharacterBasic>().basicInfo.type;
                        int t2 = (int)_objB.GetComponent<CharacterBasic>().basicInfo.type;
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
