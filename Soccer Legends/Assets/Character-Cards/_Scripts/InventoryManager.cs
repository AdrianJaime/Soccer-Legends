using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public InventoryData inventoryInfo;
    public Transform locationCharacterSpawn;
    public GameObject inventoryPrefabUnit;

    int counterIdentifier = 0;
    private void Start()
    {
        CreateInventory();
    }


    public void CreateInventory()
    {
        foreach(CharacterInfo characterUnit in inventoryInfo.listOfCharacters)
        {
            GameObject actualCard=Instantiate(inventoryPrefabUnit, locationCharacterSpawn);
            actualCard.GetComponent<InventoryCardRender>().characterInfo = characterUnit;
            actualCard.GetComponent<InventoryCardRender>().identifierSlot = counterIdentifier;
            counterIdentifier++;
        }
    }


}
