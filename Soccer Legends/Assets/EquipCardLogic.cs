using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipCardLogic : MonoBehaviour
{
    public int identifier = -1;
    public EquipCardRender render;
    EquipamentManager manager;

    private void Start()
    {
        manager = FindObjectOfType<EquipamentManager>();
    }

    public void OnClickSlot()
    {
        manager.SetCurrentSlot(identifier);
        SelectedRender();
    }

    public void SelectedRender()
    {
        render.Slected();
    }
    public void DiselectedRender()
    {
        render.Diselected();
    }

    public void Set(CharacterBasic _character)
    {
        render.characterInfo = _character;
        render.UpdateRender();
    }

}
