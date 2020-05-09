using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class strategyUI : MonoBehaviour
{
    PVE_Manager mg;

    bool interacting = false;

    double cooldown = 60 * 10;

    // Start is called before the first frame update
    void Start()
    {
        mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
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
        else if (!mg.GameOn) cooldown += Time.deltaTime;
    }

    public void setStrategy(int _strat)
    {
        if (!mg.GameOn || (int)mg.myPlayers[0].transform.parent.GetComponent<IA_manager>().teamStrategy == _strat)
        {
            interacting = true;
            return;
        }
        InstantiateTap();
        interacting = true;
        mg.myPlayers[0].transform.parent.GetComponent<IA_manager>().teamStrategy = (IA_manager.strategy)_strat;
        cooldown = Time.time;
        Button[] stratButtons = transform.GetComponentsInChildren<Button>();
        foreach (var _button in stratButtons) _button.interactable = false;
        Color c = GetComponent<Image>().color;
        c.a = 0.5f;
        GetComponent<Image>().color = c;
    }

    public bool isInteracting() { return interacting; }

    void InstantiateTap()
    {
        foreach (var tap in Input.touches)
        {
            if (tap.phase == TouchPhase.Ended)
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(tap.position.x, tap.position.y, 0));
                Instantiate(mg.circleTapPrefab, worldPos, mg.circleTapPrefab.transform.rotation, transform.parent.parent);
            }
        }
    }
}
