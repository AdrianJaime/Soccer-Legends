using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipamentManager : MonoBehaviour
{
    public EquipObject actualEquip;
    public EquipObject []equips;//array of characters

    public InventoryManager inventoryManager;

    int identifierEquip = 0;
    public int identifierCurrentSlotEquipament = -1;
    public int identifierCurrentSlotInventory = -1;


    private void Start()
    {
        actualEquip = equips[0];
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

    public void EquipCharacter(CharacterInfo _character)
    {

        if (identifierCurrentSlotEquipament != -1)//si tenemos un slot seleccionado
        {

            int characterUsed = actualEquip.equipData.isUsed(_character);


            if (characterUsed == -1)
            {
                actualEquip.equipData.arrayEquiped[identifierCurrentSlotEquipament] = true;
                actualEquip.equipData.listOfCharacters[identifierCurrentSlotEquipament] = _character;
                actualEquip.arraySlots[identifierCurrentSlotEquipament].Set(_character);

            }
            else
            {
                actualEquip.equipData.arrayEquiped[characterUsed] = false;
                actualEquip.equipData.listOfCharacters[characterUsed] = null;
                actualEquip.arraySlots[characterUsed].Set(null);


                actualEquip.equipData.arrayEquiped[identifierCurrentSlotEquipament] = true;
                actualEquip.equipData.listOfCharacters[identifierCurrentSlotEquipament] = _character;
                actualEquip.arraySlots[identifierCurrentSlotEquipament].Set(_character);
            }
        }


    }

    public void DisEquipCharacter(CharacterInfo _character)
    {
        int characterUsed = actualEquip.equipData.isUsed(_character);


        if (characterUsed != -1)//usado
        {
            actualEquip.equipData.arrayEquiped[characterUsed] = false;
            actualEquip.equipData.listOfCharacters[characterUsed] = null;
            actualEquip.arraySlots[characterUsed].Set(null);

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
        foreach (EquipCardLogic card in actualEquip.arraySlots)
        {
            card.render.characterInfo = equips[identifierEquip].equipData.listOfCharacters[aux];
            card.render.UpdateRender();
            aux++;
        }
    }

}
