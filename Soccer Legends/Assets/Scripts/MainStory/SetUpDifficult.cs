using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpDifficult : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("stages_difficult", 0);
    }

}
