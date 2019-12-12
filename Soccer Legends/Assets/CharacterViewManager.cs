using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterViewManager : MonoBehaviour
{
    private CharacterInfo actualCharacter;

    public Text pivotStat, defenseStat, technicalStat, nameCharacter, levelCharacter;
    public Image imageCharacter;


    // Start is called before the first frame update
    void Start()
    {
        actualCharacter = StaticInfo.characterToAcces; UpdateInterface();
    }


    public void UpdateInterface()
    {
        if (actualCharacter!=null)
        {
            pivotStat.text = actualCharacter.stats.shot.ToString();
            defenseStat.text = actualCharacter.stats.defense.ToString();
            technicalStat.text = actualCharacter.stats.technique.ToString();
            nameCharacter.text = actualCharacter.nameCharacter;
            levelCharacter.text = actualCharacter.level.ToString();
            imageCharacter.sprite = actualCharacter.artwork;
        }

    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(StaticInfo.previousScene);
    }


}
