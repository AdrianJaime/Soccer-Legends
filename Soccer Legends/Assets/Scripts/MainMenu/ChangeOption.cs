using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOption : MonoBehaviour
{
    [SerializeField] List<GameObject> options;
    [SerializeField] List<Button> buttons;
    // Start is called before the first frame update
    void Start()
    {
        SetOption(0);
    }

    public void SetOption(int _index)
    {
        Button predictedButton;
        if (_index<buttons.Count&& _index >= 0)
        {
            predictedButton = buttons[_index];
            int auxIndex = 0;
            foreach (Button button in buttons)
            {
                if (button == predictedButton)
                {
                    options[auxIndex].SetActive(true);
                    buttons[auxIndex].interactable = false;
                }
                else
                {
                    options[auxIndex].SetActive(false);
                    buttons[auxIndex].interactable = true;
                }
                auxIndex++;
            }
        }

        
    }

}
