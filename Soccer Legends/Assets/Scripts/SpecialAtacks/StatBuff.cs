using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuff : SpecialAtack
{
    public override bool canUseSpecial(PVE_Manager mg, GameObject specialOwner)
    {
        return mg.energy >= energyRequired;
    }

    public override bool canUseSpecial(Manager mg, GameObject specialOwner)
    {
        return mg.energy >= energyRequired;
    }

    public override IEnumerator callSpecial(PVE_Manager mg, GameObject specialOwner)
    {
        specialOwner.GetComponent<MyPlayer_PVE>().stats.shoot *= 3;
        specialOwner.GetComponent<MyPlayer_PVE>().stats.defense *= 3;
        specialOwner.GetComponent<MyPlayer_PVE>().stats.technique *= 3;

        while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);

        specialOwner.GetComponent<MyPlayer_PVE>().stats.shoot /= 3;
        specialOwner.GetComponent<MyPlayer_PVE>().stats.defense /= 3;
        specialOwner.GetComponent<MyPlayer_PVE>().stats.technique /= 3;
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner)
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
