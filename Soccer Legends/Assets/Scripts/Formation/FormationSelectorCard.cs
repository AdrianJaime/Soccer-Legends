using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationSelectorCard : MonoBehaviour
{
    public CharacterBasic characterInfo;
    private FormationManager manager;

    private void Start()
    {
        characterInfo = gameObject.GetComponent<CharacterBasic>();
        gameObject.GetComponent<Image>().sprite= characterInfo.basicInfo.artworkSelectorIcon;
        manager = FindObjectOfType<FormationManager>();

    }
    public void OnClickSlot()
    {
        manager.EquipCharacter(characterInfo);
    }
}
