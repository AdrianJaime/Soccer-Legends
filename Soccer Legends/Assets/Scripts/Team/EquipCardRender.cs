
using UnityEngine;
using UnityEngine.UI;

public class EquipCardRender : MonoBehaviour
{
    //public CharacterBasic characterInfo;

    //display objects from de prefab
    [SerializeField] Text nameText;
    [SerializeField] Image artworkImage;
    [SerializeField] Text powerText;
    public CharacterBasic characterInfo;
    [SerializeField] Image borderColor;
    [SerializeField] Image spriteStars;
    [SerializeField] Image elementColor;

    [SerializeField] bool UtdateAtInit;


    //init resources
    //tema estrellas y cosas que se repiten entre cartas
    //como marcos o cosas asi
    [SerializeField] Sprite[] borderColors;
    [SerializeField] Sprite[] starSprites;
    [SerializeField] Color[] elementColors;


    //Es mejor esperar a que indique un script por encima que haga update el render
    private void Start()
    {
        if (UtdateAtInit)
            UpdateRender();
    }

    public void UpdateRender()
    {
        if (characterInfo != null)
        { 
            if (characterInfo.basicInfo != null)
            {
                Color opaque = artworkImage.color;
                opaque.a = 1;
                artworkImage.color = opaque;

                nameText.text = characterInfo.basicInfo.nameCharacter;
                artworkImage.sprite = characterInfo.basicInfo.artworkIcon;
                powerText.text = characterInfo.power.ToString();

                borderColor.sprite = borderColors[(int)characterInfo.basicInfo.rarity];
                spriteStars.sprite = starSprites[(int)characterInfo.basicInfo.rarity];
                elementColor.color = elementColors[(int)characterInfo.basicInfo.type];
            }
            else
            {
                artworkImage.sprite = null;
                Color transparent = artworkImage.color;
                transparent.a = 0;
                artworkImage.color = transparent;

                nameText.text = "None";
                powerText.text = "0";



                borderColor.sprite = borderColors[3];
                spriteStars.sprite = null;

                borderColor.sprite = borderColors[3];
                spriteStars.sprite = starSprites[0];
                elementColor.color = elementColors[5];

            }
        }
        else 
        {
            artworkImage.sprite = null;
            Color transparent = artworkImage.color;
            transparent.a = 0;
            artworkImage.color = transparent;

            nameText.text = "None";
            powerText.text = "0";

            borderColor.sprite = borderColors[3];
            spriteStars.sprite = starSprites[0];
            elementColor.color = elementColors[5];

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
