using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSelector_PlayerManager : MonoBehaviour
{

    public List<CharacterBasic> characterSelected;
    public Button confirmationButton;

    private void Start()
    {
        confirmationButton.interactable = false;
    }

    public bool AddPlayerSelected(CharacterBasic player)
    {
        if (characterSelected.Count < 4)
        {
            characterSelected.Add(player);
            if(characterSelected.Count == 4)
                confirmationButton.interactable = true;
            return true;

        }
        return false;
    }
    public void ErasePlayerSelected(CharacterBasic player)
    {
        characterSelected.Remove(player);
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
