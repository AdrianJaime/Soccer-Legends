using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName ="New_TournamentTeamInfo", menuName ="Tournament/NewTeamInfo")]
public class TeamTournamentInfo : ScriptableObject
{
    //info player
    public string ID_DB;
    public string nameTeam;
    public string description;

    //info drawable
    public Sprite artworkTeam;
    public List<StageInfo> stages;

}
