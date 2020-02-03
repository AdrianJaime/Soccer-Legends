using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector_EnemyRepresentation : MonoBehaviour
{

    public List<CharacterBasic> listActualCharacters = new List<CharacterBasic>();

    /// <summary> Encapsular esto tambein para el inventario script comun
    public List<CharacterInfo> listOfCharacters = new List<CharacterInfo>();//hay que sustituir esto desde el editor por leerlo desde la carpeta
    public CharacterInfo FindCharacterByID(string dbID)
    {
        Debug.Log("ENTRO EN LA FUNCION DE BUSCAR");
        return listOfCharacters.Find(x => x.ID == dbID);
    }/// </summary>

    public string[] arrayID = new string[8];//DESCARGAMOS UNA LISTA DE INTS con la info del ID de cada personaje equipado en el team desde BD A traves del identifierEquip

    /// <summary>
    /// Esta clase tiene que ser de tipo CharacterBasicDB
    /// </summary>
    public CharacterBasic[] arrayCharactersBase ;//DESCARGAMOS UNA LISTA DE INTS con la info del ID de cada personaje equipado en el team desde BD A traves del identifierEquip



    // Start is called before the first frame update
    void Awake()
    {
        LoadEquipBD();
    }
    public void LoadEquipBD()
    {
        int aux = 0;
        foreach (string value in arrayID)
        {
            if (value != null)
            {
                Debug.Log("BUSCO UN ID EN EL INVENTARIO");
                CharacterInfo auxCharacter = FindCharacterByID(value);
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
