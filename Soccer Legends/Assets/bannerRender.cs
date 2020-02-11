using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bannerRender : MonoBehaviour
{
    public Image imageBanner;
    public BannerBASE banner;

    public bool isSmallBanner;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (isSmallBanner)
            imageBanner.sprite = banner.basicInfo.spriteSmallBanner;
        else
            imageBanner.sprite = banner.basicInfo.spriteBigBanner;
    }
}
