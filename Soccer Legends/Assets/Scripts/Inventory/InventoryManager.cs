﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    [SerializeField] Transform locationCharacterSpawn;
    [SerializeField] GameObject inventoryPrefabUnit;

    public List<GameObject> listOfSlots = new List<GameObject>();//list of slots with the info of player

    public List<CharacterBasic> listActualCharacters = new List<CharacterBasic>();
    [SerializeField] CharactersCompendium listOfCharacters;

    private void Awake()
    {
        CreateInventory();
    }


    public void CreateInventory()
    {
        int counterIdentifier = 0;

        //por cada jgador en la lista de jugadores (CharacterInfo)
        foreach (CharacterInfo characterUnit in listOfCharacters.compendiumOfCharacters)
        {
            if (characterUnit != null)
            {
                //vamos a crear un objeto "Carta" que contiene la info completa de un cvharacter (characterBasic)
                GameObject actualCard = Instantiate(inventoryPrefabUnit, locationCharacterSpawn);
                //Le asignamos una informacion basica de jugador
                actualCard.GetComponent<CharacterBasic>().basicInfo = listOfCharacters.compendiumOfCharacters[counterIdentifier];
                //con la infromación basica cargamos los datos de BD con el ID del personaje.
                actualCard.GetComponent<CharacterBasic>().LoadCharacterStats(actualCard.GetComponent<CharacterBasic>().basicInfo.ID);


                listActualCharacters.Add(actualCard.GetComponent<CharacterBasic>());//GUARDAMOS ESTO 
                listOfSlots.Add(actualCard);
            }
            counterIdentifier++;
        }
    }

    public void SortByAtribute(string _aux)
    {
        listActualCharacters.Sort(new SortCharacter("index"));
        listActualCharacters.Sort(new SortCharacter(_aux));
        listActualCharacters.Reverse(); //sino los oredna de menos a mas

        int counterIdentifier = 0;

        foreach (GameObject slot in listOfSlots)
        {

            slot.GetComponent<InventoryCardRender>().characterInfo = listActualCharacters[counterIdentifier];
            slot.GetComponent<InventoryCardRender>().UpdateSlotRender();
            counterIdentifier++;

        }

    }

    public CharacterBasic FindCharacterByID(string dbID)
    {
        Debug.Log("ENTRO EN LA FUNCION DE BUSCAR");
        return listActualCharacters.Find(x => x.basicInfo.ID == dbID);
    }

    public class SortCharacter : IComparer<CharacterBasic>
    {
        public string characterAtributeToSort;

        int IComparer<CharacterBasic>.Compare(CharacterBasic _objA, CharacterBasic _objB)
        {
            switch (characterAtributeToSort)
            {

                case "level":
                    {
                        Debug.Log("level");

                        int t1 = _objA.info.level;
                        int t2 = _objB.info.level;
                        return t1.CompareTo(t2);
                    }
                case "rarity":
                    {
                        int t1 = (int)_objA.basicInfo.rarity;
                        int t2 = (int)_objB.basicInfo.rarity;
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
                        int t1 = _objA.info.atk;
                        int t2 = _objB.info.atk;
                        return t1.CompareTo(t2);
                    }
                case "stats-defense":
                    {
                        int t1 = _objA.info.def;
                        int t2 = _objB.info.def;
                        return t1.CompareTo(t2);
                    }
                case "stats-technique":
                    {
                        int t1 = _objA.info.teq;
                        int t2 = _objB.info.teq;
                        return t1.CompareTo(t2);
                    }
                case "type":
                    {

                        int t1 = (int)_objA.basicInfo.type;
                        int t2 = (int)_objB.basicInfo.type;
                        return t1.CompareTo(t2);
                    }
                case "index":
                    {
                        int t1 = _objA.basicInfo.index;
                        int t2 = _objB.basicInfo.index;
                        return -(t1.CompareTo(t2));//cambio el signo al resultado para hacer un sort de menos a mas en este orden
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
