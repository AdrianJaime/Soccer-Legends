using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] EnergyRender energyRender;
    DateTime nextRestore;
    int totalEnergy = 0;
    int maxEnergy = 3;
    int restoreInterval = 5;

    // Start is called before the first frame update
    void Start()
    {
        LoadTimes();
        StartCoroutine(RestoreFunction());
    }

    private IEnumerator RestoreFunction()
    {
        while (totalEnergy < maxEnergy)
        {
            DateTime currentTime = DateTime.Now;
            DateTime nextTimeToRestore = nextRestore;

            while (currentTime > nextTimeToRestore)
            {
                if (totalEnergy < maxEnergy)
                {
                    ///Sumamos energia
                    totalEnergy++;
                    ///Añadomos intervalo
                    nextTimeToRestore = AddTimeInterval(nextTimeToRestore, restoreInterval);
                    //ACTUALIZAR VARIABLES
                    nextRestore = nextTimeToRestore;
                    //Subimos a BD
                    SaveTimes();
                }
                else
                    break;
            }
            if (totalEnergy == maxEnergy)
                energyRender.UpdateRender(true, totalEnergy);
            else
                energyRender.UpdateRender(nextRestore - DateTime.Now,totalEnergy);

            yield return null;
        }
    }

    private DateTime AddTimeInterval(DateTime _date, int _timeInterval)
    {
        return _date.AddSeconds(_timeInterval);
    }
    private void LoadTimes()
    {
        //Buscar en BD y asignar variables
        string dateTimeBD = null;//donde asignaremos la variable
        if (string.IsNullOrEmpty(dateTimeBD))
        {
            nextRestore = DateTime.Now;
            SaveTimes();
        }
        else
            nextRestore = StringToDate(dateTimeBD);
    }
    private void SaveTimes()
    {
        //Hacer comprobaciones de tiempos y si todo está correcto 
        //Guardar en BD los tiempos
    }
    private DateTime StringToDate(string date)
    {
        if (string.IsNullOrEmpty(date))
            return DateTime.Now;
        return DateTime.Parse(date);
    }

}
