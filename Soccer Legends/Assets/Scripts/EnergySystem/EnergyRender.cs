using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyRender : MonoBehaviour
{
    [SerializeField] Text energyText, textTimer;


    public void UpdateRender(bool _isFull, int _energy)
    {
        textTimer.text = "FULL ENERGY";
        energyText.text = _energy.ToString();

    }
    public void UpdateRender(TimeSpan timeToWait,int _energy)
    {
        string timeToWaitValue = timeToWait.Minutes.ToString() + "min. " + timeToWait.Seconds.ToString() + "s.";
        textTimer.text = timeToWaitValue;
        energyText.text = _energy.ToString();

    }
}
