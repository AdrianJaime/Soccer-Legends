using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAwaken : MonoBehaviour
{
    [SerializeField] Image artworkLocation;
    [SerializeField] Transform placeToSpawnAnimation;

    [SerializeField] Text levelMax,characterName;

    CharacterBasic character;
    CharacterBasic newCharacter;
    private void Start()
    {
        character = new CharacterBasic(StaticInfo.characterToAcces);
        newCharacter = new CharacterBasic(character);
        //get character from database with the id that hsa shared static info en vez de charazterbasic variable
        characterName.text = character.basicInfo.nameCharacter;


        if (character.basicInfo.animation2DObject == null)
        {
            artworkLocation.sprite = character.basicInfo.completeArtwork;
            artworkLocation.gameObject.SetActive(true);
        }
        else
        {
            //ReloadDBDataByAlejandro();
            artworkLocation.gameObject.SetActive(false);
            Instantiate(character.basicInfo.animation2DObject, placeToSpawnAnimation);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (character.levelMAX != newCharacter.levelMAX)
        {

            levelMax.text = character.info.level + "/" + newCharacter.levelMAX;//sustituir este 100 en funcion de su maximo alcanzado si awaken o no
        }
        else
        {
            levelMax.text =  character.info.level + "/" + character.levelMAX;//sustituir este 100 en funcion de su maximo alcanzado si awaken o no

        }
    }


    void LevelUpAwakening()
    {
        newCharacter.levelMAX++;

    }
    void LevelDownAwakening()
    {
        newCharacter.levelMAX--;

    }



    public void AddExpAwakening(int _exp)
    {
        if (newCharacter.currentExpAwakening + _exp > (newCharacter.currentExpAwakening + 2))//añadir la experiencia necesaria para subir de nivel del pj
        {
            LevelUpAwakening();
            UpdateUI();
        }
    }
    public void SubstractExpAwakening(int _exp)
    {
        if (newCharacter.currentExpAwakening - _exp < (newCharacter.currentExpAwakening - 2))
        {
            LevelDownAwakening();
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

