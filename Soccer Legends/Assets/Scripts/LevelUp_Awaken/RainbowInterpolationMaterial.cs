using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainbowInterpolationMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Material rend;
    [SerializeField] Color []colors;

    private void Start()
    {
        rend.SetColor("Color_E42400F8", colors[(int)StaticInfo.characterToAcces.basicInfo.type]);
    }
}
