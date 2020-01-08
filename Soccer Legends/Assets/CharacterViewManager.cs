using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterViewManager : MonoBehaviour
{
    private CharacterBasic actualCharacter;

    public Text pivotStat, defenseStat, technicalStat, nameCharacter, levelCharacter;
    public Image imageCharacter;


    // Start is called before the first frame update
    void Start()
    {
        actualCharacter = StaticInfo.characterToAcces;
        UpdateInterface();
    }


    public void UpdateInterface()
    {
        if (actualCharacter.basicInfo != null)
        {
            pivotStat.text = actualCharacter.stats.atq.ToString();
            defenseStat.text = actualCharacter.stats.def.ToString();
            technicalStat.text = actualCharacter.stats.teq.ToString();
            nameCharacter.text = actualCharacter.basicInfo.nameCharacter;
            levelCharacter.text = actualCharacter.level.ToString();
            imageCharacter.sprite = actualCharacter.basicInfo.artwork;
        }

    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(StaticInfo.previousScene);
    }


}
