
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventoryCardRender : MonoBehaviour
{
    public CharacterBasic characterInfo;

    //display objects from de prefab
    public Text nameText;
    public Image artworkImage;


    //init resources
    //tema estrellas y cosas que se repiten entre cartas
    //como marcos o cosas asi
    public Image[] starSprites;
    EquipamentManager manager;

    private void Start()
    {
        manager = FindObjectOfType<EquipamentManager>();
        UpdateSlotRender();

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
        StaticInfo.previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(2);

    }

    public void UpdateSlotRender()
    {
        if (characterInfo != null)
        {
            nameText.text = characterInfo.basicInfo.nameCharacter;
            artworkImage.sprite = characterInfo.basicInfo.artwork;

            if (!characterInfo.owned)
            {
                artworkImage.color = Color.black;
            }

        }
    }
}
