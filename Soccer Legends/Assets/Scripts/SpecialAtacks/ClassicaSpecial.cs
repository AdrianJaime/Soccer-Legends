using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicaSpecial : SpecialAttack
{
    SpecialAttackInfo specialAttackInfo;

    public ClassicaSpecial() { }
    public ClassicaSpecial(SpecialAttackInfo _specialAttackInfo) { specialAttackInfo = _specialAttackInfo; }

    public override bool canUseSpecial(PVE_Manager mg, GameObject specialOwner, float energy)
    {
        return energy >= specialAttackInfo.requiredEnergy && mg.state == PVE_Manager.fightState.FIGHT;
    }

    public override bool canUseSpecial(Manager mg, GameObject specialOwner, float energy)
    {
        return energy >= specialAttackInfo.requiredEnergy && mg.state == Manager.fightState.FIGHT;
    }

    public override IEnumerator callSpecial(PVE_Manager mg, GameObject specialOwner, GameObject rival)
    {
        List<MyPlayer_PVE> rivalsList = new List<MyPlayer_PVE>(rival.transform.parent.GetComponentsInChildren<MyPlayer_PVE>(true));
        List<KeyValuePair<MyPlayer_PVE, int>> musicList = new List<KeyValuePair<MyPlayer_PVE, int>>();
        for(int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().characterBasic.basicInfo
                .school == School.MUSIC) musicList.Add(new KeyValuePair<MyPlayer_PVE, int>
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>(), 3));
        }
        while (musicList.Count > 0)
        {
            while (musicList.Find(x => x.Key.fightDir != null).Key == null ||
            rivalsList.Find(x => x.fightDir != null) == null) yield return new WaitForSeconds(Time.deltaTime);
            KeyValuePair<MyPlayer_PVE, int> musicPlayer = musicList.Find(x => x.Key.fightDir != null);
            mg.statsUpdate(!musicPlayer.Key.transform.parent.GetComponent<IA_manager>().playerTeam,
                0, musicPlayer.Key.stats.technique * 30 / 100, musicPlayer.Key.stats.defense * 30 / 100);
            KeyValuePair<MyPlayer_PVE, int> newMusicPlayer = 
                new KeyValuePair<MyPlayer_PVE, int>(musicPlayer.Key, musicPlayer.Value - 1);
            musicList.Remove(musicPlayer);
            if (newMusicPlayer.Value > 0) musicList.Add(newMusicPlayer);
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        List<MyPlayer> rivalsList = new List<MyPlayer>(rival.transform.parent.GetComponentsInChildren<MyPlayer>(true));
        List<KeyValuePair<MyPlayer, int>> musicList = new List<KeyValuePair<MyPlayer, int>>();
        for(int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().characterBasic.basicInfo
                .school == School.MUSIC) musicList.Add(new KeyValuePair<MyPlayer, int>
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>(), 3));
        }
        while (musicList.Count > 0)
        {
            while (musicList.Find(x => x.Key.fightDir != null).Key == null ||
            rivalsList.Find(x => x.fightDir != null) == null) yield return new WaitForSeconds(Time.deltaTime);
            KeyValuePair<MyPlayer, int> musicPlayer = musicList.Find(x => x.Key.fightDir != null);
            mg.statsUpdate(musicPlayer.Key.photonView.ViewID, 0, musicPlayer.Key.stats.technique * 30 / 100, 
                musicPlayer.Key.stats.defense * 30 / 100);
            KeyValuePair<MyPlayer, int> newMusicPlayer = 
                new KeyValuePair<MyPlayer, int>(musicPlayer.Key, musicPlayer.Value - 1);
            musicList.Remove(musicPlayer);
            if (newMusicPlayer.Value > 0) musicList.Add(newMusicPlayer);
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
