using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainbowInterpolationMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Material rend;
    [SerializeField] ColorsScriptableObject  colorsType;

    private void Start()
    {
        rend.SetColor("Color_E42400F8", colorsType.colors[(int)StaticInfo.characterToAcces.basicInfo.type]);
    }
}
