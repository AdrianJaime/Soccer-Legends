﻿
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventoryCardRender : MonoBehaviour
{
    public CharacterBasic characterInfo;

    //display objects from de prefab
    [SerializeField] Image artworkImage;
    [SerializeField] Text powerText;
    [SerializeField] Image borderColor;
    [SerializeField] Image elementColor;
    [SerializeField] Image spriteStars;


    //init resources
    //tema estrellas y cosas que se repiten entre cartas
    //como marcos o cosas asi
    [SerializeField] Sprite[] starSprites;
    [SerializeField] Sprite[] borderColors;
    [SerializeField] Color[] elementColors;

    EquipamentManager manager;

    private void Start()
    {
        manager = FindObjectOfType<EquipamentManager>();
    }

    private void Update()
    {
        UpdateSlotRender();
    }

    public void OnClickSlot()
    {
        if(characterInfo.info.owned)
          manager.EquipCharacter(characterInfo);     
    }
    public void DisEquip()
    {
        manager.DisEquipCharacter(characterInfo);
    }
    public void OpenCharacterInfo()
    {
        if (characterInfo.info.owned)
        {
            StaticInfo.characterToAcces = characterInfo;
            StaticInfo.previousScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene("CharacterInfoScene");
        }
    }
    public void OpenLevelUpMenu()
    {
        StaticInfo.characterToAcces = characterInfo;
        StaticInfo.previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("LevelUpScene");

    }
    public void OpenAwakenMenu()
    {
        StaticInfo.characterToAcces = characterInfo;
        StaticInfo.previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("AwakenScene");

    }

    public void UpdateSlotRender()
    {
        if (characterInfo != null)
        {
            artworkImage.sprite = characterInfo.basicInfo.artworkIcon;

            borderColor.sprite = borderColors[(int)characterInfo.basicInfo.rarity];
            elementColor.color = elementColors[(int)characterInfo.basicInfo.type];
            spriteStars.sprite = starSprites[(int)characterInfo.basicInfo.rarity];

            if (!characterInfo.info.owned)
            {
                artworkImage.color = Color.black;
            }
            else
                artworkImage.color = Color.white;
        }
    }
}
