using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainStory_ChapterDisplay : MonoBehaviour
{
    public Image chapterArt;

    [SerializeField] DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap chaptersSlider;


    public void UpdateUI()
    {
        TeamTournamentSlot aux = chaptersSlider.Panels[chaptersSlider.TargetPanel/*CurrentPanel*/].GetComponentInChildren<TeamTournamentSlot>();
        if (aux!=null&&aux.canEnter)
        {
            Debug.Log("encuentro info");
            chapterArt.sprite = aux.basicInfo.teamArtwork;
        }
    }
}
