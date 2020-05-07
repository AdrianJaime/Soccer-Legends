using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VainillaSpecial : SpecialAttack
{
    SpecialAttackInfo specialAttackInfo;

    public VainillaSpecial() { }
    public VainillaSpecial(SpecialAttackInfo _specialAttackInfo) { specialAttackInfo = _specialAttackInfo; }

    public override bool canUseSpecial(PVE_Manager mg, GameObject specialOwner, float energy)
    {
        return energy >= specialAttackInfo.requiredEnergy && mg.state == PVE_Manager.fightState.SHOOT &&
            specialOwner.GetComponent<MyPlayer_PVE>().ball != null;
    }

    public override bool canUseSpecial(Manager mg, GameObject specialOwner, float energy)
    {
        return energy >= specialAttackInfo.requiredEnergy && mg.state == Manager.fightState.SHOOT &&
            specialOwner.GetComponent<MyPlayer>().ball != null;
    }

    public override IEnumerator callSpecial(PVE_Manager mg, GameObject specialOwner, GameObject rival)
    {
        List<MyPlayer_PVE> rivalsList = new List<MyPlayer_PVE>(rival.transform.parent.GetComponentsInChildren<MyPlayer_PVE>(true));
        List<MyPlayer_PVE> flavorList = new List<MyPlayer_PVE>();
        for (int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().characterBasic.basicInfo
                .school == School.FLAVOR) flavorList.Add(specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>());
        }
        while (mg.GameStarted)
        {
            while (flavorList.Find(x => x.fightDir != null) == null ||
            rivalsList.Find(x => x.fightDir != null) == null) yield return new WaitForSeconds(Time.deltaTime);
            MyPlayer_PVE flavorPlayer = flavorList.Find(x => x.fightDir != null);
            mg.statsUpdate(!flavorPlayer.transform.parent.GetComponent<IA_manager>().playerTeam,
                flavorPlayer.stats.shoot * 10 / 100, 0, 0);
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        List<MyPlayer> rivalsList = new List<MyPlayer>(rival.transform.parent.GetComponentsInChildren<MyPlayer>(true));
        List<MyPlayer> flavorList = new List<MyPlayer>();
        for (int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().characterBasic.basicInfo
                .school == School.FLAVOR) flavorList.Add(specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>());
        }
        while (mg.GameStarted)
        {
            while (flavorList.Find(x => x.fightDir != null) == null ||
            rivalsList.Find(x => x.fightDir != null) == null) yield return new WaitForSeconds(Time.deltaTime);
            MyPlayer flavorPlayer = flavorList.Find(x => x.fightDir != null);
            mg.statsUpdate(flavorPlayer.photonView.ViewID, flavorPlayer.stats.shoot * 10 / 100, 0, 0);
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
