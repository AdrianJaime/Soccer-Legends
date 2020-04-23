using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorTEQBuff : SpecialAttack
{
    SpecialAttackInfo specialAttackInfo;

    public FlavorTEQBuff() { }
    public FlavorTEQBuff(SpecialAttackInfo _specialAttackInfo) { specialAttackInfo = _specialAttackInfo; }

    public override bool canUseSpecial(PVE_Manager mg, GameObject specialOwner, float energy)
    {
        return energy >= specialAttackInfo.requiredEnergy && mg.state == PVE_Manager.fightState.FIGHT &&
            specialOwner.GetComponent<MyPlayer_PVE>().ball != null;
    }

    public override bool canUseSpecial(Manager mg, GameObject specialOwner, float energy)
    {
        return energy >= specialAttackInfo.requiredEnergy && mg.state == Manager.fightState.FIGHT &&
            specialOwner.GetComponent<MyPlayer>().ball != null;
    }

    public override IEnumerator callSpecial(PVE_Manager mg, GameObject specialOwner, GameObject rival)
    {
        while (specialOwner.GetComponent<MyPlayer_PVE>().fightDir == null ||
            specialOwner.GetComponent<MyPlayer_PVE>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        for(int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>()
                .characterBasic.basicInfo.school == School.FLAVOR && 
                specialOwner.transform.parent.GetChild(i).gameObject != specialOwner)
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.technique +=
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.technique * 10) / 100;
            else if(specialOwner.transform.parent.GetChild(i).gameObject == specialOwner)
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.technique +=
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.technique * 30) / 100;
        }
            
        yield break;
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        while (specialOwner.GetComponent<MyPlayer>().fightDir == null ||
            specialOwner.GetComponent<MyPlayer>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        for (int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>()
                .characterBasic.basicInfo.school == School.FLAVOR &&
                specialOwner.transform.parent.GetChild(i).gameObject != specialOwner)
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.technique +=
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.technique * 10) / 100;
            else if (specialOwner.transform.parent.GetChild(i).gameObject == specialOwner)
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.technique +=
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.technique * 30) / 100;
        }

        yield break;
    }
}
