using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagesManager : MonoBehaviour
{

    public Image teamImage;
    public Text nameTeam;

    public TeamTournamentInfo info; //publico para poner por defecto alguno en el testing
    public Transform placeToSpawn;
    public GameObject prefabStage;
    public List<StageTournament> Stages;

    public bool ownedReward = false;

    private void Awake()
    {
        if(StaticInfo.tournamentTeam!=null)
            info = StaticInfo.tournamentTeam;
        UpdateUI();
        SetUpStages();
    }

    void SetUpStages()
    {
        foreach(StageInfo stage in info.stages)
        {
            GameObject auxStage = Instantiate(prefabStage, placeToSpawn);
            //Le asignamos una informacion basica de jugador
            //Tambien habría que pasarle desde base de datos si esta completada la mision o no o si ha reclamado la recompensda o n
            auxStage.GetComponent<StageTournament>().clear = true;

            auxStage.GetComponent<StageTournament>().basicInfo = stage;

            Stages.Add(auxStage.GetComponent<StageTournament>());
        }
    }

    public void UpdateUI()
    {
        teamImage.sprite = info.artworkTeam;
        nameTeam.text = info.nameTeam;
    }
}
