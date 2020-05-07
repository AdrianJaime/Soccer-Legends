using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapricornioSpecial : SpecialAttack
{
    SpecialAttackInfo specialAttackInfo;

    public CapricornioSpecial() { }
    public CapricornioSpecial(SpecialAttackInfo _specialAttackInfo) { specialAttackInfo = _specialAttackInfo; }

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
        List<MyPlayer_PVE> rivalsList = new List<MyPlayer_PVE>(rival.transform.parent.GetComponentsInChildren<MyPlayer_PVE>(true));
        int counter = 0;
        while (counter < 3)
        {
            while (specialOwner.GetComponent<MyPlayer_PVE>().fightDir == null ||
            rivalsList.Find(x=> x.fightDir != null) == null) yield return new WaitForSeconds(Time.deltaTime);
            counter++;
            mg.statsUpdate(!specialOwner.transform.parent.GetComponent<IA_manager>().playerTeam,
                0, specialOwner.GetComponent<MyPlayer_PVE>().stats.technique * 50 / 100, 0);
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        List<MyPlayer> rivalsList = new List<MyPlayer>(rival.transform.parent.GetComponentsInChildren<MyPlayer>(true));
        int counter = 0;
        while (counter < 3)
        {
            while (specialOwner.GetComponent<MyPlayer>().fightDir == null ||
            rivalsList.Find(x => x.fightDir != null) == null) yield return new WaitForSeconds(Time.deltaTime);
            counter++;
            mg.statsUpdate(specialOwner.GetComponent<MyPlayer>().photonView.ViewID,
                0, specialOwner.GetComponent<MyPlayer>().stats.technique * 50 / 100, 0);
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
