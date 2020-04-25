using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "New_SpriteCompendium", menuName = "Cards/newSpriteCompendium")]
public class SpriteConpendiumSO : ScriptableObject
{

    public List<Sprite> sprites;
}
