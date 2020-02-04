using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Este Script se encarga de organizar el apartado visual dentro 
/// de una stage. Tambien se encarga de comprobar si la reward se ha 
/// conseguido o no a través de base de datos
/// </summary>
public class StageTournament : MonoBehaviour
{
    public Text description, stageName;
    public Image stageArt,artReward, clearPanel;
    public StageInfo basicInfo;//Información básica sobre ésta stage, variables que no varían
    public RewardLogic reward;

    public bool clear = true; 
    bool ownedReward = false;

    // Start is called before the first frame update
    void Start()
    {
        //leer de base de datos si se ha completado
        //
        UpdateUI();
    }

    void UpdateUI()
    {
        if (clear)
        {
            if (ownedReward)
                reward.SetReclaimed();
            else
                reward.UnlockReward();

            Destroy(clearPanel);
        }


        description.text = basicInfo.description;
        stageName.text = basicInfo.stageName;
        stageArt.sprite = basicInfo.stageArtwork;
        artReward.sprite = basicInfo.imageReward;
    }
}
