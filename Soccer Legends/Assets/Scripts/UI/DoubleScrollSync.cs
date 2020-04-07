using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleScrollSync : MonoBehaviour
{
    public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap smallBannerScroll, bigBannerScroll;


    /// <summary>
    /// Cambia los paneles sin que hayan problemas de bucles
    /// </summary>
    public void ChangePanelSync()
    {

        if (bigBannerScroll.CurrentPanel != bigBannerScroll.TargetPanel)
        {
            smallBannerScroll.setCanInvokeOnPanelSelected(false);
            smallBannerScroll.GoToPanel(bigBannerScroll.TargetPanel);
        }
        else if (smallBannerScroll.CurrentPanel != smallBannerScroll.TargetPanel)
        {
            bigBannerScroll.setCanInvokeOnPanelSelected(false);
            bigBannerScroll.GoToPanel(smallBannerScroll.TargetPanel);
        }
        bigBannerScroll.setCanInvokeOnPanelSelected(true);
        smallBannerScroll.setCanInvokeOnPanelSelected(true);

    }

    /// <summary>
    /// Genera de 0 la logica 
    /// </summary>
    public void SetUpPannels()
    {
        smallBannerScroll.SetUp();
        bigBannerScroll.SetUp();
    }
}
