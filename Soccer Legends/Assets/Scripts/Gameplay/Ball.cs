using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviourPun, IPunObservable
{
    private Rigidbody2D rb;
    private Vector2 smoothMove;
    private Vector3 realPos;
    private Vector3 localCamPos;
    private Vector3 lastCamPosition;

    public bool shoot = false;
    public Vector2 direction, shootPosition;
    public bool shooterIsMaster;
    public int kick;
    private bool newBall;
    public float shootTimeRef;

    Collider2D area1, area2;

    public bool inArea = false;

    GameObject mainCamera;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        shootTimeRef = Time.time - 0.05f;
        if (GameObject.Find("Manager").GetComponent<PVE_Manager>() == null) transform.GetChild(0).gameObject.AddComponent<PVP_cameraMovement>();
        else transform.GetChild(0).gameObject.AddComponent<cameraMovement>();
        area1 = GameObject.Find("Area 1").GetComponent<Collider2D>();
        area2 = GameObject.Find("Area 2").GetComponent<Collider2D>();
        RepositionBall();
    }
    private void Update()
    {
        if (!shoot)
        {
            if (!photonView.IsMine)
            {
                smoothMovement();
            }
            else if (transform.parent != null)
            {
                transform.localPosition = new Vector3(0, -0.5f, transform.localPosition.z);
            }
        }
        if (area1.bounds.Contains(new Vector3(transform.position.x, transform.position.y, area1.transform.position.z)) ||
            area2.bounds.Contains(new Vector3(transform.position.x, transform.position.y, area2.transform.position.z))) inArea = true;
        else inArea = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    {
        if (stream.IsWriting)
        {
            stream.SendNext(new Vector3(-transform.position.x, -transform.position.y, transform.position.z)); //Solo se envía si se está moviendo.
            if(transform.parent != null)
            {
                stream.SendNext(true);
                stream.SendNext(transform.parent.GetComponent<MyPlayer>().photonView.ViewID);
            }
            else
            {
                stream.SendNext(false);
                stream.SendNext(0);
            }
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
            bool parent = (bool)stream.ReceiveNext();
            int id = (int)stream.ReceiveNext();
            if (parent) transform.parent = PhotonView.Find(id).transform;
            else transform.parent = null;
        }
    }
    private void smoothMovement()
    {
        if (GameObject.Find("Manager").GetComponent<PVE_Manager>() == null)
        {
            if (transform.parent != null)
            {
                transform.localPosition = new Vector3(0, -0.5f, transform.localPosition.z);
            }
            else if (Vector2.Distance(transform.position, smoothMove) < 1.5f) transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 20);
            else transform.position = smoothMove;
        }
        else
        {
            if (transform.parent != null)
            {
                transform.localPosition = new Vector3(0, -0.5f, transform.localPosition.z);
            }
            else if(GameObject.Find("Manager").GetComponent<PVE_Manager>().FindWhoHasTheBall() != null)
            {
                transform.parent = GameObject.Find("Manager").GetComponent<PVE_Manager>().FindWhoHasTheBall().transform;
            }
        }
    }

    [PunRPC]
    public void ShootBall(float[] _dir)
    {
        Vector2 shootDir = new Vector2(_dir[0] - shootPosition.x, _dir[1] - shootPosition.y).normalized;
        if (rb.velocity == Vector2.zero)
        {
            GetComponent<Rigidbody2D>().AddForce(shootDir * kick, ForceMode2D.Impulse);
            Debug.Log("Shoot direction is-> " + shootDir);
            shoot = false;
            direction = Vector2.zero;
            transform.parent = null;
            newBall = false;
        }
    }

    [PunRPC]
    public void RepositionBall()
    {
        transform.parent = null;
        transform.position = Vector3.zero;
        rb.velocity = Vector2.zero;
        ShootBall(new float[] { Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), transform.position.x, transform.position.y });
        newBall = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall" && (photonView.IsMine || GameObject.Find("Manager").GetComponent<PVE_Manager>() != null))
        {
            if ((collision.name == "U_Wall" || collision.name == "D_Wall")) rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            if (collision.name == "L_Wall" || collision.name == "R_Wall") rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            if(!newBall && (collision.name == "Goal 1" || collision.name == "Goal 2"))
            {
                if (GameObject.Find("Manager").GetComponent<PVE_Manager>() != null)
                {
                    if (GameObject.Find("Manager").GetComponent<PVE_Manager>().GameOn) return;
                    if (transform.position.y > 0) GameObject.Find("Manager").GetComponent<PVE_Manager>().Goal(true);
                    else GameObject.Find("Manager").GetComponent<PVE_Manager>().Goal(false);
                }
                else if (GameObject.Find("Manager").GetComponent<Manager>() != null)
                {
                    if (GameObject.Find("Manager").GetComponent<Manager>().GameOn) return;
                    GameObject.Find("Manager").GetComponent<Manager>().photonView.RPC("Goal", RpcTarget.AllViaServer);
                }
            }
        }
    }
}
