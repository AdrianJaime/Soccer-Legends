using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EquipObject : MonoBehaviour
{
    public int identifierEquip;

    public EquipCardLogic[] arraySlots = new EquipCardLogic[8];
    public CharacterBasic[] listOfCharacters = new CharacterBasic[8];

    public bool[] arrayEquiped = new bool[8];


    public string[] arrayID = new string[8];//DESCARGAMOS UNA LISTA DE INTS con la info del ID de cada personaje equipado en el team desde BD A traves del identifierEquip

    // public bool preferent;
    public int isUsed(CharacterBasic _aux)
    {
        for (int i = 0; i < 8; i++)
        {
            if (listOfCharacters[i] != null)
            {
                if (listOfCharacters[i].basicInfo.ID == _aux.basicInfo.ID)
                    if (arrayEquiped[i] == true)
                        return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Esta funcion se tiene que implementar con BD y gestionarse sola como los characteers
    /// </summary>
    /// <param name="inventory"></param>
    public void LoadEquipBD(InventoryManager inventory)
    {
        int aux = 0;
        foreach (string value in arrayID)
        {
            if (value != null)
            {
                Debug.Log("BUSCO UN ID EN EL INVENTARIO");
                CharacterBasic auxCharacter = inventory.FindCharacterByID(value);
                if (auxCharacter != null)
                {
                    Debug.Log("LO ENCONTRÉ");
                    listOfCharacters[aux] = auxCharacter;
                    arrayEquiped[aux] = true;


                }
                else
                    Debug.Log("NO LO ENCONTRÉ DEJA DE PIRATEAR Y PONER IDs ERRONEOS");

                aux++;
            }
        }
    }



}
