using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterViewManager : MonoBehaviour
{
    private CharacterBasic actualCharacter;

    [SerializeField] Text pivotStat, defenseStat, technicalStat, nameCharacter, levelCharacter;
    [SerializeField] Image imageCharacter, imageFillNameRarity;

    [SerializeField] Color[] colorsRarity;

    // Start is called before the first frame update
    void Start()
    {
        actualCharacter = StaticInfo.characterToAcces;
        UpdateInterface();
    }


    public void UpdateInterface()
    {
            pivotStat.text = actualCharacter.info.atk.ToString();
            defenseStat.text = actualCharacter.info.def.ToString();
            technicalStat.text = actualCharacter.info.teq.ToString();
            imageCharacter.sprite = actualCharacter.basicInfo.completeArtwork;
            nameCharacter.text = actualCharacter.basicInfo.nameCharacter;
            levelCharacter.text = "Lvl "+actualCharacter.info.level.ToString();

        imageFillNameRarity.color = colorsRarity[(int)StaticInfo.characterToAcces.basicInfo.rarity];
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(StaticInfo.previousScene);
    }


}
