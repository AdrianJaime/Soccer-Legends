
using UnityEngine;
using UnityEngine.UI;

public class EquipCardRender : MonoBehaviour
{
    //public CharacterBasic characterInfo;

    //display objects from de prefab
    public Text nameText;
    public Image artworkImage;
    public Text powerText;
    public CharacterBasic characterInfo;


    //init resources
    //tema estrellas y cosas que se repiten entre cartas
    //como marcos o cosas asi
    public Image[] starSprites;

    public void UpdateRender()
    {
        if (characterInfo != null)
        {
            Color opaque = artworkImage.color;
            opaque.a = 1;
            artworkImage.color = opaque;

            nameText.text = characterInfo.basicInfo.nameCharacter;
            artworkImage.sprite = characterInfo.basicInfo.artwork;
            powerText.text = characterInfo.power.ToString();
        }
        else
        {
            artworkImage.sprite = null;
            Color transparent= artworkImage.color;
            transparent.a = 0;
            artworkImage.color = transparent;

            nameText.text = "None";
            powerText.text = "0";
        }
    }
    public void Slected()
    {
        Color aux = artworkImage.color;
        aux.a = 0.5f;
        artworkImage.color = aux;
    }
    public void Diselected()
    {

        UpdateRender();
    }
}
