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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        if (transform.parent != null)
        {
            transform.position = transform.parent.position;
        }
        else if (Vector2.Distance(transform.position, -smoothMove) < 4) transform.position = Vector3.Lerp(transform.position, -smoothMove, Time.deltaTime * 10);
        else transform.position = -smoothMove;
    }

    public void ShootBall(float[] _dir)
    {
        Vector2 shootDir = new Vector2(_dir[0] - shootPosition.x, _dir[1] - shootPosition.y).normalized;
        if (rb.velocity == Vector2.zero)
        {
            if(shooterIsMaster)GetComponent<Rigidbody2D>().AddForce(-shootDir * kick, ForceMode2D.Impulse);
            else  GetComponent<Rigidbody2D>().AddForce(shootDir * kick, ForceMode2D.Impulse);
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
        if (collision.tag == "Wall" && photonView.IsMine)
        {
            if (collision.name == "U_Wall" || collision.name == "D_Wall") rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            if (collision.name == "L_Wall" || collision.name == "R_Wall") rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
    }
}
