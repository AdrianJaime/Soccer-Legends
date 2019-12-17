using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeActiveScript : MonoBehaviour
{

    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void SetAciveGameObject(bool _aux)
    {
        gameObject.SetActive(_aux);
    }
}
