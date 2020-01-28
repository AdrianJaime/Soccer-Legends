using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageTournament : MonoBehaviour
{
    public Text description, nameStage;
    public Image artStage,artReward, clearPanel;
    public StageInfo basicInfo;

    public bool clear = true;
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
            Destroy(clearPanel); 


        description.text = basicInfo.description;
        nameStage.text = basicInfo.nameStage;
        artStage.sprite = basicInfo.artworkStage;
        artReward.sprite = basicInfo.imageReward;
    }
}
