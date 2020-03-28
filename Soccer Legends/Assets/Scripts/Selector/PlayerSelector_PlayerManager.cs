using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSelector_PlayerManager : MonoBehaviour
{

    public List<CharacterBasic> characterSelected;
    public Button confirmationButton;

    [SerializeField] Image []ImagesSelectedSlots;
    [SerializeField] Animator []animatorsSlotsSelectors;

    private void Start()
    {
        confirmationButton.interactable = false;
    }

    public bool AddPlayerSelected(CharacterBasic player)
    {
        int aux = 0;
        if (characterSelected.Count < 4)
        {
            foreach (Image image in ImagesSelectedSlots)
            {
                if (image.sprite == null)
                {
                    image.sprite = player.basicInfo.artworkSelectorIcon;
                    characterSelected.Add(player);
                    animatorsSlotsSelectors[aux].SetTrigger("Show");
                    if (characterSelected.Count == 4)
                        confirmationButton.interactable = true;
                    return true;
                }
                aux++;
            }



        }
        return false;
    }
    public void ErasePlayerSelected(CharacterBasic player)
    {
        int aux = 0;
        foreach(Image image in ImagesSelectedSlots)
        {
            if (image.sprite == player.basicInfo.artworkSelectorIcon)
            {
                animatorsSlotsSelectors[aux].SetTrigger("Hide");
                characterSelected.Remove(player);

                break;
            }
            aux++;

        }

        if (confirmationButton.IsInteractable()==true)
            confirmationButton.interactable = false;      
    }

    public void OnConfirmationClick()
    {
        StaticInfo.teamSelectedToPlay = characterSelected;
        StaticInfo.rivalTeam = new List<CharacterBasic>(characterSelected);
        SceneManager.LoadScene("FormationScene");
    }
}
