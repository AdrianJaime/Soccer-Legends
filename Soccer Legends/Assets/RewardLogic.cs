using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardLogic : MonoBehaviour
{
    public bool canReclaim, reclaimed;

    public Image blockRewardImage, reclaimedImage;
    private void Start()
    {
        SetUp();
        UpdateUI();
    }

    private void SetUp()
    {
        reclaimedImage.enabled = false;
        blockRewardImage.enabled = true;

    }

    private void UpdateUI()
    {
        if (canReclaim)
        {
            blockRewardImage.enabled = false;
            reclaimedImage.enabled = false;
            if (reclaimed)
            {
                canReclaim = false;//Variables que deberian estar asignadas
                reclaimedImage.enabled = true;
                blockRewardImage.enabled = true;
            }
        }
        else
        {
            blockRewardImage.enabled = true;
            if (!reclaimed)
            {
                reclaimedImage.enabled = false;
            }
        }
    }

    public void ReclaimReward()
    {
        if (canReclaim)
        {
            //Logica de añadir a base de Datos
            reclaimed = true;
            UpdateUI();
        }
    }
    public void UnlockReward()
    {
        if (!canReclaim)
        {
            canReclaim = true;
            UpdateUI();
        }
    }
    public void SetReclaimed()
    {
        canReclaim = true;
        reclaimed = true;
        UpdateUI();
    }

}
