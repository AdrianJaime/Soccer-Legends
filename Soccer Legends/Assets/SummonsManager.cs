using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonsManager : MonoBehaviour
{
    bool canSummon=true;
    [SerializeField] float blockSummonTime = 2;
    public List<BannerInfo> banners;
    public BannerInfo actualBanner;

    public GameObject prefabSmallBanner, prefabBigBanner, paginationTogglePrefab;
    public Transform transformSmallBanner, transformBigBanner,paginationToggleLocation;
    public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap smallBannerScroll,bigBannerScroll;
    public float endSpacing = 0.05f;

    private void Start()
    {
        SetUpSummons();
    }

    /// <summary>
    /// Coloca los banners y su información
    /// </summary>
    private void SetUpSummons()
    {

        // LEER BANNERS DE BASE DE DATOS Y ACTUALIZAR LISTA BANNERS.
        foreach (BannerInfo banner in banners)
        {
            if (banner != null)
            {
                GameObject actualBanner = Instantiate(prefabSmallBanner, transformSmallBanner);
                actualBanner.GetComponent<bannerRender>().basicInfo = banner;

                GameObject actualBanner2 = Instantiate(prefabBigBanner, transformBigBanner);
                actualBanner2.GetComponent<bannerRender>().basicInfo = banner;

                Color aux = new Color(Random.Range(0,255),Random.Range(0,255),Random.Range(0,255));

                Instantiate(paginationTogglePrefab, paginationToggleLocation);

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

        actualBanner = banners[bigBannerScroll.CurrentPanel];
    }

    /// <summary>
    /// Cambia los paneles sin que hayan problemas de bucles
    /// </summary>
    public void ChangePanelSync()
    {
        StartCoroutine(BlockSummon(blockSummonTime));

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

        actualBanner = banners[bigBannerScroll.TargetPanel];
    }

    /// <summary>
    /// Calcula la rareza del personaje que se obtendrá
    /// </summary>
    public void CalculateRaritySummon()
    {
        if (canSummon)
        {
            StartCoroutine(BlockInteractions());

            float rand = Random.Range(0, actualBanner.goldChance + actualBanner.silverChance + actualBanner.bronzeChance);
            if (rand < actualBanner.goldChance)
                CalculateByRarity(Rarity.GOLD);
            else if (rand < actualBanner.silverChance)
                CalculateByRarity(Rarity.SILVER);
            else if (rand < actualBanner.bronzeChance)
                CalculateByRarity(Rarity.BRONZE);
        }
    }

    /// <summary>
    /// Con la rareza obtiene un personaje de dicha.
    /// </summary>
    CharacterInfo CalculateByRarity(Rarity rarity)
    {
        float aux = 0;
        List<BannerInfo.SpecialChance> list = null;
        switch (rarity)
        {
            case Rarity.GOLD:
                list = actualBanner.GoldPlayers;
                break;
            case Rarity.SILVER:
                list = actualBanner.SilverPlayers;
                break;
            case Rarity.BRONZE:
                list = actualBanner.BronzePlayers;
                break;
            default:
                return null;
        }
        foreach (BannerInfo.SpecialChance character in list)
        {
            aux += character.customChance;
        }
        float rand = Random.Range(0.0f, aux);
        //banner.basicInfo.GoldPlayers.Find(x => rand<x.customChance&& x.customChance == rand);
        float counter = 0;
        foreach (BannerInfo.SpecialChance character in list)
        {
            counter += character.customChance;
            if (rand <= counter)
            {
                return character.character;
            }
        }

        //Error 
        return null;
    }

    /// <summary>
    /// Esta funcion bloque el summon cuando hay una interacción
    /// </summary>
    private IEnumerator BlockSummon(float waitTime)
    {
        canSummon = false;
        yield return new WaitForSeconds(waitTime);
        canSummon = true;
    }

    /// <summary>
    /// Esta funcion bloque las interacciones cuando hay un summon
    /// </summary>
    private IEnumerator BlockInteractions()
    {

        smallBannerScroll.enabled = false;
        bigBannerScroll.enabled = false;
        bigBannerScroll.previousButton.interactable = false;
        bigBannerScroll.nextButton.interactable = false;
        yield return new WaitForSeconds(5);
        smallBannerScroll.enabled = true;
        bigBannerScroll.enabled = true;
        bigBannerScroll.previousButton.interactable = true;
        bigBannerScroll.nextButton.interactable = true;

    }

}
