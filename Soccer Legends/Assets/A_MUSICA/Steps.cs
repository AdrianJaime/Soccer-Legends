using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steps : MonoBehaviour
{

    public int terrainIndex;

    private void Step()
    {
        GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("surface", terrainIndex);
        GetComponent<FMODUnity.StudioEventEmitter>().Play();
    }
}
