using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector_Representation : MonoBehaviour
{

    public List<CharacterBasic> listActualCharacters = new List<CharacterBasic>();

    /// <summary> Encapsular esto tambein para el inventario script comun
    [SerializeField]
    InventoryManager inventory;

    public string[] arrayID = new string[8];//DESCARGAMOS UNA LISTA DE INTS con la info del ID de cada personaje equipado en el team desde BD A traves del identifierEquip

    /// <summary>
    /// Esta clase tiene que ser de tipo CharacterBasicDB
    /// </summary>
    public CharacterBasic[] arrayCharactersBase ;//DESCARGAMOS UNA LISTA DE INTS con la info del ID de cada personaje equipado en el team desde BD A traves del identifierEquip

    // Start is called before the first frame update
    void Awake()
    {
        StaticInfo.rivalTeam = new List<CharacterBasic>();
        LoadEquipBD(inventory);
    }

    public void LoadEquipBD(InventoryManager _inventory)
    {
        int aux = 0;
        for (int i = 0; i < 8; i++)
        {
            arrayID[i] = PlayerPrefs.GetString("team" + "1" + "slot" + i);
            Debug.Log("In " + "team" + "1" + "slot" + i + " There is: " + PlayerPrefs.GetString("team" + "1" + "slot" + i));
        }
        foreach (string value in arrayID)
        {
            if (value != null)
            {
                Debug.Log("BUSCO UN ID EN EL INVENTARIO");
                CharacterInfo auxCharacter = _inventory.FindCharacterByID(value).basicInfo;
                if (auxCharacter != null)
                {
                    Debug.Log("LO ENCONTRÉ");
                    listActualCharacters[aux].basicInfo=auxCharacter;

                }
                else
                    Debug.Log("NO LO ENCONTRÉ DEJA DE PIRATEAR Y PONER IDs ERRONEOS");

            }
            aux++;

        }
    }

}
