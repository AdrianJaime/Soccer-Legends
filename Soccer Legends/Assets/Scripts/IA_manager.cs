using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IA_manager : MonoBehaviour
{
    public enum formationPositions { FORWARD, PIVOT, GOALKEEPER}
    enum IA_State { FREE_BALL, PLAYER_HAS_BALL, IA_HAS_BALL}

    IA_State ia_State;

    [SerializeField]
    GameObject[] ia_players;

    [SerializeField]
    bool playerTeam;

    PVE_Manager manager;

    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<PVE_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        checkIA_State();
        processIA();
    }

    void checkIA_State()
    {
        ia_State = (IA_State)manager.HasTheBall();
    }

    void processIA()
    {
        Vector2 ballPosition = new Vector2(GameObject.FindGameObjectWithTag("Ball").transform.position.x, GameObject.FindGameObjectWithTag("Ball").transform.position.y);       
        switch (ia_State)
        {
            case IA_State.FREE_BALL:
                if (playerTeam) catchingBallAlgorithm(manager.myIA_Players, ballPosition);
                else catchingBallAlgorithm(manager.myPlayers, ballPosition);
                break;
            case IA_State.PLAYER_HAS_BALL:
                if (playerTeam) atackAlgorithm(manager.myIA_Players, ballPosition);
                else deffendAlgorithm(manager.myPlayers, ballPosition);
                break;
            case IA_State.IA_HAS_BALL:
                if (playerTeam) deffendAlgorithm(manager.myIA_Players, ballPosition);
                else atackAlgorithm(manager.myPlayers, ballPosition);
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
            if (ia_players[i].GetComponent<MyPlayer_PVE>().formationPos == formationPositions.PIVOT)
            {
                IA_ObjectivePos = new Vector2(ballPos.x + ia_players[i].GetComponent<MyPlayer_PVE>().startPosition.x,
                    ballPos.y + ia_players[i].GetComponent<MyPlayer_PVE>().startPosition.y) / 2.0f;
            }
            else if (ia_players[i].GetComponent<MyPlayer_PVE>().formationPos == formationPositions.FORWARD)
            {
                IA_ObjectivePos = ballPos;
            }
            ia_players[i].GetComponent<MyPlayer_PVE>().MoveTo(new float[] { IA_ObjectivePos.x, IA_ObjectivePos.y, 0.0f });
        }
    }

    void deffendAlgorithm(GameObject[] rivalPlayers, Vector3 ballPos)
    {
        bool rival_in_our_Camp = false;
        Vector2[] playerPositions = new Vector2[rivalPlayers.Length - 1];
        GameObject pivot, closeForward, farForward, playerWithBall, playerCloseToBall;
        pivot = closeForward = farForward = playerWithBall = playerCloseToBall = playerWithBall = null;

        //Find player with Ball
        for (int i = 0; i < rivalPlayers.Length - 1; i++)
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
                for (int j = 0; j < rivalPlayers.Length - 1; j++)
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
        farForward.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { playerCloseToBall.transform.position.x, playerCloseToBall.transform.position.y, 0.0f });
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

    void atackAlgorithm(GameObject[] rivalPlayers, Vector3 ballPos)
    {
        GameObject forward_with_ball, forward_without_ball, pivot, goal;
        bool pivotWithBall = false;
        pivot = ia_players[0];
        if(!playerTeam)goal = GameObject.Find("Goal 1");
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
            if(ia_players[0].GetComponent<MyPlayer_PVE>().ball != null)
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
            if(Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
                forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x + 1.0f,forwardWithoutBall_Y, 0.0f });
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
}
