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
    [SerializeField] CharactersCompendium inventory;

    private void Awake()
    {
        LoadEquipBD();
    }

    public void LoadEquipBD()
    {
        string[] arrayID = new string[8];
        int aux = 0;
        for (int i = 0; i < 8; i++)
        {
            arrayID[i] = PlayerPrefs.GetString("team" + "1" + "slot" + i);
            Debug.Log("In " + "team" + "1" + "slot" + i + " There is: " + PlayerPrefs.GetString("team" + "1" + "slot" + i));
        }
        foreach (string value in arrayID)
        {
            if (value != null)
            {
                Debug.Log("BUSCO UN ID EN EL INVENTARIO");
                CharacterInfo auxCharacter = inventory.compendiumOfCharacters.Find(x => x.ID == value);
                if (auxCharacter != null)
                {
                    Debug.Log("LO ENCONTRÉ");
                    transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(aux)
                        .GetComponent<CharacterBasic>().basicInfo = auxCharacter;
                }
                else
                    Debug.Log("NO LO ENCONTRÉ DEJA DE PIRATEAR Y PONER IDs ERRONEOS");

            }
            aux++;

        }
    }

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
        List<CharacterBasic> rivalDeck = new List<CharacterBasic>(StaticInfo.rivalTeam);
        StaticInfo.rivalTeam.Clear();
        CharacterBasic[] rivalTeam = new CharacterBasic[4];
        List<CharacterBasic> charactersFound = new List<CharacterBasic>(rivalDeck);
        for (int i = 0; i < rivalTeam.Length; i++)
        {
            charactersFound.Clear();
            foreach (var rival in rivalDeck)
            {
                if (rival.basicInfo.rol == (Rol)i) charactersFound.Add(rival);
            }
            if (charactersFound.Count > 0)
            {
                CharacterBasic selectedCharacter = charactersFound[Random.Range(0, charactersFound.Count)];
                switch ((Rol)i)
                {
                    case Rol.PIVOT:
                        rivalTeam[1] = selectedCharacter;
                        rivalDeck.Remove(selectedCharacter);
                        break;
                    case Rol.WINGER:
                        rivalTeam[0] = selectedCharacter;
                        rivalDeck.Remove(selectedCharacter);
                        break;
                    case Rol.LAST_MAN:
                        rivalTeam[2] = selectedCharacter;
                        rivalDeck.Remove(selectedCharacter);
                        break;
                    case Rol.GOALKEEPER:
                        rivalTeam[3] = selectedCharacter;
                        rivalDeck.Remove(selectedCharacter);
                        break;
                }
            }
        }

        if (rivalDeck.Count > 0)
        {
            for (int i = 0; i < rivalTeam.Length; i++)
            {
                if (rivalTeam[i] == null)
                {
                    CharacterBasic selectedCharacter = rivalDeck[Random.Range(0, rivalDeck.Count)];
                    rivalTeam[i] = selectedCharacter;
                    rivalDeck.Remove(selectedCharacter);
                    if (rivalDeck.Count == 0) break;
                }
            }
        }

        StaticInfo.rivalTeam = new List<CharacterBasic>(rivalTeam);

        SceneManager.LoadScene("FormationScene");
    }
}
