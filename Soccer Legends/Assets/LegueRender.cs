using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegueRender : MonoBehaviour
{
    public EquipObject actualEquip;
    public EquipObject[] equips;//array of characters


    public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap SimpleScrollSnap;



    private void Start()
    {
        //ASIGNAMOS EL PRIMER EQUIPO
        actualEquip = equips[0];
        //LEEMOS DE BD
        //actualEquip.LoadEquipBD(inventoryManager);
        //CARGAMOS EN LOS SLOTS
        LoadEquipInSlots();
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
