using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class specialAnimManager : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject.Find("UI GAMEPLAY").GetComponent<UI_Gameplay>().OnAnimationEventEnded("SpecialAnim");
    }
}
