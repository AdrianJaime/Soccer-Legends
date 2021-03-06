﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultRewardManager : MonoBehaviour
{
    /// <summary>
    /// De momento esto es serializable hasta que se enlace con BD
    /// </summary>
    [System.Serializable]
    public struct BD_REWARD_DATA
    {
        public int number;
        public RewardObject.TypeReward type;
        public string _id;
    }

    public List<BD_REWARD_DATA> listOfLevelRewards;
    [SerializeField] Transform uniqueObjects, commonObjects, extraObjects, eventObjects;
    [SerializeField] GameObject rewardItemPrefab;
    [SerializeField] CompendiumItems compendiumItems;
    [SerializeField] GameObject layoutCorrector;
    public Text gold;

    // Start is called before the first frame update
    void Start()
    {
        LoadItemsBD();
        SetUpItems();

        gold.text = Random.Range(500, 1000).ToString();

    }

    /// <summary>
    ///Leer los items associados al nivel (CON EL ID DEL NIVEL) que se acaba de pasar el jugador y guardarlos en la lista;
    ///Los objetos berian leerse por listas de typo de reward.
    /// </summary>
    void LoadItemsBD()
    {

    }

    void SetUpItems()
    {
        bool commonActive,eventActive,extraActive,uniqueActive;
        commonActive=eventActive=extraActive=uniqueActive=false;

        foreach(BD_REWARD_DATA reward in listOfLevelRewards)
        {
            switch (reward.type)
            {
                case RewardObject.TypeReward.COMMON:
                    //activamos el invenatrio si no lo estaba
                    if (!commonActive)
                    {
                        commonActive = true;
                    }
                    ConsumBaseInfo itemBase=compendiumItems.compendiumOfItems.Find(x => x.ID == reward._id);
                    //Si lo ha encontrado lo instanciamos correctamente
                    if (itemBase != null)
                    {
                        GameObject rewardItem=Instantiate(rewardItemPrefab, commonObjects);
                       // rewardItem.transform.localScale = new Vector3(0,0,1);
                        RewardObject iteamInfo= rewardItem.GetComponent<RewardObject>();
                        iteamInfo.baseInfo = itemBase;
                        iteamInfo.number = reward.number;
                        iteamInfo.type = reward.type;
                        iteamInfo.UpdateRender();
                    }
                break;
                case RewardObject.TypeReward.EVENT:
                    //activamos el invenatrio si no lo estaba
                    if (!eventActive)
                    {
                        eventActive = true;
                    }
                    itemBase = compendiumItems.compendiumOfItems.Find(x => x.ID == reward._id);
                    //Si lo ha encontrado lo instanciamos correctamente
                    if (itemBase != null)
                    {
                        GameObject rewardItem = Instantiate(rewardItemPrefab, eventObjects);
                       // rewardItem.transform.localScale = new Vector3(0, 0, 1);
                        RewardObject iteamInfo = rewardItem.GetComponent<RewardObject>();
                        iteamInfo.baseInfo = itemBase;
                        iteamInfo.number = reward.number;
                        iteamInfo.type = reward.type;
                        iteamInfo.UpdateRender();
                    }
                    break;
                case RewardObject.TypeReward.EXTRA:
                    //activamos el invenatrio si no lo estaba
                    if (!extraActive)
                    {
                        extraActive = true;
                    }
                    itemBase = compendiumItems.compendiumOfItems.Find(x => x.ID == reward._id);
                    //Si lo ha encontrado lo instanciamos correctamente
                    if (itemBase != null)
                    {
                        GameObject rewardItem = Instantiate(rewardItemPrefab, extraObjects);
                        //rewardItem.transform.localScale = new Vector3(0, 0, 1);
                        RewardObject iteamInfo = rewardItem.GetComponent<RewardObject>();
                        iteamInfo.baseInfo = itemBase;
                        iteamInfo.number = reward.number;
                        iteamInfo.type = reward.type;
                        iteamInfo.UpdateRender();
                    }
                    break;
                case RewardObject.TypeReward.UNIQUE:
                    //activamos el invenatrio si no lo estaba
                    if (!uniqueActive)
                    {
                        uniqueActive = true;
                    }
                    itemBase=compendiumItems.compendiumOfItems.Find(x => x.ID == reward._id);
                    //Si lo ha encontrado lo instanciamos correctamente
                    if (itemBase != null)
                    {
                        GameObject rewardItem=Instantiate(rewardItemPrefab, uniqueObjects);
                       // rewardItem.transform.localScale = new Vector3(0, 0, 1);
                        RewardObject iteamInfo = rewardItem.GetComponent<RewardObject>();
                        iteamInfo.baseInfo = itemBase;
                        iteamInfo.number = reward.number;
                        iteamInfo.type = reward.type;
                        iteamInfo.UpdateRender();
                    }
                    break;
                default:
                break;

            }
        }

        if(commonActive)
            commonObjects.parent.gameObject.SetActive(true);
        if (eventActive)
            eventObjects.parent.gameObject.SetActive(true);
        if (extraActive)
            extraObjects.parent.gameObject.SetActive(true);
        if (uniqueActive)
            uniqueObjects.parent.gameObject.SetActive(true);

    }


    /// <summary>
    /// QUITAR ESTA PUTA MIERDA
    ///// </summary>
    private void Update()
    {
        layoutCorrector.SetActive (false);
        layoutCorrector.SetActive( true);
    }
}
