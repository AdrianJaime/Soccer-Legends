using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;


public class IA_Player_PVE : MonoBehaviour
{

    public class Stats
    {
        public int shoot, technique, defense;

        public Stats(int _shoot, int _technique, int _defense)
        {
            shoot = _shoot;
            technique = _technique;
            defense = _defense;
        }
    }

    public Stats stats;
    public float speed, dist, maxPointDist, minPointDist, characterRad, maxSize, shootTime;
    public GameObject playerCamera, ball, line;
    public bool onMove = false, stunned = false, colliding = false;
    public string fightDir;
    public Vector3 playerObjective = Vector3.zero;
    public Vector2 startPosition = Vector2.zero;

    private Vector3 smoothMove, aux;
    private GameObject actualLine;
    private Manager mg;
    private List<Vector3> points;
    private Collider2D goal;
    private float touchTime;

    private void Start()
    {

        stats = new Stats(0, 0, 0);
        int[] starr = { Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10) };
        fightDir = null;
        //if (photonView.IsMine)
        //{
            mg = GameObject.Find("Manager").GetComponent<Manager>();
            goal = GameObject.FindGameObjectWithTag("Goal").GetComponent<Collider2D>();
        //}
        points = new List<Vector3>();
    }
    private void Update()
    {
        //if (photonView.IsMine) //Check if we're the local player
        //{
            if (startPosition == Vector2.zero) startPosition = transform.position;
            if (playerObjective != Vector3.zero) transform.position = Vector3.MoveTowards(transform.position, playerObjective, Time.deltaTime * 0.5f);
            //else if (actualLine && points.Count > 1 && mg.GameOn && !stunned) FollowLine();
        //}
        //else if(!photonView.IsMine)
        //{
           // smoothMovement();
        //}

    }
}
