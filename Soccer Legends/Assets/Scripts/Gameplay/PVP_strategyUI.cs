﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PVP_strategyUI : MonoBehaviour
{

    Manager mg;

    bool interacting = false;

    double cooldown = 60 * 10;

    // Start is called before the first frame update
    void Start()
    {
        mg = GameObject.Find("Manager").GetComponent<Manager>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        interacting = false;
        if (cooldown + 10.0f < Time.time && GetComponent<Image>().color.a == 0.5f)
        {
            Button[] stratButtons = transform.GetComponentsInChildren<Button>(true);
            foreach (var _button in stratButtons) _button.interactable = true;
            Color c = GetComponent<Image>().color;
            c.a = 1.0f;
            GetComponent<Image>().color = c;
        }
    }

    public void setStrategy(int _strat)
    {
        if (!mg.GameOn) return;
        interacting = true;
        mg.photonView.RPC("setStrategyBonus", Photon.Pun.RpcTarget.AllViaServer, _strat, mg.myPlayers[0].GetComponent<MyPlayer>().photonView.ViewID);
        cooldown = Time.time;
        Button[] stratButtons = transform.GetComponentsInChildren<Button>();
        foreach (var _button in stratButtons) _button.interactable = false;
        Color c = GetComponent<Image>().color;
        c.a = 0.5f;
        GetComponent<Image>().color = c;
    }

    public bool isInteracting() { return interacting; }
}