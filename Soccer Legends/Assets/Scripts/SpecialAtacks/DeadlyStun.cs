using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyStun : SpecialAttack
{
    SpecialAttackInfo specialAttackInfo;

    public DeadlyStun() { }
    public DeadlyStun(SpecialAttackInfo _specialAttackInfo) { specialAttackInfo = _specialAttackInfo; }

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
            rival.GetComponent<MyPlayer_PVE>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        mg.statsUpdate(!rival.transform.parent.GetComponent<IA_manager>().playerTeam,
            0, 0, -(rival.GetComponent<MyPlayer_PVE>().stats.defense * 20) / 100);

        while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);

        if (!rival.GetComponent<MyPlayer_PVE>().stunned) yield break;

        string score0 = mg.scoreBoard.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
        string score1 = mg.scoreBoard.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;

        while (rival.GetComponent<MyPlayer_PVE>().stunned)
        {
            if (score0 != mg.scoreBoard.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text ||
                score1 != mg.scoreBoard.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text)
                yield break;
            else
                yield return new WaitForSeconds(Time.deltaTime);
        }

        while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);

        rival.GetComponent<MyPlayer_PVE>().Lose();

    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        while (specialOwner.GetComponent<MyPlayer>().fightDir == null ||
            rival.GetComponent<MyPlayer>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        mg.statsUpdate(rival.GetComponent<MyPlayer>().photonView.ViewID, 0, 0, 
            -(rival.GetComponent<MyPlayer>().stats.defense * 20) / 100);

        while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);

        if (!rival.GetComponent<MyPlayer>().stunned) yield break;

        string score0 = mg.scoreBoard.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
        string score1 = mg.scoreBoard.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;

        while (rival.GetComponent<MyPlayer>().stunned)
        {
            if (score0 != mg.scoreBoard.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text ||
                score1 != mg.scoreBoard.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text)
                yield break;
            else
                yield return new WaitForSeconds(Time.deltaTime);
        }

        while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);

        rival.GetComponent<MyPlayer>().Lose();
    }
}
