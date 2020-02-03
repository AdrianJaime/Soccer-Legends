using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipamentManager : MonoBehaviour
{
    public EquipObject actualEquip;
    public EquipObject []equips;//array of characters

    public InventoryManager inventoryManager;

    public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap SimpleScrollSnap;

    public int identifierCurrentSlotEquipament = -1;


    private void Start()
    {
        //ASIGNAMOS EL PRIMER EQUIPO
        actualEquip = equips[0];
        //LEEMOS DE BD
        actualEquip.LoadEquipBD(inventoryManager);
        //CARGAMOS EN LOS SLOTS
        LoadEquipInSlots();
    }


    public void SetCurrentSlot(int _identifier)
    {
        if(identifierCurrentSlotEquipament == -1)//no hay slot identificado
            identifierCurrentSlotEquipament = _identifier;
        else
        {
            actualEquip.arraySlots[identifierCurrentSlotEquipament].DiselectedRender();
            identifierCurrentSlotEquipament = _identifier;
        }
    }

    public void EquipCharacter(CharacterBasic _character)
    {

        if (identifierCurrentSlotEquipament != -1)//si tenemos un slot seleccionado
        {

            int characterUsed = actualEquip.isUsed(_character);


            if (characterUsed == -1)
            {
                actualEquip.arrayEquiped[identifierCurrentSlotEquipament] = true;
                actualEquip.listOfCharacters[identifierCurrentSlotEquipament] = _character;
                actualEquip.arraySlots[identifierCurrentSlotEquipament].Set(_character);


            }
            else
            {
                actualEquip.arrayEquiped[characterUsed] = false;
                actualEquip.listOfCharacters[characterUsed] = null;
                actualEquip.arraySlots[characterUsed].Set(null);


                actualEquip.arrayEquiped[identifierCurrentSlotEquipament] = true;
                actualEquip.listOfCharacters[identifierCurrentSlotEquipament] = _character;
                actualEquip.arraySlots[identifierCurrentSlotEquipament].Set(_character);
            }
            identifierCurrentSlotEquipament = -1;
        }


    }

    public void DisEquipCharacter(CharacterBasic _character)
    {
        int characterUsed = actualEquip.isUsed(_character);


        if (characterUsed != -1)//usado
        {
            actualEquip.arrayEquiped[characterUsed] = false;
            actualEquip.listOfCharacters[characterUsed] = null;
            actualEquip.arraySlots[characterUsed].Set(null);

            identifierCurrentSlotEquipament = -1;

        }
    }

    public void ChangeEquipSlide()
    {

        actualEquip = equips[SimpleScrollSnap.TargetPanel];
        LoadEquipInSlots();

    }

    
    public void LoadEquipInSlots()
    {
        //esta funcion carga cada InfoPersonaje en cada slot
        int aux = 0;
        foreach (EquipCardLogic card in actualEquip.arraySlots)
        {
            card.Set(actualEquip.listOfCharacters[aux]);
            aux++;
        }
    }

}
