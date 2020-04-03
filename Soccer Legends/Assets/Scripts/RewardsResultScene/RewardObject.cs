using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardObject : MonoBehaviour
{
    public enum TypeReward { UNIQUE, COMMON, EXTRA, EVENT}
    public int number = 0;
    public TypeReward type;
    public ConsumBaseInfo baseInfo;

    //EDITOR
    [SerializeField]Image imageReward;
    [SerializeField]Text numberText;

    public void UpdateRender()
    {
        imageReward.sprite = baseInfo.image;
        numberText.text = number.ToString();
    }
}
