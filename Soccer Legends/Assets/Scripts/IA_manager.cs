using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IA_manager : MonoBehaviour
{
    public enum formationPositions { FORWARD, PIVOT, GOALKEEPER}
    public enum strategy { EQUILIBRATED, OFFENSIVE, DEFFENSIVE }
    enum IA_State { FREE_BALL, PLAYER_HAS_BALL, IA_HAS_BALL}

    public strategy teamStrategy;
    IA_State ia_State;

    [SerializeField]
    GameObject[] ia_players;

    [SerializeField]
    bool playerTeam;

    [SerializeField]
    float separationDist;

    PVE_Manager mg;

    private void Start()
    {
        mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        checkIA_State();
        processIA();
    }

    private void LateUpdate()
    {
        processSeparation();
    }

    void checkIA_State()
    {
        ia_State = (IA_State)mg.HasTheBall();
    }

    void processIA()
    {
        Vector2 ballPosition = new Vector2(GameObject.FindGameObjectWithTag("Ball").transform.position.x, GameObject.FindGameObjectWithTag("Ball").transform.position.y);       
        switch (ia_State)
        {
            case IA_State.FREE_BALL:
                if (playerTeam) catchingBallAlgorithm(mg.myIA_Players, ballPosition);
                else catchingBallAlgorithm(mg.myPlayers, ballPosition);
                break;
            case IA_State.PLAYER_HAS_BALL:
                if (playerTeam) atackAlgorithm(mg.myIA_Players, ballPosition);
                else deffendAlgorithm(mg.myPlayers, ballPosition);
                break;
            case IA_State.IA_HAS_BALL:
                if (playerTeam) deffendAlgorithm(mg.myIA_Players, ballPosition);
                else atackAlgorithm(mg.myPlayers, ballPosition);
                break;
            default:
                break;
        }
    }

    void catchingBallAlgorithm(GameObject[]rivalPlayers, Vector3 ballPos)
    {
        Vector2 IA_ObjectivePos = Vector2.zero;
        for (int i = 0; i < ia_players.Length; i++)
        {
            if (ia_players[i].GetComponent<MyPlayer_PVE>().formationPos == formationPositions.PIVOT &&
                (Vector2.Distance(ia_players[i].transform.position, ballPos) > Vector2.Distance(ia_players[i + 1].transform.position, ballPos) ||
                    Vector2.Distance(ia_players[i].transform.position, ballPos) > Vector2.Distance(ia_players[i + 2].transform.position, ballPos)))
            {

                IA_ObjectivePos = new Vector2(ballPos.x + ia_players[i].GetComponent<MyPlayer_PVE>().startPosition.x,
                    ballPos.y + ia_players[i].GetComponent<MyPlayer_PVE>().startPosition.y) / 2.0f;
            }
            else
            {
                IA_ObjectivePos = new Vector2(ballPos.x, ballPos.y + 0.5f);
            }
            ia_players[i].GetComponent<MyPlayer_PVE>().MoveTo(new float[] { IA_ObjectivePos.x, IA_ObjectivePos.y, 0.0f });
        }
    }

    void deffendAlgorithm(GameObject[] _rivalPlayers, Vector3 _ballPos)
    {
        switch (teamStrategy)
        {
            case strategy.EQUILIBRATED:
                equilibratedDeffending(_rivalPlayers, _ballPos);
                break;
            case strategy.OFFENSIVE:
                break;
            case strategy.DEFFENSIVE:
                break;
        } 
    }

    void atackAlgorithm(GameObject[] _rivalPlayers, Vector3 _ballPos)
    {
        switch (teamStrategy)
        {
            case strategy.EQUILIBRATED:
                equilibratedAtacking(_rivalPlayers, _ballPos);
                break;
            case strategy.OFFENSIVE:
                break;
            case strategy.DEFFENSIVE:
                break;
        }
    }

    void processSeparation()
    {
        if (!mg.GameOn) return;
        for (int i = 0; i < ia_players.Length; i++)
        {
            Vector2 iaSparationForce, playerSeparationForce;
            iaSparationForce = playerSeparationForce = Vector2.zero;
            for (int j = 0; j < ia_players.Length; j++)
            {
                if (ia_players[j] != ia_players[i] && Vector2.Distance(ia_players[j].transform.position, ia_players[i].transform.position) < separationDist)
                {
                    ia_players[i].GetComponent<Rigidbody2D>().AddForce((ia_players[j].transform.position - ia_players[i].transform.position).normalized *
                        (separationDist - Vector2.Distance(ia_players[j].transform.position, ia_players[i].transform.position)) * -20, ForceMode2D.Force);
                }
                else ia_players[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }

    void equilibratedDeffending(GameObject[] rivalPlayers, Vector3 ballPos)
    {
        bool rival_in_our_Camp = false;
        Vector2[] playerPositions = new Vector2[rivalPlayers.Length];
        GameObject pivot, closeForward, farForward, playerWithBall, playerCloseToBall;
        pivot = closeForward = farForward = playerWithBall = playerCloseToBall = playerWithBall = null;

        //Find player with Ball
        for (int i = 0; i < rivalPlayers.Length; i++)
        {
            playerPositions[i] = rivalPlayers[i].transform.position;
            //We also check if there is any player inside the IA team field for the pivot position
            if (!rival_in_our_Camp && playerPositions[i].y > 0.0f) rival_in_our_Camp = true;
            if (rivalPlayers[i].GetComponent<MyPlayer_PVE>().ball != null)
            {
                playerWithBall = rivalPlayers[i];
                //Now find the closest player to this player with the ball
                int closestIdx = -1;
                float closestDist = -1.0f;
                for (int j = 0; j < rivalPlayers.Length; j++)
                {
                    if (j != i)
                    {
                        if (closestDist == -1.0f)
                        {
                            closestIdx = j;
                            closestDist = Vector2.Distance(rivalPlayers[i].transform.position, rivalPlayers[j].transform.position);
                        }
                        else if (Vector2.Distance(rivalPlayers[i].transform.position, rivalPlayers[j].transform.position) < closestDist)
                        {
                            closestIdx = j;
                            closestDist = Vector2.Distance(rivalPlayers[i].transform.position, rivalPlayers[j].transform.position);
                        }
                    }
                }
                playerCloseToBall = rivalPlayers[closestIdx];
            }
        }


        //Find closest Forward to player with ball
        if (Vector2.Distance(ia_players[1].transform.position, playerWithBall.transform.position) < Vector2.Distance(ia_players[2].transform.position, playerWithBall.transform.position))
        {
            closeForward = ia_players[1];
            farForward = ia_players[2];
        }
        else
        {
            closeForward = ia_players[2];
            farForward = ia_players[1];
        }

        //Set objectives
        closeForward.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { playerWithBall.transform.position.x, playerWithBall.transform.position.y, 0.0f });
        Vector2 farForwardPos = playerWithBall.transform.position + (playerCloseToBall.transform.position - playerWithBall.transform.position) / 2.0f;
        farForward.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { farForwardPos.x, farForwardPos.y - 0.5f, 0.0f });
        //Set Pivot
        pivot = ia_players[0];
        Vector2 pivotObjectivePos = Vector2.zero;

        if (rival_in_our_Camp)
        {
            float y_value = pivot.GetComponent<MyPlayer_PVE>().startPosition.y;
            if (ballPos.y > pivot.transform.position.y) y_value = ballPos.y;
            pivot.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { ballPos.x, y_value, 0.0f });
        }
        else
        {
            pivotObjectivePos = new Vector2(ballPos.x + pivot.GetComponent<MyPlayer_PVE>().startPosition.x,
                    ballPos.y + pivot.GetComponent<MyPlayer_PVE>().startPosition.y) / 2.0f;
            pivot.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { pivotObjectivePos.x, pivotObjectivePos.y, 0.0f });
        }
    }

    void equilibratedAtacking(GameObject[] rivalPlayers, Vector3 ballPos)
    {
        GameObject forward_with_ball, forward_without_ball, pivot, goal;
        bool pivotWithBall = false;
        pivot = ia_players[0];
        if (!playerTeam) goal = GameObject.Find("Goal 1");
        else goal = GameObject.Find("Goal 2");
        if (ia_players[1].GetComponent<MyPlayer_PVE>().ball != null)
        {
            forward_with_ball = ia_players[1];
            forward_without_ball = ia_players[2];
        }
        else
        {
            forward_with_ball = ia_players[2];
            forward_without_ball = ia_players[1];
            if (ia_players[0].GetComponent<MyPlayer_PVE>().ball != null)
            {
                pivotWithBall = true;
            }
        }

        float forwardWithBall_Y, forwardWithoutBall_Y;
        forwardWithBall_Y = forwardWithoutBall_Y = goal.transform.position.y;

        if (pivotWithBall)
        {
            if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(pivot.transform.position, goal.transform.position) && Vector2.Distance(forward_with_ball.transform.position, pivot.transform.position) >= 2)
                forwardWithBall_Y = pivot.transform.position.y;
            if (Vector2.Distance(forward_without_ball.transform.position, goal.transform.position) < Vector2.Distance(pivot.transform.position, goal.transform.position) && Vector2.Distance(forward_without_ball.transform.position, pivot.transform.position) >= 2)
                forwardWithoutBall_Y = pivot.transform.position.y;

        }


        if (forward_with_ball.transform.position.x < 0.0f)
        {
            forward_with_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x - 1.0f, forwardWithBall_Y, 0.0f });
            if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
                forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x + 1.0f, forwardWithoutBall_Y, 0.0f });
            else forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.x, forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
        }
        else
        {
            forward_with_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x + 1.0f, forwardWithBall_Y, 0.0f });
            if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
                forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x - 1.0f, forwardWithoutBall_Y, 0.0f });
            else forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.x, forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
        }
        if (Vector2.Distance(pivot.transform.position, goal.transform.position) > Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) &&
            Vector2.Distance(pivot.transform.position, goal.transform.position) > Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
        {
            pivot.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x, goal.transform.position.y + 4.0f * (goal.transform.position.y / (Mathf.Abs(goal.transform.position.y))), 0.0f });
        }
        else
        {
            pivot.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { pivot.GetComponent<MyPlayer_PVE>().startPosition.x, pivot.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
        }
    }

    public void check_IA_Shoot()
    {
        GameObject playerWithBall = null;
        foreach(GameObject player in mg.myIA_Players)
        {
            if (player.GetComponent<MyPlayer_PVE>().ball != null)
            {
                playerWithBall = player;
                break;
            }
        }
        if (!mg.GameOn || playerWithBall == null || (mg.myPlayers[0].transform.position.y > playerWithBall.transform.position.y
            && mg.myPlayers[1].transform.position.y > playerWithBall.transform.position.y && mg.myPlayers[2].transform.position.y > playerWithBall.transform.position.y))
        {
            playerWithBall.GetComponent<MyPlayer_PVE>().stablishNewShootCheck();
            return;
        }

        GameObject ball = playerWithBall.GetComponent<MyPlayer_PVE>().ball;
        Vector3 shootingTarget;
        List<Vector3> closePlayers = new List<Vector3>();

        for (int i = 0; i < mg.myIA_Players.Length; i++)
        {
            if (Vector2.Distance(playerWithBall.transform.position, mg.myIA_Players[i].transform.position) < 9 && mg.myIA_Players[i] != gameObject) closePlayers.Add(mg.myIA_Players[i].transform.position);
        }
        while (closePlayers.Count != 0)
        {
            shootingTarget = closePlayers[0];
            float minDist = Vector2.Distance(shootingTarget, playerWithBall.transform.position);
            if (closePlayers.Count > 1)
            {
                for (int i = 0; i < closePlayers.Count; i++)
                {
                    //Debug.Log("Player " + (i + 1).ToString() + "-> " + Vector2.Distance(transform.position, closePlayers[i]).ToString());
                    if (Vector2.Distance(playerWithBall.transform.position, closePlayers[i]) < minDist)
                    {
                        // Debug.Log("Target change to-> " + (i + 1).ToString());
                        shootingTarget = closePlayers[i];
                        minDist = Vector2.Distance(playerWithBall.transform.position, closePlayers[i]);
                    }
                }
            }
            RaycastHit2D hit;
            Vector3 dir = shootingTarget - ball.transform.position;
            //Debug.Log("Final distance-> " + Vector2.Distance(transform.position, shootingTarget).ToString());
            hit = Physics2D.Raycast(ball.transform.position, dir, 10);
            if (hit && hit.transform.parent == playerWithBall.transform.parent && hit.transform.name != playerWithBall.transform.name && !hit.transform.GetComponent<MyPlayer_PVE>().covered)
            {
                //Debug.Log(hit.transform.name);
                //Debug.Log("Shoot distance-> " + Vector2.Distance(transform.position, hit.transform.position).ToString());
                playerWithBall.GetComponent<MyPlayer_PVE>().ShootBall(new float[] { hit.transform.position.x, hit.transform.position.y - 0.25f, playerWithBall.transform.position.x, playerWithBall.transform.position.y });
                closePlayers.Clear();
            }
            else
            {
                // Debug.Log("Unable to pass to player with distance-> " + Vector2.Distance(transform.position, hit.transform.position).ToString());
                closePlayers.Remove(shootingTarget);
            }
        }

        if (ball != null) playerWithBall.GetComponent<MyPlayer_PVE>().stablishNewShootCheck();
    }
}
