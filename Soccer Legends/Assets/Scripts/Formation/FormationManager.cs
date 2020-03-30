using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    public Formation actualForm;

    public int identifierCurrentSlotEquipament = -1;

    /// <summary>
    /// Actualiza el indice del slot de la formación seleccionado para luego equipar un personaje
    /// </summary>
    /// <param name="_identifier"></param>
    public void SetCurrentSlot(int _identifier)
    {
        if (identifierCurrentSlotEquipament == -1)//no hay slot identificado
            identifierCurrentSlotEquipament = _identifier;
        else
        {
            actualForm.slotsFormation[identifierCurrentSlotEquipament].DiselectedRender();
            identifierCurrentSlotEquipament = _identifier;
        }
    }


    /// <summary>
    /// Comprueba donde debe ir el personaje dependiendo de si ha sido seleccionado previamernte o no
    /// </summary>
    /// <param name="_character"></param>
    public void EquipCharacter(CharacterBasic _character)
    {

        if (identifierCurrentSlotEquipament != -1)//si tenemos un slot seleccionado
        {

            int characterUsed = actualForm.isUsed(_character);


            if (characterUsed == -1)
            {
                actualForm.arrayEquiped[identifierCurrentSlotEquipament] = true;
                actualForm.listOfCharactersInFormation[identifierCurrentSlotEquipament] = _character;
                actualForm.slotsFormation[identifierCurrentSlotEquipament].Set(_character);


            }
            else
            {
                actualForm.arrayEquiped[characterUsed] = false;
                actualForm.listOfCharactersInFormation[characterUsed] = null;
                actualForm.slotsFormation[characterUsed].Set(null);


                actualForm.arrayEquiped[identifierCurrentSlotEquipament] = true;
                actualForm.listOfCharactersInFormation[identifierCurrentSlotEquipament] = _character;
                actualForm.slotsFormation[identifierCurrentSlotEquipament].Set(_character);
            }
            identifierCurrentSlotEquipament = -1;

            actualForm.CheckButtonInteractable();
        }


    }

    public void DisEquipCharacter(CharacterBasic _character)
    {
        int characterUsed = actualForm.isUsed(_character);


        if (characterUsed != -1)//usado
        {
            actualForm.arrayEquiped[characterUsed] = false;
            actualForm.listOfCharactersInFormation[characterUsed] = null;
            actualForm.slotsFormation[characterUsed].Set(null);

            identifierCurrentSlotEquipament = -1;

        }
    }



}
