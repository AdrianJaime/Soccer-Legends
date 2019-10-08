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
        if (transform.parent != null)
        {
            transform.position = transform.parent.position;
        }
        else if (!photonView.IsMine)
        {
            smoothMovement();
        }

        if (shoot)
        {
            Vector2 shootDir = new Vector2(direction.x - transform.position.x, direction.y - transform.position.y).normalized;
            GetComponent<Rigidbody2D>().AddForce(shootDir * transform.parent.GetComponent<MyPlayer>().kick, ForceMode2D.Impulse);
            transform.SetParent(null);
            shoot = false;
            direction = Vector2.zero;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    {
        if (stream.IsWriting)
        {
            stream.SendNext(new Vector3(-transform.position.x, -transform.position.y, transform.position.z)); //Solo se envía si se está moviendo.
            //stream.SendNext(-rb.velocity);
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
            //rb.velocity = (Vector2)stream.ReceiveNext();
        }
    }
    private void smoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Background") rb.velocity *= -1;
    }
}
