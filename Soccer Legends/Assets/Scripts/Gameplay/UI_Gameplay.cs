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
        if (anim == "SpecialAnim") Destroy(GameObject.Find("Manager").GetComponent<PVE_Manager>() != null ?
            GameObject.Find("Manager").GetComponent<PVE_Manager>().lastSpecialClip :
            GameObject.Find("Manager").GetComponent<Manager>().lastSpecialClip);
    }

    public void specialSpriteSwap()
    {
        transform.GetChild(1).GetComponent<Image>().sprite = transform.GetChild(2).GetComponent<Image>().sprite;
    }

    public void changeGoalNum(int _ref)
    {
        TMPro.TextMeshProUGUI score;
        if(_ref == 0)
        {
            score = transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>();
            if (GameObject.Find("Manager").GetComponent<PVE_Manager>() != null)
                transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText(PlayerPrefs.GetString("username"));
            else transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText(PhotonNetwork.LocalPlayer.NickName);
        }
        else
        {
            score = transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>();
            if (GameObject.Find("Manager").GetComponent<PVE_Manager>() != null)
                transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText(StaticInfo.tournamentTeam.teamName);
            else transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText(PhotonNetwork.PlayerListOthers[0].NickName);
        }
        
        int scoreNum = int.Parse(score.text.Substring(1));
        scoreNum++;
        score.SetText("0" + scoreNum.ToString());

        StartCoroutine(endGoalAnim(scoreNum != 3 ? 1.25f : 0.0f));
    }

    IEnumerator endGoalAnim(float time)
    {
        yield return new WaitForSeconds(time);

        GetComponent<Animator>().SetTrigger("Reset");
    }
}
