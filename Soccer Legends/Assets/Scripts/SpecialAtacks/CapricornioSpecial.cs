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
        while (specialOwner.GetComponent<MyPlayer_PVE>().fightDir == null ||
            rival.GetComponent<MyPlayer_PVE>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        specialOwner.GetComponent<MyPlayer_PVE>().stats.technique +=
                    (specialOwner.GetComponent<MyPlayer_PVE>().stats.technique * 50) / 100;

        int counter = 0;
        while(counter < 3)
        {
            counter++;

            //Esperamos a que se salga del combate
            while(!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
            //Esperamos a que se entre en un nuevo combate
            while (mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
            //Esperamos a que elijan los jugadores
            while (specialOwner.GetComponent<MyPlayer_PVE>().fightDir == null ||
            rival.GetComponent<MyPlayer_PVE>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);
            //Se repite el ciclo hasta que al llegar aqui el contador sea 3
        }

        specialOwner.GetComponent<MyPlayer_PVE>().stats.technique +=
                    -(specialOwner.GetComponent<MyPlayer_PVE>().stats.technique * 50) / 100;

        yield break;
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        while (specialOwner.GetComponent<MyPlayer>().fightDir == null ||
            rival.GetComponent<MyPlayer>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        specialOwner.GetComponent<MyPlayer>().stats.technique +=
                    (specialOwner.GetComponent<MyPlayer>().stats.technique * 50) / 100;

        int counter = 0;
        while (counter < 3)
        {
            counter++;

            //Esperamos a que se salga del combate
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
            //Esperamos a que se entre en un nuevo combate
            while (mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
            //Esperamos a que elijan los jugadores
            while (specialOwner.GetComponent<MyPlayer>().fightDir == null ||
            rival.GetComponent<MyPlayer>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);
            //Se repite el ciclo hasta que al llegar aqui el contador sea 3
        }

        specialOwner.GetComponent<MyPlayer>().stats.technique +=
                    -(specialOwner.GetComponent<MyPlayer>().stats.technique * 50) / 100;

        yield break;
    }
}
