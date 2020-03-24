﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLevelUp : MonoBehaviour
{

    [SerializeField] Image artworkLocation;
    [SerializeField] Text oldAtkTextLocation, oldTeqTextLocation, oldDefTextLocation;
    [SerializeField] Text newAtkTextLocation, newTeqTextLocation, newDefTextLocation;
    [SerializeField] Text level, characterName;

    CharacterBasic character;
    CharacterBasic newCharacter;
    private void Start()
    {
        character = new CharacterBasic(StaticInfo.characterToAcces);
        newCharacter = new CharacterBasic(character);
        //get character from database with the id that hsa shared static info en vez de charazterbasic variable
        artworkLocation.sprite = character.basicInfo.completeArtwork;
        characterName.text = character.basicInfo.nameCharacter;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (character.info.level != newCharacter.info.level)
        {
            newAtkTextLocation.text = "ATK: " + newCharacter.info.atk.ToString();
            newTeqTextLocation.text = "TEQ: " + newCharacter.info.teq.ToString();
            newDefTextLocation.text = "DEF: " + newCharacter.info.def.ToString();

            level.text = newCharacter.info.level + "/" + newCharacter.levelMAX;//sustituir este 100 en funcion de su maximo alcanzado si awaken o no
        }
        else
        {
            oldAtkTextLocation.text = "ATK: " + character.info.atk.ToString();
            oldTeqTextLocation.text = "TEQ: " + character.info.teq.ToString();
            oldDefTextLocation.text = "DEF: " + character.info.def.ToString();
            newAtkTextLocation.text = "ATK: -";
            newTeqTextLocation.text = "TEQ: -";
            newDefTextLocation.text = "DEF: -";
            level.text = + character.info.level + "/"+newCharacter.levelMAX;
        }
    }

    void LevelUp()
    {
        newCharacter.info.level++;
        newCharacter.info.atk++;
        newCharacter.info.teq++;
        newCharacter.info.def++;
    }
    void LevelDown()
    {
        newCharacter.info.level--;
        newCharacter.info.atk--;
        newCharacter.info.teq--;
        newCharacter.info.def--;
    }


    public void AddExp(int _exp)
    {
        if(newCharacter.currentExp+_exp> (newCharacter.currentExp + 2))
        {
            LevelUp();
            UpdateUI();
        }
    }
    public void SubstractExp(int _exp)
    {
        if (newCharacter.currentExp - _exp < (newCharacter.currentExp - 2))
        {
            LevelDown();
            UpdateUI();
        }
    }


    public void ConfirmSelection()
    {
        character = new CharacterBasic(newCharacter);
        UpdateUI();

    }
    public void Reset()
    {
        newCharacter = new CharacterBasic(character);
        UpdateUI();

    }


}
