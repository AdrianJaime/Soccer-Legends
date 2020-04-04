using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New_CompendiumItems", menuName = "Consum/NewCompendiumItems")]
public class CompendiumItems : ScriptableObject
{
    public List<ConsumBaseInfo> compendiumOfItems;
}
