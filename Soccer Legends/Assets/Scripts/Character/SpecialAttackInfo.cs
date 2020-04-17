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
    int energyRequired;
    public float requiredEnergy { get { return energyRequired; } }
    [System.NonSerialized]
    public SpecialAttack specialAtack;
    [SerializeField]
    Object specialScript;

    public void LoadSpecialAtack()
    {
        switch(specialScript.name)
        {
            case "StatBuff":
                specialAtack = new StatBuff(this);
                break;
        }
    }
}

