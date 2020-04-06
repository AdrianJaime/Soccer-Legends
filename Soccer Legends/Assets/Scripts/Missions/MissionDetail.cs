using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionDetail : MonoBehaviour
{

    [SerializeField] Text descriptionText, progressText;
    [SerializeField] Slider progressBar;
    [SerializeField] Image[] rewardImage; //deberia ser siempre 3 en el editor

    string description;
    int maxProgress, actualProgress;
    List<MissionObject.OBJECT_DATA> rewards;


    public void SetUpVariables(string _description, int _actualProgress, int _maxProgress, List<MissionObject.OBJECT_DATA> _rewards)
    {
        description = _description; actualProgress = _actualProgress; maxProgress = _maxProgress; rewards = _rewards;

        UpdateRender();

        gameObject.transform.parent.gameObject.SetActive(true);
    }
    void UpdateRender()
    {
        //Textos
        descriptionText.text = description;
        progressText.text = actualProgress.ToString() + "/" + maxProgress.ToString();
        //imagenes de reward
        for (int i = 0; i < 3; i++)
        {
            //comprobamos siempre que la i sea menor que la longitud de la lista para no accceder a un valor fuera de rango y pete.
            //tambien hago una comprobacion en el array de imagenes, por si acaso, aunque siempre va a ser constante 3.
            if (i < rewards.Count && i < rewardImage.Length)
            {
                rewardImage[i].color = Color.white;
                rewardImage[i].sprite = rewards[i].baseInfo.image;
                rewardImage[i].gameObject.SetActive(true);
            }
        }
        //barra de progreso
        progressBar.maxValue = maxProgress;
        progressBar.value = actualProgress;

    }

    public void ClosePopUp()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
