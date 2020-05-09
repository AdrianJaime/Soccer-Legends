using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("destroyTap", 1.0f);
    }

    void destroyTap() { Destroy(gameObject); }
}
