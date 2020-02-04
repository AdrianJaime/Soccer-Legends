using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    public Formation actualForm;

    public int identifierCurrentSlotEquipament = -1;


    private void Start()
    {
        //CARGAMOS EN LOS SLOTS
        LoadEquipInSlots();
    }


    public void SetCurrentSlot(int _identifier)
    {
        if (identifierCurrentSlotEquipament == -1)//no hay slot identificado
            identifierCurrentSlotEquipament = _identifier;
        else
        {
            actualForm.arraySlots[identifierCurrentSlotEquipament].DiselectedRender();
            identifierCurrentSlotEquipament = _identifier;
        }
    }

    public void EquipCharacter(CharacterBasic _character)
    {

        if (identifierCurrentSlotEquipament != -1)//si tenemos un slot seleccionado
        {

            int characterUsed = actualForm.isUsed(_character);


            if (characterUsed == -1)
            {
                actualForm.arrayEquiped[identifierCurrentSlotEquipament] = true;
                actualForm.listOfCharacters[identifierCurrentSlotEquipament] = _character;
                actualForm.arraySlots[identifierCurrentSlotEquipament].Set(_character);


            }
            else
            {
                actualForm.arrayEquiped[characterUsed] = false;
                actualForm.listOfCharacters[characterUsed] = null;
                actualForm.arraySlots[characterUsed].Set(null);


                actualForm.arrayEquiped[identifierCurrentSlotEquipament] = true;
                actualForm.listOfCharacters[identifierCurrentSlotEquipament] = _character;
                actualForm.arraySlots[identifierCurrentSlotEquipament].Set(_character);
            }
            identifierCurrentSlotEquipament = -1;
        }


    }

    public void DisEquipCharacter(CharacterBasic _character)
    {
        int characterUsed = actualForm.isUsed(_character);


        if (characterUsed != -1)//usado
        {
            actualForm.arrayEquiped[characterUsed] = false;
            actualForm.listOfCharacters[characterUsed] = null;
            actualForm.arraySlots[characterUsed].Set(null);

            identifierCurrentSlotEquipament = -1;

        }
    }


    public void LoadEquipInSlots()
    {
        //esta funcion carga cada InfoPersonaje en cada slot
        int aux = 0;
        foreach (EquipCardFormationLogic card in actualForm.arraySlots)
        {
            card.Set(actualForm.listOfCharacters[aux]);
            aux++;
        }
    }
}
