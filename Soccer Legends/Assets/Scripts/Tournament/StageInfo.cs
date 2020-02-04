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
    public string ID_DB;
    public string stageName;
    public string description;

    //info drawable
    public Sprite stageArtwork;
    public Sprite imageReward;

}
