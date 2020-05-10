using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Este Script se encarga de organizar el apartado visual dentro 
/// de una stage. Tambien se encarga de comprobar si la reward se ha 
/// conseguido o no a través de base de datos
/// </summary>
public class StageTournament : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image stageBASE,stageBUTTON, stageBoss, artReward, clearPanel, difficultSprite;
    [SerializeField] SpriteConpendiumSO stagesPanels, stagesButtons, difficultStagesImages;
    public StageInfo basicInfo;//Información básica sobre ésta stage, variables que no varían
    [SerializeField] RewardLogic reward;

    public bool clear = true; 
    bool ownedReward = false;

    // Start is called before the first frame update
    void Start()
    {
        //leer de base de datos si se ha completado
        //
        UpdateUI();
    }

    void UpdateUI()
    {
        if (clear)
        {
            if (ownedReward)
                reward.SetReclaimed();
            else
                reward.UnlockReward();

            Destroy(clearPanel);
        }
        //Base Sprite
        if (basicInfo.isBoss) {
            stageBoss.enabled = (true);
            stageBASE.sprite = stagesPanels.sprites[2];
            stageBUTTON.sprite = stagesButtons.sprites[2];
        }
        else
        {
            stageBoss.enabled=(false);
            stageBASE.sprite = stagesPanels.sprites[1];
            stageBUTTON.sprite = stagesButtons.sprites[1];
        }
        //sprites
        description.text = basicInfo.description;
        stageBoss.sprite = basicInfo.stageArtwork;
        artReward.sprite = basicInfo.imageReward;

        if (PlayerPrefs.GetInt("stages_difficult") != -1)
        {
            difficultSprite.sprite = difficultStagesImages.sprites[PlayerPrefs.GetInt("stages_difficult")];
        }
    }

    public void selectStage()
    {
        StaticInfo.tournamentTeam.selectedStage = transform.GetSiblingIndex();
    }

    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }
}
