
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventoryCardRender : MonoBehaviour
{
    public CharacterInfo characterInfo;

    //display objects from de prefab
    public Text nameText;
    public Image artworkImage;
    public int identifierSlot = -1;


    //init resources
    //tema estrellas y cosas que se repiten entre cartas
    //como marcos o cosas asi
    public Image[] starSprites;
    EquipamentManager manager;

    private void Start()
    {
        manager = FindObjectOfType<EquipamentManager>();

        if (characterInfo != null)
        {
            nameText.text = characterInfo.nameCharacter;
            artworkImage.sprite = characterInfo.artwork;

            if (!characterInfo.owned)
            {
                artworkImage.color = Color.black;
            }

        }
    }

    public void OnClickSlot()
    {
        if(characterInfo.owned)
          manager.EquipCharacter(characterInfo);     
    }
    public void DisEquip()
    {
        manager.DisEquipCharacter(characterInfo);
    }
    public void OpenVharacterInfo()
    {
        StaticInfo.characterToAcces = characterInfo;
        SceneManager.LoadScene(2);

    }
}
