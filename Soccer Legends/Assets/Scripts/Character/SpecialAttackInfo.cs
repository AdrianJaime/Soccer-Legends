using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New_SpecialAttackInfo", menuName = "Cards/NewSpecialAttack")]
public class SpecialAttackInfo : ScriptableObject
{
    [SerializeField]
    protected string name;
    [SerializeField]
    protected string description;
    [SerializeField]
    protected int energyRequired;
    [System.NonSerialized]
    public SpecialAtack specialAtack;
    [SerializeField]
    Object specialScript;

    public void LoadSpecialAtack()
    {
        switch(specialScript.name)
        {
            case "StatBuff":
                specialAtack = new StatBuff();
                break;
        }
    }
}

