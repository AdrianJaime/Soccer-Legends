using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipCardFormationLogic : MonoBehaviour
{
    public int identifier = -1;

    [HideInInspector]
    public EquipCardRender render;

    FormationManager manager;

    private void Start()
    {
        manager = FindObjectOfType<FormationManager>();
        render = gameObject.GetComponent<EquipCardRender>();
        render = gameObject.GetComponent<EquipCardRender>();
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
