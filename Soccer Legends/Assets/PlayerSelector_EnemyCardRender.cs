using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector_EnemyCardRender : MonoBehaviour
{
    //display objects from de prefab
    public Text nameText;
    public Image artworkImage;
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
}
