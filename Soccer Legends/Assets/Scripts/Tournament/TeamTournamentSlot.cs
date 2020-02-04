﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Se encarga de la visualización de los institutos en el menu 
/// de tournament, donde se debe seleccionar.
/// </summary>
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
        nameTeam.text = basicInfo.teamName;
        artTeam.sprite = basicInfo.teamArtwork;
    }

    public void OpenTeam()
    {
        StaticInfo.tournamentTeam = basicInfo;
        SceneManager.LoadScene("StageScene");
    }
}
