using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonsManager : MonoBehaviour
{
    public List<BannerInfo> banners;
    public GameObject prefabSmallBanner, prefabBigBanner;
    public Transform transformSmallBanner, transformBigBanner;
    public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap smallBannerScroll,bigBannerScroll;
    public float endSpacing = 0.05f;

    private void Start()
    {
        SetUpSummons();
    }

    private void SetUpSummons()
    {
        foreach(BannerInfo banner in banners)
        {
            if (banner != null)
            {
                GameObject actualBanner = Instantiate(prefabSmallBanner, transformSmallBanner);
                actualBanner.GetComponent<BannerBASE>().basicInfo = banner;

                GameObject actualBanner2 = Instantiate(prefabBigBanner, transformBigBanner);
                actualBanner2.GetComponent<BannerBASE>().basicInfo = banner;

                Color aux = new Color(Random.Range(0,255),Random.Range(0,255),Random.Range(0,255));


            }

        }
        if(banners.Count>2)
        {
            smallBannerScroll.infinitelyScroll=true;
            smallBannerScroll.infiniteScrollingEndSpacing= endSpacing;
            bigBannerScroll.infinitelyScroll = true;
            bigBannerScroll.infiniteScrollingEndSpacing = endSpacing;
        }
   
        smallBannerScroll.SetUp();
        bigBannerScroll.SetUp();
    }


    public void PrintTest(bool auc)
    {

        if (auc)
            Debug.Log("Im CHANGING");
        else
            Debug.Log("ive CHANGED");
    }

    public void ChangePanelSync()
    {
        if(bigBannerScroll.CurrentPanel!= bigBannerScroll.TargetPanel)
        {
            smallBannerScroll.setCanInvokeOnPanelSelected(false);
            smallBannerScroll.GoToPanel(bigBannerScroll.TargetPanel);
        }
        else if (smallBannerScroll.CurrentPanel!= smallBannerScroll.TargetPanel)
        {
            bigBannerScroll.setCanInvokeOnPanelSelected( false);
            bigBannerScroll.GoToPanel(smallBannerScroll.TargetPanel);
        }
        bigBannerScroll.setCanInvokeOnPanelSelected(true);
        smallBannerScroll.setCanInvokeOnPanelSelected(true);
    }

}
