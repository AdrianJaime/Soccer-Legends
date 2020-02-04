using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este Script se encarga de recoger la información básica 
/// que contendrá una entrada del Torneo, es decir, un instituto
/// </summary>
[CreateAssetMenu(fileName ="New_TournamentTeamInfo", menuName ="Tournament/NewTeamInfo")]
public class TeamTournamentInfo : ScriptableObject
{
    //info player
    public string ID_DB;
    public string teamName;
    public string description;

    //info drawable
    public Sprite teamArtwork;
    public List<StageInfo> stages; //lista con la infromación básica de cada stage dentro de este instituto

}
