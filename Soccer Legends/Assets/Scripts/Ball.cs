using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Background") rb.velocity *= -1;
    }
}
