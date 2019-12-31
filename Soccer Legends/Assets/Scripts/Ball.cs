using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviourPun, IPunObservable
{
    private Rigidbody2D rb;
    private Vector2 smoothMove;
    private Vector3 realPos;

    public bool shoot = false;
    public Vector2 direction, shootPosition;
    public bool shooterIsMaster;
    public int kick;

    GameObject mainCamera;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ShootBall(new float[] { Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), transform.position.x, transform.position.y});
    }
    private void Update()
    {
        if (!photonView.IsMine)
        {
            smoothMovement();
        }
        else if (shoot) {
            float[] dir = { direction.x, direction.y };
            ShootBall(dir);
        }
        checkClosestToCam();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    {
        if (stream.IsWriting)
        {
            stream.SendNext(new Vector3(transform.position.x, transform.position.y, transform.position.z)); //Solo se envía si se está moviendo.
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
        }
    }
    private void smoothMovement()
    {
        if (GameObject.Find("Manager").GetComponent<PVE_Manager>() == null)
        {
            if (transform.parent != null)
            {
                transform.position =transform.parent.position;
            }
            else if (Vector2.Distance(transform.position, -smoothMove) < 4) transform.position = Vector3.Lerp(transform.position, -smoothMove, Time.deltaTime * 10);
            else transform.position = -smoothMove;
        }
        else
        {
            if (transform.parent != null)
            {
                transform.localPosition = new Vector3(0, -0.5f, 0);
            }
            else if(GameObject.Find("Manager").GetComponent<PVE_Manager>().FindWhoHasTheBall() != null)
            {
                transform.parent = GameObject.Find("Manager").GetComponent<PVE_Manager>().FindWhoHasTheBall().transform;
            }
        }
    }

    public void ShootBall(float[] _dir)
    {
        Vector2 shootDir = new Vector2(_dir[0] - shootPosition.x, _dir[1] - shootPosition.y).normalized;
        if (rb.velocity == Vector2.zero)
        {
            if(shooterIsMaster)GetComponent<Rigidbody2D>().AddForce(-shootDir * kick, ForceMode2D.Impulse);
            else  GetComponent<Rigidbody2D>().AddForce(shootDir * kick, ForceMode2D.Impulse);
            Debug.Log("Shoot direction is-> " + shootDir);
            shoot = false;
            direction = Vector2.zero;
        }
    }

    [PunRPC]
    public void RepositionBall()
    {
        transform.parent = null;
        transform.position = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall" && (photonView.IsMine || GameObject.Find("Manager").GetComponent<PVE_Manager>() != null))
        {
            if (collision.name == "U_Wall" || collision.name == "D_Wall") rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            if (collision.name == "L_Wall" || collision.name == "R_Wall") rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
    }

    void checkClosestToCam()
    {
        if (transform.parent != null)
        {
            if (GameObject.FindGameObjectWithTag("MainCamera") != null) GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
            transform.parent.GetChild(1).gameObject.SetActive(true);
            return;
        }
        if(Time.frameCount % 30 != 0) return;
        GameObject[] players = new GameObject[4];
        float minDist;
        int shortestDistIdx;

        if(GameObject.Find("Manager").GetComponent<Manager>() != null)
        {
            //GameObject.Find("Manager").GetComponent<Manager>().
        }
        else
        {
            players = GameObject.Find("Manager").GetComponent<PVE_Manager>().myPlayers;
        }

        minDist = Vector2.Distance(transform.position, players[0].transform.position);
        shortestDistIdx = 0;

        for(int i = 1; i < players.Length - 2; i++)
        {
            if(Vector2.Distance(transform.position, players[i].transform.position) < minDist)
            {
                minDist = Vector2.Distance(transform.position, players[i].transform.position);
                shortestDistIdx = i;
            }
        }

        if(GameObject.FindGameObjectWithTag("MainCamera") != null) GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);

        if (GameObject.Find("Manager").GetComponent<Manager>() != null)
        {
            ;//GameObject.Find("Manager").GetComponent<Manager>().
        }
        else
        {
            players[shortestDistIdx].GetComponent<MyPlayer_PVE>().playerCamera.SetActive(true);
        }
    }
}
