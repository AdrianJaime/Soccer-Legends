using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipamentManager : MonoBehaviour
{

    public EquipData actualEquip;//array of characters
    public EquipCardLogic [] arraySlots=new EquipCardLogic[8];
    public InventoryManager inventoryManager;

    public int identifierCurrentSlotEquipament = -1;
    public int identifierCurrentSlotInventory = -1;

    public void SetCurrentSlot(int _identifier)
    {
        if(identifierCurrentSlotEquipament == -1)//no hay slot identificado
            identifierCurrentSlotEquipament = _identifier;
        else
        {
            arraySlots[identifierCurrentSlotEquipament].DiselectedRender();
            identifierCurrentSlotEquipament = _identifier;




        }
    }

    public void EquipCharacter(CharacterInfo _character)
    {

        if (identifierCurrentSlotEquipament != -1)//si tenemos un slot seleccionado
        {

            int characterUsed = actualEquip.isUsed(_character);


            if (characterUsed == -1)
            {
                actualEquip.arrayEquiped[identifierCurrentSlotEquipament] = true;
                actualEquip.listOfCharacters[identifierCurrentSlotEquipament] = _character;
                arraySlots[identifierCurrentSlotEquipament].Set(_character);

            }
            else
            {
                actualEquip.arrayEquiped[characterUsed] = false;
                actualEquip.listOfCharacters[characterUsed] = null;
                arraySlots[characterUsed].Set(null);


                actualEquip.arrayEquiped[identifierCurrentSlotEquipament] = true;
                actualEquip.listOfCharacters[identifierCurrentSlotEquipament] = _character;
                arraySlots[identifierCurrentSlotEquipament].Set(_character);
            }
        }


    }

}
