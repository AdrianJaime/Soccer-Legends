﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTEQDebuff : SpecialAttack
{
    SpecialAttackInfo specialAttackInfo;

    public EnemyTEQDebuff() { }
    public EnemyTEQDebuff(SpecialAttackInfo _specialAttackInfo) { specialAttackInfo = _specialAttackInfo; }

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
            rival.GetComponent<MyPlayer_PVE>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        mg.statsUpdate(!rival.transform.parent.GetComponent<IA_manager>().playerTeam,
            0, -(rival.GetComponent<MyPlayer_PVE>().stats.technique * 10) / 100, 0);
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        while (specialOwner.GetComponent<MyPlayer>().fightDir == null ||
            rival.GetComponent<MyPlayer>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        mg.statsUpdate(rival.GetComponent<MyPlayer>().photonView.ViewID,
            0, -(rival.GetComponent<MyPlayer>().stats.technique * 10) / 100, 0);
    }
}
