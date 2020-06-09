using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class OST_MUSIC : MonoBehaviour
{

    public StudioEventEmitter ui_emt, gameplay_emt, gradas_emt, goal_emt;

    private void OnLevelWasLoaded(int level)
    {
        switch (SceneManager.GetSceneByBuildIndex(level).name)
        {
            case "PlayersSelectorScene":
                if (!ui_emt.IsPlaying()) ui_emt.Play();
                if (gameplay_emt.IsPlaying())
                {
                    gameplay_emt.Stop();
                    gradas_emt.EventInstance.stop( FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                break;
            case "Match-PVE_test":
                ui_emt.Stop();
                gameplay_emt.Play();
                gradas_emt.Play();
                break;
            default:
                break;
        }
    }
}
