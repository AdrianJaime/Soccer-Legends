using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PVP_strategyUI : MonoBehaviour
{
    [SerializeField]
    Text stratText;
    Scrollbar scrollbar;

    Manager mg;

    Vector3 startButtonPos = Vector3.zero;
    Vector3 lastButtonPos;

    bool interacting;

    int cooldown = 60 * 10;
    int cooldownRef = 60 * 10;

    // Start is called before the first frame update
    void Start()
    {
        mg = GameObject.Find("Manager").GetComponent<Manager>();
        scrollbar = GetComponent<Scrollbar>();
        lastButtonPos = startButtonPos = Vector3.zero;
    }

    // Update is called once per frame
    void LateUpdate() {
        stratText.text = mg.myPlayers[0].transform.parent.GetComponent<PVP_IA_manager>().teamStrategy.ToString();
        interacting = false;

        if (isInteracting()) stratText.color = scrollbar.colors.pressedColor;
        else stratText.color = scrollbar.colors.disabledColor;

        if (cooldown == 60) scrollbar.interactable = false;
        if (cooldown > cooldownRef) scrollbar.interactable = true;
        else cooldown++;
        if (scrollbar.handleRect.transform.localPosition.y != lastButtonPos.y)
        {
            cooldown = 0;
            if (scrollbar.handleRect.transform.localPosition.y == startButtonPos.y)
                mg.myPlayers[0].transform.parent.GetComponent<PVP_IA_manager>().teamStrategy = IA_manager.strategy.EQUILIBRATED;
            else if (scrollbar.handleRect.transform.localPosition.y > startButtonPos.y)
                mg.myPlayers[0].transform.parent.GetComponent<PVP_IA_manager>().teamStrategy = IA_manager.strategy.OFFENSIVE;
            else if (scrollbar.handleRect.transform.localPosition.y < startButtonPos.y)
                mg.myPlayers[0].transform.parent.GetComponent<PVP_IA_manager>().teamStrategy = IA_manager.strategy.DEFFENSIVE;
        }

        lastButtonPos = scrollbar.handleRect.transform.localPosition;
    }

    public void guiInteracting() {
        interacting = true;
    }

    public bool isInteracting() { return cooldown < 60 ? true : false; }
}
