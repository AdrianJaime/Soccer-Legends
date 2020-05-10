using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainStory_ChapterDisplay : MonoBehaviour
{
    [SerializeField] Image chapterArt, chapterArtDifficult, chapterSelectorDifficult, difficultOnlyImage;
    [SerializeField] ColorsScriptableObject difficultColorLevels;

    [SerializeField] DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap chaptersSlider;
    [SerializeField] SpriteConpendiumSO difficultOnly;
    private void Start()
    {
        ChangeDifficult(PlayerPrefs.GetInt("stages_difficult"));

    }

    public void UpdateUI()
    {
        TeamTournamentSlot aux = chaptersSlider.Panels[chaptersSlider.TargetPanel/*CurrentPanel*/].GetComponentInChildren<TeamTournamentSlot>();
        if (aux!=null&&aux.canEnter)
            chapterArt.sprite = aux.basicInfo.teamArtwork;
        
    }

    public void ChangeDifficult(int _levelOfDifficult)
    {
        PlayerPrefs.SetInt("stages_difficult", _levelOfDifficult);

        if(_levelOfDifficult< difficultColorLevels.colors.Count)
        {
            chapterArtDifficult.color = difficultColorLevels.colors[_levelOfDifficult];
            chapterSelectorDifficult.color = difficultColorLevels.colors[_levelOfDifficult];
            difficultOnlyImage.sprite = difficultOnly.sprites[_levelOfDifficult];
        }
    }
}
