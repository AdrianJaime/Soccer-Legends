using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este Script se encarga de recoger la información 
/// básica que contendrá una stage
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName ="New_StageInfo", menuName ="Tournament/NewStageInfo")]
public class StageInfo : ScriptableObject
{
    //info player
    [System.Serializable]
    public struct stagePlayer
    {
        public CharacterInfo characterInfo;
        public MyPlayer_PVE.Stats stageStats;
    }
    [SerializeField]
    public List<stagePlayer> stageTeam;
    public string ID_DB;
    public string stageName;
    public string description;
    public bool isBoss;

    //info drawable
    public Sprite stageArtwork;
    public Sprite imageReward;

}
