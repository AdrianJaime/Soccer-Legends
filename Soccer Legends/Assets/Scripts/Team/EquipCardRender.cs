
using UnityEngine;
using UnityEngine.UI;

public class EquipCardRender : MonoBehaviour
{
    //public CharacterBasic characterInfo;

    //display objects from de prefab
    [SerializeField] Text nameText, powerText;
    [SerializeField] Image artworkImage, borderColor, spriteStars, elementColor, role;
    public CharacterBasic characterInfo;

    [SerializeField] bool UtdateAtInit;


    //init resources
    //tema estrellas y cosas que se repiten entre cartas
    //como marcos o cosas asi
    [SerializeField] SpriteConpendiumSO borderColors, starSprites, roles;
    [SerializeField] ColorsScriptableObject elementColors;


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

                borderColor.sprite = borderColors.sprites[(int)characterInfo.basicInfo.rarity];
                spriteStars.sprite = starSprites.sprites[(int)characterInfo.basicInfo.rarity];
                elementColor.color = elementColors.colors[(int)characterInfo.basicInfo.type];

                role.sprite = roles.sprites[(int)characterInfo.basicInfo.rol];

            }
            else
            {
                artworkImage.sprite = null;
                Color transparent = artworkImage.color;
                transparent.a = 0;
                artworkImage.color = transparent;

                nameText.text = "None";
                powerText.text = "0";



                borderColor.sprite = borderColors.sprites[3];
                spriteStars.sprite = null;

                borderColor.sprite = borderColors.sprites[3];
                spriteStars.sprite = starSprites.sprites[0];
                elementColor.color = elementColors.colors[5];

                role.sprite = roles.sprites[0];


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

            borderColor.sprite = borderColors.sprites[3];
            spriteStars.sprite = starSprites.sprites[0];
            elementColor.color = elementColors.colors[5];
            role.sprite = roles.sprites[0];


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
