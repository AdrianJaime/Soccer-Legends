using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSelector_CardSelection : MonoBehaviour
{
    private bool first = true;
    private bool selected = false;
    private PlayerSelector_PlayerManager manager;
    public CharacterBasic characterBasic;

    private void Start()
    {
        manager = FindObjectOfType<PlayerSelector_PlayerManager>();
    }
    public void OnButtonClick()
    {
        if (first&&!selected)//Si no está seleccionado el pj
        {
            if (characterBasic.basicInfo != null && manager.AddPlayerSelected(characterBasic)==true)
            {
                gameObject.GetComponent<PlayerSelector_CardRender>().SelectedRender();
                selected = true;
                first = false;
            }
        }
        else
        {
            manager.ErasePlayerSelected(characterBasic);
            gameObject.GetComponent<PlayerSelector_CardRender>().DiselectedRender();
            selected = false;
            first = true;
        }
    }
}
