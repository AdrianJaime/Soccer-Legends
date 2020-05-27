using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuciferSpecial : SpecialAttack
{
    SpecialAttackInfo specialAttackInfo;

    public LuciferSpecial() { }
    public LuciferSpecial(SpecialAttackInfo _specialAttackInfo) { specialAttackInfo = _specialAttackInfo; }

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
        List<KeyValuePair<MyPlayer_PVE, int>> demonList = new List<KeyValuePair<MyPlayer_PVE, int>>();
        for (int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().characterBasic.basicInfo
                .school == School.DEMONS) demonList.Add(new KeyValuePair<MyPlayer_PVE, int>
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>(), 3));
        }
        while (demonList.Count > 0)
        {
            while (demonList.Find(x => x.Key.fightDir != null).Key == null ||
            rivalsList.Find(x => x.fightDir != null) == null) yield return new WaitForSeconds(Time.deltaTime);
            KeyValuePair<MyPlayer_PVE, int> demonPlayer = demonList.Find(x => x.Key.fightDir != null);
            mg.statsUpdate(!demonPlayer.Key.transform.parent.GetComponent<IA_manager>().playerTeam,
                demonPlayer.Key.stats.shoot * 30 / 100, demonPlayer.Key.stats.technique * 30 / 100, 0);
            KeyValuePair<MyPlayer_PVE, int> newDemonPlayer =
                new KeyValuePair<MyPlayer_PVE, int>(demonPlayer.Key, demonPlayer.Value - 1);
            demonList.Remove(demonPlayer);
            if (newDemonPlayer.Value > 0) demonList.Add(newDemonPlayer);
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        List<MyPlayer> rivalsList = new List<MyPlayer>(rival.transform.parent.GetComponentsInChildren<MyPlayer>(true));
        List<KeyValuePair<MyPlayer, int>> demonList = new List<KeyValuePair<MyPlayer, int>>();
        for (int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().characterBasic.basicInfo
                .school == School.DEMONS) demonList.Add(new KeyValuePair<MyPlayer, int>
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>(), 3));
        }
        while (demonList.Count > 0)
        {
            while (demonList.Find(x => x.Key.fightDir != null).Key == null ||
            rivalsList.Find(x => x.fightDir != null) == null) yield return new WaitForSeconds(Time.deltaTime);
            KeyValuePair<MyPlayer, int> demonPlayer = demonList.Find(x => x.Key.fightDir != null);
            mg.statsUpdate(demonPlayer.Key.photonView.ViewID, demonPlayer.Key.stats.shoot * 30 / 100, 
                demonPlayer.Key.stats.technique * 30 / 100, 0);
            KeyValuePair<MyPlayer, int> newDemonPlayer =
                new KeyValuePair<MyPlayer, int>(demonPlayer.Key, demonPlayer.Value - 1);
            demonList.Remove(demonPlayer);
            if (newDemonPlayer.Value > 0) demonList.Add(newDemonPlayer);
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
