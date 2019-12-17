
using UnityEngine;
using UnityEngine.UI;

public class CharacterRender : MonoBehaviour
{
    public CharacterBasic characterInfo;

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
        nameText.text = characterInfo.basicInfo.nameCharacter;
        descriptionText.text = characterInfo.basicInfo.description;
        shotText.text = characterInfo.stats.shot.ToString();
        defenseText.text = characterInfo.stats.defense.ToString();
        techinqueText.text = characterInfo.stats.technique.ToString();
        artworkImage.sprite = characterInfo.basicInfo.artwork;
    }
}
