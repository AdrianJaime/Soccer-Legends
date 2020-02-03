using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector_CardRender : MonoBehaviour
{
    //display objects from de prefab
    public Text nameText;
    public Image artworkImage;
    public Image cardImage;
    public Text powerText;
    public CharacterBasic characterInfo;


    //init resources
    //tema estrellas y cosas que se repiten entre cartas
    //como marcos o cosas asi
    public Image[] starSprites;

    private void Start()
    {
        if(characterInfo != null)
            UpdateRender();
    }
    public void UpdateRender()
    {
        if (characterInfo.basicInfo != null)
        {
            Color opaque = artworkImage.color;
            opaque.a = 1;
            artworkImage.color = opaque;

            nameText.text = characterInfo.basicInfo.nameCharacter;
            artworkImage.sprite = characterInfo.basicInfo.artwork;
            powerText.text = characterInfo.power.ToString();
        }

    }
    public void SelectedRender()
    {
        Color selectedColor = Color.green;
        selectedColor.a = 1;
        cardImage.color = selectedColor;
    }
    public void DiselectedRender()
    {
        Color diselectedColor = Color.white;
        diselectedColor.a = 0.5f;
        cardImage.color = diselectedColor;
    }
}
