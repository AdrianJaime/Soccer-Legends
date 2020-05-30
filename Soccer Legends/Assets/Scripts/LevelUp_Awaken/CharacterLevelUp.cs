using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLevelUp : MonoBehaviour
{

    [SerializeField] Image artworkLocation, namePlaceImageRarity;
    [SerializeField] Text oldAtkTextLocation, oldTeqTextLocation, oldDefTextLocation;
    [SerializeField] Text newAtkTextLocation, newTeqTextLocation, newDefTextLocation;
    [SerializeField] Text CPtext;
    [SerializeField] TMPro.TextMeshPro level;
    [SerializeField] TMPro.TextMeshProUGUI  characterName;
    [SerializeField] Transform placeToSpawnAnimation;
    [SerializeField] ColorsScriptableObject rarityColors;

    public CharacterBasic character;
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


        namePlaceImageRarity.color = rarityColors.colors[(int)character.basicInfo.rarity];
        level.text = character.info.level + "/" + character.levelMAX;
        CPtext.text = (character.info.atk + character.info.def + character.info.teq).ToString();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (character.info.level != newCharacter.info.level)
        {
            newAtkTextLocation.text = newCharacter.info.atk.ToString();
            newTeqTextLocation.text = newCharacter.info.teq.ToString();
            newDefTextLocation.text = newCharacter.info.def.ToString();

            level.text = newCharacter.info.level + "/" + newCharacter.levelMAX;//sustituir este 100 en funcion de su maximo alcanzado si awaken o no
        }
        else
        {
            oldAtkTextLocation.text = character.info.atk.ToString();
            oldTeqTextLocation.text = character.info.teq.ToString();
            oldDefTextLocation.text = character.info.def.ToString();
            newAtkTextLocation.text = "-";
            newTeqTextLocation.text = "-";
            newDefTextLocation.text = "-";
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
        if(newCharacter.currentExp+_exp> (newCharacter.currentExp + 2)&&newCharacter.info.level<newCharacter.levelMAX)
        {
            LevelUp();
            UpdateUI();
        }
    }
    public void SubstractExp(int _exp)
    {
        if (newCharacter.currentExp - _exp < (newCharacter.currentExp - 2) && newCharacter.info.level > character.info.level)
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
