using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Status
{

    public int att;
    public int def;
    public int teq;
    public Status(int _att,int _def,int _teq)
    {
        att = _att;
        def = _def;
        teq = _teq;
    }
}
[Serializable]
public class User
{
    public string userName;
    public int userScore;
    public Status stats;
    public EquipDB equipTest;

    public User(string _UN, int _US, Status _stats)
    {
        userName = _UN;
        userScore = _US;
        stats = _stats;
        
    }
}
[Serializable]
public class EquipDB
{
    public int[] characterEquipIDs;

}