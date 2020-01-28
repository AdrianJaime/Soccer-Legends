using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName ="New_StageInfo", menuName ="Tournament/NewStageInfo")]
public class StageInfo : ScriptableObject
{
    //info player
    public string ID_DB;
    public string nameStage;
    public string description;

    //info drawable
    public Sprite artworkStage;
    public Sprite imageReward;

}
