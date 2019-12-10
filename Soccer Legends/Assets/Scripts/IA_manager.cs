using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_manager : MonoBehaviour
{
    public enum formationPositions { FORWARD, PIVOT, GOALKEEPER}
    enum IA_State { FREE_BALL, PLAYER_HAS_BALL, IA_HAS_BALL}

    IA_State ia_State;

    [SerializeField]
    GameObject[] ia_players;

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
        Vector2 ballPos = new Vector2(GameObject.FindGameObjectWithTag("Ball").transform.position.x, GameObject.FindGameObjectWithTag("Ball").transform.position.y);       
        switch (ia_State)
        {
            case IA_State.FREE_BALL:
                Vector2 IA_ObjectivePos = Vector2.zero;
                for (int i = 0; i < ia_players.Length; i++)
                {
                    if(ia_players[i].GetComponent<IA_Player_PVE>().ia_formationPos == formationPositions.PIVOT)
                    {
                        IA_ObjectivePos = new Vector2(ballPos.x + ia_players[i].GetComponent<IA_Player_PVE>().startPosition.x,
                            ballPos.y + ia_players[i].GetComponent<IA_Player_PVE>().startPosition.y) / 2.0f;
                    }
                    else if(ia_players[i].GetComponent<IA_Player_PVE>().ia_formationPos == formationPositions.FORWARD)
                    {
                        IA_ObjectivePos = ballPos;
                    }
                    ia_players[i].GetComponent<IA_Player_PVE>().MoveTo(new float[] { IA_ObjectivePos.x, IA_ObjectivePos.y, 0.0f });
                }
                break;
            case IA_State.PLAYER_HAS_BALL:
                bool player_in_IA_Camp = false;
                Vector2[] playerPositions = new Vector2[manager.myPlayers.Length - 1];
                GameObject pivot, closeForward, farForward, playerWithBall, playerCloseToBall;
                pivot = closeForward = farForward = playerWithBall = playerCloseToBall = playerWithBall = null;

                //Find player with Ball
                for(int i = 0; i < manager.myPlayers.Length - 1; i++)
                {
                    playerPositions[i] = manager.myPlayers[i].transform.position;
                    //We also check if there is any player inside the IA team field for the pivot position
                    if (!player_in_IA_Camp && playerPositions[i].y > 0.0f) player_in_IA_Camp = true;
                    if (manager.myPlayers[i].GetComponent<MyPlayer_PVE>().ball != null)
                    {
                        playerWithBall = manager.myPlayers[i];
                        //Now find the closest player to this player with the ball
                        int closestIdx = -1;
                        float closestDist = -1.0f;
                        for (int j = 0; j < manager.myPlayers.Length - 1; j++)
                        {
                            if(j != i)
                            {
                                if(closestDist == -1.0f)
                                {
                                    closestIdx = j;
                                    closestDist = Vector2.Distance(manager.myPlayers[i].transform.position, manager.myPlayers[j].transform.position);
                                }
                                else if(Vector2.Distance(manager.myPlayers[i].transform.position, manager.myPlayers[j].transform.position) < closestDist)
                                {
                                    closestIdx = j;
                                    closestDist = Vector2.Distance(manager.myPlayers[i].transform.position, manager.myPlayers[j].transform.position);
                                }
                            }
                        }
                        playerCloseToBall = manager.myPlayers[closestIdx];
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
                closeForward.GetComponent<IA_Player_PVE>().MoveTo(new float[] { playerWithBall.transform.position.x, playerWithBall.transform.position.y, 0.0f });
                farForward.GetComponent<IA_Player_PVE>().MoveTo(new float[] { playerCloseToBall.transform.position.x, playerCloseToBall.transform.position.y, 0.0f });
                //Set Pivot
                pivot = ia_players[0];
                Vector2 pivotObjectivePos = Vector2.zero;

                if (player_in_IA_Camp)
                {
                    float y_value = pivot.GetComponent<IA_Player_PVE>().startPosition.y;
                    if (ballPos.y > pivot.transform.position.y) y_value = ballPos.y;
                    pivot.GetComponent<IA_Player_PVE>().MoveTo(new float[] { ballPos.x, y_value, 0.0f });
                }
                else
                {
                    pivotObjectivePos = new Vector2(ballPos.x + pivot.GetComponent<IA_Player_PVE>().startPosition.x,
                            ballPos.y + pivot.GetComponent<IA_Player_PVE>().startPosition.y) / 2.0f;
                    pivot.GetComponent<IA_Player_PVE>().MoveTo(new float[] { pivotObjectivePos.x, pivotObjectivePos.y, 0.0f });
                }
                break;
            case IA_State.IA_HAS_BALL:
                GameObject forward_with_ball, forward_without_ball, goal;
                pivot = ia_players[0];
                goal = GameObject.Find("Goal 1");
                if (ia_players[1].GetComponent<IA_Player_PVE>().ball != null)
                {
                    forward_with_ball = ia_players[1];
                    forward_without_ball = ia_players[2];
                }
                else
                {
                    forward_with_ball = ia_players[2];
                    forward_without_ball = ia_players[1];
                }

                if(forward_with_ball.transform.position.x < 0.0f)
                {
                   forward_with_ball.GetComponent<IA_Player_PVE>().MoveTo(new float[] { goal.transform.position.x - 1.0f, goal.transform.position.y, 0.0f });
                   forward_without_ball.GetComponent<IA_Player_PVE>().MoveTo(new float[] { goal.transform.position.x + 1.0f, goal.transform.position.y, 0.0f });
                }
                else
                {
                    forward_with_ball.GetComponent<IA_Player_PVE>().MoveTo(new float[] { goal.transform.position.x + 1.0f, goal.transform.position.y, 0.0f });
                    forward_without_ball.GetComponent<IA_Player_PVE>().MoveTo(new float[] { goal.transform.position.x - 1.0f, goal.transform.position.y, 0.0f });
                }
                if (pivot.transform.position.y > forward_with_ball.transform.position.y && pivot.transform.position.y > forward_without_ball.transform.position.y)
                {
                    pivot.GetComponent<IA_Player_PVE>().MoveTo(new float[] { goal.transform.position.x, goal.transform.position.y + 4.0f, 0.0f });
                }
                else
                {
                    pivot.GetComponent<IA_Player_PVE>().MoveTo(new float[] { pivot.GetComponent<IA_Player_PVE>().startPosition.x, pivot.GetComponent<IA_Player_PVE>().startPosition.y, 0.0f });
                }
                break;
            default:
                break;
        }
    }
}
