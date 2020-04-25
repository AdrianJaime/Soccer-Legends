using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New_Color", menuName = "Cards/newColor")]
public class ColorsScriptableObject : ScriptableObject
{
    public List<Color> colors;
}
