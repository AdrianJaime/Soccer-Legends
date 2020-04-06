using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New_CompendiumCharacters", menuName = "Cards/NewCompendium")]
public class CharactersCompendium : ScriptableObject
{
    public List<CharacterInfo> compendiumOfCharacters;

}
