using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipamentManager : MonoBehaviour
{

    public EquipData actualEquip;//array of characters
    public EquipData []equips;//array of characters
    public EquipCardLogic [] arraySlots=new EquipCardLogic[8];
    public InventoryManager inventoryManager;

    int identifierEquip = 0;
    public int identifierCurrentSlotEquipament = -1;
    public int identifierCurrentSlotInventory = -1;


    private void Start()
    {
        LoadEquipInSlots();
    }


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

    public void DisEquipCharacter(CharacterInfo _character)
    {
        int characterUsed = actualEquip.isUsed(_character);


        if (characterUsed != -1)//usado
        {
            actualEquip.arrayEquiped[characterUsed] = false;
            actualEquip.listOfCharacters[characterUsed] = null;
            arraySlots[characterUsed].Set(null);

            identifierCurrentSlotInventory = -1;
            identifierCurrentSlotEquipament = -1;

        }
    }

    public void ChangeEquip(bool _rightSide)
    {
        if (_rightSide)
        {
            identifierEquip++;
            if (identifierEquip > equips.Length-1)
            {
                identifierEquip = 0;
            }
            actualEquip = equips[identifierEquip];
        }
        else
        {
            identifierEquip--;
            if (identifierEquip < 0)
            {
                identifierEquip = equips.Length-1;
            }
            actualEquip = equips[identifierEquip];
        }

        LoadEquipInSlots();

    }

    public void LoadEquipInSlots()
    {
        int aux = 0;
        foreach (EquipCardLogic card in arraySlots)
        {
            card.render.characterInfo = equips[identifierEquip].listOfCharacters[aux];
            card.render.UpdateRender();
            aux++;
        }
    }

}
