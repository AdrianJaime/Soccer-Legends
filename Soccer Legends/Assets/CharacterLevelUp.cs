using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLevelUp : MonoBehaviour
{

    public Image artworkLocation;
    public Text oldAtkTextLocation, oldTeqTextLocation, oldDefTextLocation;
    public Text newAtkTextLocation, newTeqTextLocation, newDefTextLocation;
    public Text level;

    CharacterBasic character;
    CharacterBasic newCharacter;
    private void Start()
    {
        character = new CharacterBasic(StaticInfo.characterToAcces);
        newCharacter = new CharacterBasic(character);
        //get character from database with the id that hsa shared static info en vez de charazterbasic variable
        artworkLocation.sprite = character.basicInfo.artwork;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (character.level != newCharacter.level)
        {
            newAtkTextLocation.text = "ATK: " + newCharacter.stats.atq.ToString();
            newTeqTextLocation.text = "TEQ: " + newCharacter.stats.teq.ToString();
            newDefTextLocation.text = "DEF: " + newCharacter.stats.def.ToString();

            level.text = "Nv." + newCharacter.level + "/100";//sustituir este 100 en funcion de su maximo alcanzado si awaken o no
        }
        else
        {
            oldAtkTextLocation.text = "ATK: " + character.stats.atq.ToString();
            oldTeqTextLocation.text = "TEQ: " + character.stats.teq.ToString();
            oldDefTextLocation.text = "DEF: " + character.stats.def.ToString();
            newAtkTextLocation.text = "ATK: -";
            newTeqTextLocation.text = "TEQ: -";
            newDefTextLocation.text = "DEF: -";
            level.text = "Nv." + character.level + "/100";
        }
    }

    void LevelUp()
    {
        newCharacter.level++;
        newCharacter.stats.atq++;
        newCharacter.stats.teq++;
        newCharacter.stats.def++;
    }
    void LevelDown()
    {
        newCharacter.level--;
        newCharacter.stats.atq--;
        newCharacter.stats.teq--;
        newCharacter.stats.def--;
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
