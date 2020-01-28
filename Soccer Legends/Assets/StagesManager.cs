using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagesManager : MonoBehaviour
{
    TeamTournamentInfo info;
    public Transform placeToSpawn;
    public GameObject prefabStage;
    private void Start()
    {
        info = StaticInfo.tournamentTeam;
        SetUpStages();
    }

    void SetUpStages()
    {
        foreach(StageInfo stage in info.stages)
        {
            GameObject auxStage = Instantiate(prefabStage, placeToSpawn);
            //Le asignamos una informacion basica de jugador
            auxStage.GetComponent<StageTournament>().basicInfo = stage;
        }
    }
}
