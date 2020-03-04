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
            pivotStat.text = actualCharacter.info.atk.ToString();
            defenseStat.text = actualCharacter.info.def.ToString();
            technicalStat.text = actualCharacter.info.teq.ToString();
            nameCharacter.text = actualCharacter.basicInfo.nameCharacter;
            levelCharacter.text = actualCharacter.info.level.ToString();
            imageCharacter.sprite = actualCharacter.basicInfo.artworkIcon;
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(StaticInfo.previousScene);
    }


}
