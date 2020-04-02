using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class strategyUI : MonoBehaviour
{
    [SerializeField]
    GameObject estrategias;
    Button button;

    PVE_Manager mg;

    bool interacting = false;

    int cooldown = 60 * 10;
    int cooldownRef = 60 * 10;

    // Start is called before the first frame update
    void Start()
    {
        mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void LateUpdate() {
        interacting = false;
        if (cooldown == 60 * 2) estrategias.SetActive(false);
        if (cooldown > cooldownRef) button.interactable = true;
        else cooldown++;
        transform.GetChild(1).gameObject.SetActive(button.interactable);
    }

    public void guiInteracting() {
        interacting = true;
        if(estrategias.activeSelf)
        {
            estrategias.SetActive(false);
            cooldown = cooldownRef + 1;
        }
        else if(!estrategias.activeSelf)
        {
            estrategias.SetActive(true);
            cooldown = 0;
        }
    }

    public void setStrategy(int _strat)
    {
        if (!mg.GameOn) return;
        interacting = true;
        mg.setStrategyBonus(_strat);
        button.interactable = false;
        cooldown = 60 * 2 + 1;
        estrategias.SetActive(false);
    }

    public bool isInteracting() { return interacting; }
}
