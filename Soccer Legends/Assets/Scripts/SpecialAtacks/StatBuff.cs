using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuff : SpecialAttack
{
    SpecialAttackInfo specialAttackInfo;

    public StatBuff() { }
    public StatBuff(SpecialAttackInfo _specialAttackInfo) { specialAttackInfo = _specialAttackInfo; }

    public override bool canUseSpecial(PVE_Manager mg, GameObject specialOwner, float energy)
    {
        return energy >= specialAttackInfo.requiredEnergy;
    }

    public override bool canUseSpecial(Manager mg, GameObject specialOwner, float energy)
    {
        return energy >= specialAttackInfo.requiredEnergy;
    }

    public override IEnumerator callSpecial(PVE_Manager mg, GameObject specialOwner, GameObject rival)
    {
        specialOwner.GetComponent<MyPlayer_PVE>().stats.shoot *= 3;
        specialOwner.GetComponent<MyPlayer_PVE>().stats.defense *= 3;
        specialOwner.GetComponent<MyPlayer_PVE>().stats.technique *= 3;

        while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);

        specialOwner.GetComponent<MyPlayer_PVE>().stats.shoot /= 3;
        specialOwner.GetComponent<MyPlayer_PVE>().stats.defense /= 3;
        specialOwner.GetComponent<MyPlayer_PVE>().stats.technique /= 3;
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        specialOwner.GetComponent<MyPlayer>().stats.shoot *= 3;
        specialOwner.GetComponent<MyPlayer>().stats.defense *= 3;
        specialOwner.GetComponent<MyPlayer>().stats.technique *= 3;

        while(!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);

        specialOwner.GetComponent<MyPlayer>().stats.shoot /= 3;
        specialOwner.GetComponent<MyPlayer>().stats.defense /= 3;
        specialOwner.GetComponent<MyPlayer>().stats.technique /= 3;
    }
}
