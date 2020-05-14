using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MissionObject : MonoBehaviour
{
    [System.Serializable]
    public struct OBJECT_DATA
    {
        public int number;
        public string id;
    }

    [SerializeField] Text titleText, descriptionText, progressText,claimButtonText;
    [SerializeField] Image []rewardImage;
    [SerializeField] Slider progressBar;
    [SerializeField] Button claimButton;
    [SerializeField] Image sello;

    MissionDetail popUpMission;


    string title, description,ID; //el ID es por si reclamas una recompensa, saber donde acceder en la BD para modificar variables
    int maxProgress, actualProgress;
   // List<OBJECT_DATA> rewards;
    bool claim;

    public void SetUpVariables(string _id, bool _claim, string _title, string _description, int _actualProgress, int _maxProgress, /*List<OBJECT_DATA> _rewards,*/ MissionDetail _popUpMissionScript)
    {
        ID = _id; claim = _claim; title = _title; description = _description; actualProgress = _actualProgress; maxProgress = _maxProgress; popUpMission = _popUpMissionScript;
        //rewards = _rewards;

        UpdateRender();
    }

    void UpdateRender()
    {
        //Textos
        titleText.text = title;
        descriptionText.text = description;
        progressText.text = actualProgress.ToString() + "/" + maxProgress.ToString();
        //imagenes de reward
        for(int i=0; i < 3; i++)
        {
            //comprobamos siempre que la i sea menor que la longitud de la lista para no accceder a un valor fuera de rango y pete.
            //tambien hago una comprobacion en el array de imagenes, por si acaso, aunque siempre va a ser constante 3.
            //if (i < rewards.Count&&i<rewardImage.Length)
            //{
            //    rewardImage[i].sprite = rewards[i].baseInfo.image;
            //    rewardImage[i].gameObject.SetActive(true);
            //}
        }
        //barra de progreso
        progressBar.maxValue = maxProgress;
        progressBar.value = actualProgress;
        //boton de reclamar
        if (!claim)
        {
            if (Mathf.Floor(actualProgress / maxProgress) == 1)
            {
                claimButton.interactable = true;
                claimButtonText.text = "Claim";
            }
            else
            {
                claimButton.interactable = false;
                claimButtonText.text = "In progress";
            }

            sello.enabled = false;
        }
        
    }

    public void OpenPopUp()
    {
        popUpMission.SetUpVariables(description, actualProgress, maxProgress/*, rewards*/);
    }

}
