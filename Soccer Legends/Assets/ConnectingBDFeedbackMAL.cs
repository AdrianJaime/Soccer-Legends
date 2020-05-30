using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConnectingBDFeedbackMAL : MonoBehaviour
{
    [SerializeField] float timeToWait;
    [SerializeField] GameObject panel;
    [SerializeField] UnityEvent ListenerEvent; 

    void Start()
    {
        panel.SetActive(true);
        StartCoroutine(DestroyConnecting());

    }

    // every 2 seconds perform the print()
    private IEnumerator DestroyConnecting()
    {
   
        yield return new WaitForSeconds(timeToWait);
        Destroy(gameObject);
        if(ListenerEvent!=null)
            ListenerEvent.Invoke();
    }
}
