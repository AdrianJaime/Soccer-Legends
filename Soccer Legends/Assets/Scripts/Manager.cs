using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviour
{
    public GameObject playerPrefab, ballPrefab;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) PhotonNetwork.Instantiate(ballPrefab.name, Vector3.zero, ballPrefab.transform.rotation);
    }

    void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, playerPrefab.transform.position - new Vector3(0,5,0), playerPrefab.transform.rotation);
    }
}
