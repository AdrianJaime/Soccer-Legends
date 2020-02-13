using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOption : MonoBehaviour
{
    [SerializeField] GameObject soundOptions, graphicOptions, extraOptions;
    [SerializeField] Button soundButton, graphicButton, extraButton;
    // Start is called before the first frame update
    void Start()
    {
        soundOptions.SetActive(true);
        graphicOptions.SetActive(false);
        extraOptions.SetActive(false);
        soundButton.interactable = false;
        graphicButton.interactable = true;
        extraButton.interactable = true;

    }

    public void SetOption(string _name)
    {
        switch (_name)
        {
            case "sound":
            case "Sound":
                soundOptions.SetActive(true);
                graphicOptions.SetActive(false);
                extraOptions.SetActive(false);

                soundButton.interactable = false;
                graphicButton.interactable = true;
                extraButton.interactable = true;
                break;
            case "graphic":
            case "Graphic":
                soundOptions.SetActive(false);
                graphicOptions.SetActive(true);
                extraOptions.SetActive(false);

                soundButton.interactable = true;
                graphicButton.interactable = false;
                extraButton.interactable = true;
                break;
            case "extra":
            case "Extra":
                soundOptions.SetActive(false);
                graphicOptions.SetActive(false);
                extraOptions.SetActive(true);

                soundButton.interactable = true;
                graphicButton.interactable = true;
                extraButton.interactable = false;
                break;
        }
    }

}
