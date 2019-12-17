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
    public Test testVariable;

    public User(string _UN, int _US, Status _stats, Test _user2)
    {
        userName = _UN;
        userScore = _US;
        stats = _stats;
        testVariable = _user2;
        
    }
}
[Serializable]
public class Test
{
    public string stringTest;
    public int intTest;

    public Test(string _UN, int _US)
    {
        stringTest = _UN;
        intTest = _US;
        
    }
}