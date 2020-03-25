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
        IA_manager.strategy lastStrat = mg.myPlayers[0].transform.parent.GetComponent<IA_manager>().teamStrategy;
        mg.myPlayers[0].transform.parent.GetComponent<IA_manager>().teamStrategy = (IA_manager.strategy)_strat;
        foreach(GameObject player in mg.myPlayers)
        {
            MyPlayer_PVE playerScript = player.GetComponent<MyPlayer_PVE>();
            switch(player.transform.parent.GetComponent<IA_manager>().teamStrategy)
            {
                case IA_manager.strategy.DEFFENSIVE:
                    if(lastStrat == IA_manager.strategy.OFFENSIVE)
                    {
                        playerScript.stats.shoot = playerScript.stats.shoot - playerScript.stats.shoot / 3;
                        playerScript.stats.technique = playerScript.stats.technique - playerScript.stats.technique / 5;
                    }
                    playerScript.stats.defense = playerScript.stats.defense + playerScript.stats.defense / 2;
                    playerScript.stats.technique = playerScript.stats.technique + playerScript.stats.technique / 4;
                    break;
                case IA_manager.strategy.EQUILIBRATED:
                    if (lastStrat == IA_manager.strategy.OFFENSIVE)
                    {
                        playerScript.stats.shoot = playerScript.stats.shoot - playerScript.stats.shoot / 3;
                        playerScript.stats.technique = playerScript.stats.technique - playerScript.stats.technique / 5;
                    }
                    else if (lastStrat == IA_manager.strategy.DEFFENSIVE)
                    {
                        playerScript.stats.defense = playerScript.stats.defense - playerScript.stats.defense / 3;
                        playerScript.stats.technique = playerScript.stats.technique - playerScript.stats.technique / 5;
                    }
                    break;
                case IA_manager.strategy.OFFENSIVE:
                    if (lastStrat == IA_manager.strategy.DEFFENSIVE)
                    {
                        playerScript.stats.defense = playerScript.stats.defense - playerScript.stats.defense / 3;
                        playerScript.stats.technique = playerScript.stats.technique - playerScript.stats.technique / 5;
                    }
                    playerScript.stats.shoot = playerScript.stats.shoot + playerScript.stats.shoot / 2;
                    playerScript.stats.technique = playerScript.stats.technique + playerScript.stats.technique / 4;
                    break;
            }
        }
        button.interactable = false;
        cooldown = 60 * 2 + 1;
        estrategias.SetActive(false);
    }

    public bool isInteracting() { return interacting; }
}
