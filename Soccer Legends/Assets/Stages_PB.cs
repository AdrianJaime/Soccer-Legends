using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stages_PB : MonoBehaviour
{

    public StagesManager stagesManager;
    public Image rewardTeamStages;
    public RewardLogic reward;

    private void Start()
    {
        SetUpSlider();
    }

    public void SetUpSlider()
    {
        int stagesCount = stagesManager.info.stages.Count;

        gameObject.GetComponent<Slider>().maxValue = stagesCount;

        int clearedStages = 0;
        foreach (StageTournament stage in stagesManager.Stages)
        {
            if (stage.clear)
                clearedStages++;
        }
        gameObject.GetComponent<Slider>().value = clearedStages;

        if (!stagesManager.ownedReward)
        {
            if (stagesCount == clearedStages)
            {
                //
                reward.UnlockReward();
            }
                
        }

    }

}
