using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotConsum : MonoBehaviour
{
    public int quantity=0;
    int usedQuantity=0;

    public Text quantityText;
    public Image imageLocation;
    public Text usedQuantityText;
    public Button addButton;
    public Button substractButton;

    public ConsumBaseInfo consumBaseInfo;


    // Start is called before the first frame update
    void Start()
    {
        //Image Update
        imageLocation.sprite = consumBaseInfo.image;
    }

    //Updates with the info changed in the Inevntory when spawned 
    public void UpdateUI()
    {
        //Quantity Update
        if (quantity != 0)
        {
            quantityText.text = "x" + quantity.ToString();
            quantityText.gameObject.SetActive(true);

            if (addButton.IsInteractable()==false)
                addButton.interactable = true;

        }
        else
        {

            if (addButton.IsInteractable() == true)
                addButton.interactable = false;

            quantityText.gameObject.SetActive(false);

        }

        //UsedQuantity Update And Buttons Conditions
        if(usedQuantity != 0)
        {
            if (substractButton.IsInteractable() == false)
                substractButton.interactable = true;
        }
        else
        {
            if (substractButton.IsInteractable() == true)
                substractButton.interactable = false;
        }
        usedQuantityText.text = usedQuantity.ToString();

    }

    //function that regulates adding more objects used when using
    public void AddUsedQuantity()
    {
        if (quantity != 0)
        {
            quantity--;
            usedQuantity++;
        }

        UpdateUI();
    }    

    //function that regulates substracting more objects used when using
    public void SubstractUsedQuantity()
    {
        if (usedQuantity != 0)
        {
            quantity++;
            usedQuantity--;
        }
        UpdateUI();
    }
}
