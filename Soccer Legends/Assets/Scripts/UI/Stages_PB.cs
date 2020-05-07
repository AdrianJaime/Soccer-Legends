using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Este Script regula la progressBar del menú de Stages dentro de un 
/// instituto. Comprueba el numero de misiones superadas frente al numero 
/// total y actua en consecuencia.
/// </summary>
public class Stages_PB : MonoBehaviour
{

    public StagesManager stagesManager;
    public Image rewardTeamStages;
    //public RewardLogic reward;
    public Text percent;

    private void Start()
    {
        SetUpSlider();
    }

    public void SetUpSlider()
    {
        int stagesCount = stagesManager.info.stages.Count;

        gameObject.GetComponent<Slider>().maxValue = stagesCount;

        int clearedStages = 0;
        //estos clearedSta¡ges se deberian leer de BD teniendo cada stage un int 
        foreach (StageTournament stage in stagesManager.stages)
        {
            if (stage.clear)
                clearedStages++;
        }
        gameObject.GetComponent<Slider>().value = clearedStages;

        if(stagesCount!=0)
            percent.text = ((clearedStages / stagesCount) * 100).ToString()+"%";
        else
            percent.text = "0%";

        //if (!stagesManager.ownedReward)
        //{
        //    if (stagesCount == clearedStages)
        //    {
        //        //
        //        reward.UnlockReward();
        //    }
                
        //}

    }

}
