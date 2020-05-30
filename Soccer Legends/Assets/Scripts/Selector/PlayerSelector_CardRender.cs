using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector_CardRender : MonoBehaviour
{
    //display objects from de prefab
    //display objects from de prefab
    [SerializeField] Text nameText;
    [SerializeField] Image artworkImage,cardImage;
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

        private void Start()
    {
        if(characterInfo != null&&UtdateAtInit)
            UpdateRender();
    }

    public void UpdateRender()
    {
        if (characterInfo != null)
        {
            if (GetComponent<PlayerSelector_CardSelection>() == null && transform.parent.parent.parent.parent.parent
                .GetComponent<PlayerSelector_Representation>() == null && transform.GetSiblingIndex() <
                StaticInfo.tournamentTeam.stages[StaticInfo.tournamentTeam.selectedStage].stageTeam.Count)
            {
                transform.parent.parent.parent.parent.GetChild(1).GetComponent<Text>().text = StaticInfo.tournamentTeam.teamName;
                characterInfo.basicInfo = StaticInfo.tournamentTeam.stages[StaticInfo.tournamentTeam.selectedStage]
                    .stageTeam[transform.GetSiblingIndex()].characterInfo;
                MyPlayer_PVE.Stats statsData = StaticInfo.tournamentTeam.stages[StaticInfo.tournamentTeam.selectedStage]
                    .stageTeam[transform.GetSiblingIndex()].stageStats;
                characterInfo.info = new CharacterBasic.data(statsData.shoot, statsData.technique, statsData.defense);
                StaticInfo.rivalTeam.Add(characterInfo);
            }
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
    //public void UpdateRender()
    //{
    //    if (characterInfo.basicInfo != null)
    //    {
    //        Color opaque = artworkImage.color;
    //        opaque.a = 1;
    //        artworkImage.color = opaque;

    //        nameText.text = characterInfo.basicInfo.nameCharacter;
    //        artworkImage.sprite = characterInfo.basicInfo.artworkIcon;
    //        powerText.text = characterInfo.power.ToString();
    //    }

    //}
    public void SelectedRender()
    {
        Color selectedColor = Color.green;
        selectedColor.a = 1;
        cardImage.color = selectedColor;
    }
    public void DiselectedRender()
    {
        Color diselectedColor = Color.white;
        cardImage.color = diselectedColor;
    }
}
