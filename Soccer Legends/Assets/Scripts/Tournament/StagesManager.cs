using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Este script se encarga de que las stages se spawneen correctamente 
/// segun el numero/datos de BD; y adapta su contenido visualmente.
/// </summary>
public class StagesManager : MonoBehaviour
{

    public TeamTournamentInfo info; //publico para poner por defecto alguno en el testing
    public Transform placeToSpawn;
    public GameObject stagePrefab;
    public List<StageTournament> stages;

    public bool ownedReward = false;

    private void Awake()
    {
        if(StaticInfo.tournamentTeam!=null)
            info = StaticInfo.tournamentTeam;
        SetUpStages();
    }

    void SetUpStages()
    {
        foreach(StageInfo stage in info.stages)
        {
            GameObject auxStage = Instantiate(stagePrefab, placeToSpawn);
            //Le asignamos una informacion basica de jugador
            //Tambien habría que pasarle desde base de datos si esta completada la mision o no o si ha reclamado la recompensda o n
            auxStage.GetComponent<StageTournament>().clear = true;

            auxStage.GetComponent<StageTournament>().basicInfo = stage;

            stages.Add(auxStage.GetComponent<StageTournament>());
        }
    }
}
