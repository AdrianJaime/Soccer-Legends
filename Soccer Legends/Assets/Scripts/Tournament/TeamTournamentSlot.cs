using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TeamTournamentSlot : MonoBehaviour
{
    public Text description, nameTeam;
    public Image artTeam;

    public TeamTournamentInfo basicInfo;
    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        description.text = basicInfo.description;
        nameTeam.text = basicInfo.nameTeam;
        artTeam.sprite = basicInfo.artworkTeam;
    }

    public void OpenTeam()
    {
        StaticInfo.tournamentTeam = basicInfo;
        SceneManager.LoadScene("MissionMenu");
    }
}
