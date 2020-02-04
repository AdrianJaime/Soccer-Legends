using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationSelectorCard : MonoBehaviour
{
    public CharacterBasic characterInfo;
    private FormationManager manager;

    private void Start()
    {
        characterInfo = gameObject.GetComponent<CharacterBasic>();
        manager = FindObjectOfType<FormationManager>();

    }
    public void OnClickSlot()
    {
        manager.EquipCharacter(characterInfo);
    }
}
