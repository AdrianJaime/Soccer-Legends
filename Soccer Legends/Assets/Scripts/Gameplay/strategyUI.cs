using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class strategyUI : MonoBehaviour
{
    [SerializeField]
    Text stratText;
    Scrollbar scrollbar;

    PVE_Manager mg;

    Vector3 startButtonPos = Vector3.zero;
    Vector3 lastButtonPos;

    bool interacting;

    int cooldown = 60 * 10;
    int cooldownRef = 60 * 10;

    // Start is called before the first frame update
    void Start()
    {
        mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
        scrollbar = GetComponent<Scrollbar>();
    }

    // Update is called once per frame
    void LateUpdate() {
        if (startButtonPos == Vector3.zero) { startButtonPos = scrollbar.handleRect.position; lastButtonPos = scrollbar.handleRect.position; }
        stratText.text = mg.myPlayers[0].transform.parent.GetComponent<IA_manager>().teamStrategy.ToString();
        interacting = false;

        if (isInteracting()) stratText.color = scrollbar.colors.pressedColor;
        else stratText.color = scrollbar.colors.disabledColor;

        if (cooldown == 60) scrollbar.interactable = false;
        if (cooldown > cooldownRef) scrollbar.interactable = true;
        else cooldown++;
        if (scrollbar.handleRect.position.y != lastButtonPos.y)
        {
            cooldown = 0;
            if (scrollbar.handleRect.position.y == startButtonPos.y)
                mg.myPlayers[0].transform.parent.GetComponent<IA_manager>().teamStrategy = IA_manager.strategy.EQUILIBRATED;
            else if (scrollbar.handleRect.position.y > startButtonPos.y)
                mg.myPlayers[0].transform.parent.GetComponent<IA_manager>().teamStrategy = IA_manager.strategy.OFFENSIVE;
            else if (scrollbar.handleRect.position.y < startButtonPos.y)
                mg.myPlayers[0].transform.parent.GetComponent<IA_manager>().teamStrategy = IA_manager.strategy.DEFFENSIVE;
        }

        lastButtonPos = scrollbar.handleRect.position;
    }

    public void guiInteracting() {
        interacting = true;
    }

    public bool isInteracting() { return cooldown < 60 ? true : false; }
}
