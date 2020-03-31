using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bannerRender : MonoBehaviour
{
    public Image imageBanner;
    public BannerInfo basicInfo;
    public bool isSmallBanner;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (isSmallBanner)
            imageBanner.sprite = basicInfo.spriteSmallBanner;
        else
            imageBanner.sprite = basicInfo.spriteBigBanner;
    }
}
