using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideElementsDragonBones : MonoBehaviour
{

    [SerializeField] List<GameObject> elementsToHide;
    // Start is called before the first frame update
    void Start()
    {
       foreach(GameObject element in elementsToHide)
        {
            element.SetActive(false);
        } 
    }


}
