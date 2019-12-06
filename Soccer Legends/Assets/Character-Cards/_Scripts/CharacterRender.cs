using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterRender : MonoBehaviour
{
    public CharacterInfo characterInfo;

    //display objects from de prefab
    public Text nameText;
    public Text descriptionText;
    public Text shotText;
    public Text defenseText;
    public Text techinqueText;
    public Image artworkImage;


    //init resources
    //tema estrellas y cosas que se repiten entre cartas
    //como marcos o cosas asi
    public Image[] starSprites;


    public void Start()
    {
        nameText.text = characterInfo.nameCharacter;
        descriptionText.text = characterInfo.description;
        shotText.text = characterInfo.stats.shot.ToString();
        defenseText.text = characterInfo.stats.defense.ToString();
        techinqueText.text = characterInfo.stats.technique.ToString();
        artworkImage.sprite = characterInfo.artwork;
    }
}
