using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEFBuff : SpecialAttack
{
    SpecialAttackInfo specialAttackInfo;

    public DEFBuff() { }
    public DEFBuff(SpecialAttackInfo _specialAttackInfo) { specialAttackInfo = _specialAttackInfo; }

    public override bool canUseSpecial(PVE_Manager mg, GameObject specialOwner, float energy)
    {
        return energy >= specialAttackInfo.requiredEnergy && mg.state == PVE_Manager.fightState.FIGHT &&
            specialOwner.GetComponent<MyPlayer_PVE>().ball == null;
    }

    public override bool canUseSpecial(Manager mg, GameObject specialOwner, float energy)
    {
        return energy >= specialAttackInfo.requiredEnergy && mg.state == Manager.fightState.FIGHT &&
            specialOwner.GetComponent<MyPlayer>().ball == null;
    }

    public override IEnumerator callSpecial(PVE_Manager mg, GameObject specialOwner, GameObject rival)
    {
        while (specialOwner.GetComponent<MyPlayer_PVE>().fightDir == null ||
            specialOwner.GetComponent<MyPlayer_PVE>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        mg.statsUpdate(!specialOwner.transform.parent.GetComponent<IA_manager>().playerTeam,
            0, 0, (specialOwner.GetComponent<MyPlayer_PVE>().stats.defense * 10) / 100);
            
        yield break;
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        while (specialOwner.GetComponent<MyPlayer>().fightDir == null ||
            specialOwner.GetComponent<MyPlayer>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        mg.statsUpdate(specialOwner.GetComponent<MyPlayer>().photonView.ViewID, 
            0, 0, (specialOwner.GetComponent<MyPlayer>().stats.defense * 10) / 100);

        yield break;
    }
}
