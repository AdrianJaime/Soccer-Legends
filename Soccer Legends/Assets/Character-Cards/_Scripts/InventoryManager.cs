using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public InventoryData inventoryInfo;
    public Transform locationCharacterSpawn;
    public GameObject inventoryPrefabUnit;

    public List<GameObject> listOfSlots = new List<GameObject>();


    private void Start()
    {
        CreateInventory();
    }


    public void CreateInventory()
    {
        int counterIdentifier = 0;

        foreach (CharacterInfo characterUnit in inventoryInfo.listOfCharacters)
        {
            GameObject actualCard = Instantiate(inventoryPrefabUnit, locationCharacterSpawn);
            actualCard.GetComponent<InventoryCardRender>().characterInfo = characterUnit;
            actualCard.GetComponent<InventoryCardRender>().identifierSlot = counterIdentifier;
            listOfSlots.Add(actualCard);
            counterIdentifier++;
        }
    }
    public void SortInventoryCaharactersByAtribute(string aux)
    {
        int counterIdentifier = 0;

        inventoryInfo.SortByAtribute(aux);
        foreach(GameObject slot in listOfSlots)
        {
            
            slot.GetComponent<InventoryCardRender>().characterInfo=inventoryInfo.listOfCharacters[counterIdentifier];
            slot.GetComponent<InventoryCardRender>().UpdateSlotRender();
            counterIdentifier++;

        }
    }




}
