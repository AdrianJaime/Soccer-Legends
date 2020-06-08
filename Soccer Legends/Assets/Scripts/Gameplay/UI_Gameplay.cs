using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UI_Gameplay : MonoBehaviour
{
    public void OnAnimationEventEnded(string anim)
    {
        GetComponent<Image>().enabled = false;
        if (GameObject.Find("Manager").GetComponent<PVE_Manager>() != null)
            GameObject.Find("Manager").GetComponent<PVE_Manager>().fightResult(anim);
        else if (GameObject.Find("Manager").GetComponent<Manager>() != null && PhotonNetwork.IsMasterClient)
            GameObject.Find("Manager").GetComponent<Manager>().fightResult(anim);
    }

    public void specialSpriteSwap()
    {
        transform.GetChild(1).GetComponent<Image>().sprite = transform.GetChild(2).GetComponent<Image>().sprite;
    }
}
