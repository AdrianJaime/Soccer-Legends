using System.Collections;
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
    public TeamTournamentInfo basicInfo;
    public bool canEnter = false;
    [SerializeField] Color blockColor;
    [SerializeField] Image image;
    [SerializeField] Button button;
    

    private void Start()
    {
        if (canEnter)
        {
            image.color = Color.white;
            button.enabled = true;
        }
        else
        {
            image.color = blockColor;
            button.enabled = false;
        }
    }

    public void OpenTeam()
    {
        StaticInfo.tournamentTeam = basicInfo;
        SceneManager.LoadScene("Stage_Scene");
    }
}
