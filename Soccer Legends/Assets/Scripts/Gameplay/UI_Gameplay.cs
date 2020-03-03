using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Gameplay : MonoBehaviour
{
   public void OnAnimationEventEnded(string anim)
    {
        if (GameObject.Find("Manager").GetComponent<PVE_Manager>() != null)
            GameObject.Find("Manager").GetComponent<PVE_Manager>().fightResult(anim);
        else if (GameObject.Find("Manager").GetComponent<Manager>() != null &&
            GameObject.Find("Manager").GetComponent<Manager>().photonView.Owner.IsMasterClient)
            GameObject.Find("Manager").GetComponent<Manager>().fightResult(anim);
    }
}
