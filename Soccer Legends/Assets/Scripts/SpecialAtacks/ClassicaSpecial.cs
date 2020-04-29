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
        while (specialOwner.GetComponent<MyPlayer_PVE>().fightDir == null ||
            rival.GetComponent<MyPlayer_PVE>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        for(int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>()
                .characterBasic.basicInfo.school == School.MUSIC)
            {
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.technique +=
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.technique * 30) / 100;
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.defense +=
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.defense * 30) / 100;
            }
            
        }

        //Esperamos a que se salga del combate
        while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);

        int counter = 1;
        while (counter < 3)
        {
            counter++;

            //Esperamos a que se entre en un nuevo combate
            while (mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
            //Esperamos a que se salga del combate
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
            //Se repite el ciclo hasta que al llegar aqui el contador sea 3
        }

        for (int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>()
                .characterBasic.basicInfo.school == School.MUSIC)
            {
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.technique +=
                    -(specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.technique * 30) / 100;
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.defense +=
                    -(specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer_PVE>().stats.defense * 30) / 100;
            }

        }

        yield break;
    }

    public override IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival)
    {
        while (specialOwner.GetComponent<MyPlayer>().fightDir == null ||
            rival.GetComponent<MyPlayer>().fightDir == null) yield return new WaitForSeconds(Time.deltaTime);

        for (int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>()
                .characterBasic.basicInfo.school == School.MUSIC)
            {
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.technique +=
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.technique * 30) / 100;
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.defense +=
                    (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.defense * 30) / 100;
            }

        }

        //Esperamos a que se salga del combate
        while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);

        int counter = 1;
        while (counter < 3)
        {
            counter++;

            //Esperamos a que se entre en un nuevo combate
            while (mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
            //Esperamos a que se salga del combate
            while (!mg.GameOn) yield return new WaitForSeconds(Time.deltaTime);
            //Se repite el ciclo hasta que al llegar aqui el contador sea 3
        }

        for (int i = 0; i < specialOwner.transform.parent.childCount; i++)
        {
            if (specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>()
                .characterBasic.basicInfo.school == School.MUSIC)
            {
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.technique +=
                    -(specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.technique * 30) / 100;
                specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.defense +=
                    -(specialOwner.transform.parent.GetChild(i).GetComponent<MyPlayer>().stats.defense * 30) / 100;
            }

        }

        yield break;
    }
}
