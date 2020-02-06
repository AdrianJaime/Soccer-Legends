using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
