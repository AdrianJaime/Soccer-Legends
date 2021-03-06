﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationSelectorCard : MonoBehaviour
{
    public CharacterBasic characterInfo;
    private FormationManager manager;
    [SerializeField] Image role;
    [SerializeField] SpriteConpendiumSO roles;
    private void Start()
    {
        characterInfo = gameObject.GetComponent<CharacterBasic>();
        characterInfo.basicInfo = StaticInfo.teamSelectedToPlay[transform.GetSiblingIndex()].basicInfo;
        characterInfo.info = StaticInfo.teamSelectedToPlay[transform.GetSiblingIndex()].info;
        characterInfo.levelMAX = StaticInfo.teamSelectedToPlay[transform.GetSiblingIndex()].levelMAX;
        characterInfo.currentExpAwakening = StaticInfo.teamSelectedToPlay[transform.GetSiblingIndex()].currentExpAwakening;
        characterInfo.currentExp = StaticInfo.teamSelectedToPlay[transform.GetSiblingIndex()].currentExp;
        characterInfo.power = StaticInfo.teamSelectedToPlay[transform.GetSiblingIndex()].power;
        gameObject.GetComponent<Image>().sprite= characterInfo.basicInfo.artworkSelectorIcon;
        manager = FindObjectOfType<FormationManager>();

        //Temporal pone los roles
        role.sprite = roles.sprites[(int)characterInfo.basicInfo.rol];

    }
    public void OnClickSlot()
    {
        manager.EquipCharacter(characterInfo);
    }
}
