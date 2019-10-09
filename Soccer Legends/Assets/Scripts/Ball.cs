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
    public Vector2 direction;
    public GameObject lastPlayer;

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
            stream.SendNext(new Vector3(-transform.position.x, -transform.position.y, transform.position.z)); //Solo se envía si se está moviendo.
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
        else transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Background" && photonView.IsMine) rb.velocity *= -1;
    }

    public void ShootBall(float[] _dir)
    {
        Vector2 shootDir = new Vector2(_dir[0] - transform.position.x, _dir[1] - transform.position.y).normalized;
        if (rb.velocity == Vector2.zero)
        {
            GetComponent<Rigidbody2D>().AddForce(shootDir * 10 /*KICK*/, ForceMode2D.Impulse);
            //else  GetComponent<Rigidbody2D>().AddForce(-shootDir * 10 /*KICK*/, ForceMode2D.Impulse);
            shoot = false;
            direction = Vector2.zero;
        }
    }
}
