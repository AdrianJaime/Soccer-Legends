using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IA_manager : MonoBehaviour
{
    public enum formationPositions { ALA, PIVOT, CIERRE, GOALKEEPER}
    public enum strategy { EQUILIBRATED, OFFENSIVE, DEFFENSIVE }
    enum IA_State { FREE_BALL, PLAYER_HAS_BALL, IA_HAS_BALL}

    public strategy teamStrategy;
    IA_State ia_State;

    [SerializeField]
    GameObject[] ia_players;

    public bool playerTeam;

    [SerializeField]
    float separationDist;

    [SerializeField]
    float separationForce;


    PVE_Manager mg;

    private void Start()
    {
        mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
        if (!playerTeam) teamStrategy = (strategy)Random.Range(0, 3);
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
                else
                {
                    atackAlgorithm(mg.myPlayers, ballPosition);
                    //check_IA_Shoot();//if (Time.frameCount % (60 * Random.Range(1, 4)) == 0) 
                }
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
            if (ia_players[i].GetComponent<MyPlayer_PVE>().formationPos == formationPositions.CIERRE &&
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
                offensiveDeffending(_rivalPlayers, _ballPos);
                break;
            case strategy.DEFFENSIVE:
                defensiveDeffending(_rivalPlayers, _ballPos);
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
                offensiveAtacking(_rivalPlayers, _ballPos);
                break;
            case strategy.DEFFENSIVE:
                defensiveAtacking(_rivalPlayers, _ballPos);
                break;
        }
        if (!playerTeam)
        {
            if(Vector2.Distance(_ballPos, mg.myPlayers[3].transform.position) < 1.5f)
            {
                for(int i = 0; i < mg.myIA_Players.Length; i++)
                {
                    if (mg.myIA_Players[i].GetComponent<MyPlayer_PVE>().ball)
                    {
                        shootToGoal(mg.myIA_Players[i]);
                        return;
                    }
                }
            }
        }
    }

    void processSeparation()
    {
        if (!mg.GameOn || ia_State == IA_State.FREE_BALL) return;
        for (int i = 1; i < ia_players.Length; i++)
        {
            for (int j = 0; j < ia_players.Length; j++)
            {
                if (j != i && Vector2.Distance(ia_players[j].transform.position, ia_players[i].transform.position) < separationDist)
                {
                    ia_players[i].GetComponent<Rigidbody2D>().AddForce((ia_players[j].transform.position - ia_players[i].transform.position).normalized *
                        (separationDist - Vector2.Distance(ia_players[j].transform.position, ia_players[i].transform.position)) * -separationForce, ForceMode2D.Force);
                }
                else ia_players[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }

    void equilibratedDeffending(GameObject[] rivalPlayers, Vector3 ballPos)
    {
        bool rival_in_our_Camp = false;
        Vector2[] playerPositions = new Vector2[rivalPlayers.Length];
        GameObject cierre, closeForward, farForward, playerWithBall, playerCloseToBall;
        cierre = closeForward = farForward = playerWithBall = playerCloseToBall = playerWithBall = null;

        //Find player with Ball
        for (int i = 0; i < rivalPlayers.Length; i++)
        {
            playerPositions[i] = rivalPlayers[i].transform.position;
            //We also check if there is any player inside the IA team field for the pivot position
            if (!rival_in_our_Camp && Vector2.Distance(playerPositions[i], rivalPlayers[i].GetComponent<MyPlayer_PVE>().goal.transform.position) < 7.5f) rival_in_our_Camp = true;
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
        if (playerWithBall.GetComponent<MyPlayer_PVE>().formationPos != formationPositions.GOALKEEPER)
        {
            closeForward.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { playerWithBall.transform.position.x, playerWithBall.transform.position.y, 0.0f });
            Vector2 farForwardPos = playerWithBall.transform.position + (playerCloseToBall.transform.position - playerWithBall.transform.position) / 2.0f;
            farForward.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { farForwardPos.x, farForwardPos.y - 0.5f, 0.0f });
        }
        else
        {
            farForward.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { playerWithBall.transform.position.x, (farForward.GetComponent<MyPlayer_PVE>().startPosition.y + playerCloseToBall.transform.position.y) / 2.0f, 0.0f });
            closeForward.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { playerCloseToBall.transform.position.x, playerCloseToBall.transform.position.y - 2.0f, 0.0f });
        }
        //Set Cierre
        cierre = ia_players[0];
        Vector2 pivotObjectivePos = Vector2.zero;

        if (rival_in_our_Camp)
        {
            float y_value = cierre.GetComponent<MyPlayer_PVE>().startPosition.y;
            if(Vector2.Distance(ballPos, cierre.transform.position) < 1.0f) y_value = ballPos.y;
            cierre.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { ballPos.x, y_value, 0.0f });
        }
        else
        {
            pivotObjectivePos = new Vector2(ballPos.x + cierre.GetComponent<MyPlayer_PVE>().startPosition.x,
                    ballPos.y + cierre.GetComponent<MyPlayer_PVE>().startPosition.y) / 2.0f;
            cierre.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { pivotObjectivePos.x, pivotObjectivePos.y, 0.0f });
        }
    }

    void offensiveDeffending(GameObject[] rivalPlayers, Vector3 ballPos)
    {
        Vector2[] playerPositions = new Vector2[rivalPlayers.Length];
        GameObject pivot, closePlayer, farPlayer, playerWithBall, playerCloseToBall, playerCloseToGoal, goal;
        pivot = closePlayer = farPlayer = playerWithBall = playerCloseToBall = playerWithBall = null;

        if (!playerTeam) goal = GameObject.Find("Goal 1");
        else goal = GameObject.Find("Goal 2");


        playerCloseToGoal = rivalPlayers[0];
        float minGoalDist = Vector2.Distance(playerCloseToGoal.transform.position, goal.transform.position);

        //Find player with Ball and closest to goal for the Pivot
        for (int i = 0; i < rivalPlayers.Length; i++)
        {
            playerPositions[i] = rivalPlayers[i].transform.position;

            if(rivalPlayers[i].GetComponent<MyPlayer_PVE>().formationPos != formationPositions.GOALKEEPER &&  Vector2.Distance(playerPositions[i], goal.transform.position) < minGoalDist)
            {
                minGoalDist = Vector2.Distance(playerPositions[i], goal.transform.position);
                playerCloseToGoal = rivalPlayers[i];
            }

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


        //Find closest Cierre/Ala to player with ball
        if (Vector2.Distance(ia_players[1].transform.position, playerWithBall.transform.position) < Vector2.Distance(ia_players[0].transform.position, playerWithBall.transform.position))
        {
            closePlayer = ia_players[1];
            farPlayer = ia_players[0];
        }
        else
        {
            closePlayer = ia_players[0];
            farPlayer = ia_players[1];
        }

        //Set Pivot
        pivot = ia_players[2];
        Vector2 pivotObjectivePos = Vector2.zero;

        //Set objectives
        if (playerWithBall.GetComponent<MyPlayer_PVE>().formationPos != formationPositions.GOALKEEPER)
        {
            closePlayer.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { playerWithBall.transform.position.x, playerWithBall.transform.position.y, 0.0f });
            Vector2 farForwardPos = playerWithBall.transform.position + (playerCloseToBall.transform.position - playerWithBall.transform.position) / 2.0f;
            farPlayer.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { farForwardPos.x, farForwardPos.y - 0.5f, 0.0f });
            pivot.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { ballPos.x, playerCloseToGoal.transform.position.y, 0.0f });
        }
        else
        {
            farPlayer.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { playerWithBall.transform.position.x, (farPlayer.GetComponent<MyPlayer_PVE>().startPosition.y + playerCloseToBall.transform.position.y) / 2.0f, 0.0f });
            closePlayer.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { playerCloseToBall.transform.position.x, playerCloseToBall.transform.position.y - 2.0f, 0.0f });
            pivotObjectivePos = playerCloseToBall.transform.position + (playerWithBall.transform.position - playerCloseToBall.transform.position) / 2.0f;
            pivot.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { pivotObjectivePos.x, pivotObjectivePos.y, 0.0f });
        }
    }

    void defensiveDeffending(GameObject[] rivalPlayers, Vector3 ballPos)
    {
        bool rival_in_our_Camp = false;
        Vector2[] playerPositions = new Vector2[rivalPlayers.Length];
        GameObject cierre, closeForward, farForward, playerWithBall, playerCloseToBall;
        cierre = closeForward = farForward = playerWithBall = playerCloseToBall = playerWithBall = null;

        //Find player with Ball
        for (int i = 0; i < rivalPlayers.Length; i++)
        {
            playerPositions[i] = rivalPlayers[i].transform.position;
            //We also check if there is any player inside the IA team field for the pivot position
            if (!rival_in_our_Camp && Vector2.Distance(playerPositions[i], rivalPlayers[i].GetComponent<MyPlayer_PVE>().goal.transform.position) < 7.5f) rival_in_our_Camp = true;
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
        //Set Cierre
        cierre = ia_players[0];
        Vector2 pivotObjectivePos = Vector2.zero;
        float y_value = cierre.GetComponent<MyPlayer_PVE>().startPosition.y;
        if (Vector2.Distance(ballPos, cierre.transform.position) < 1.0f) y_value = ballPos.y;
        cierre.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { ballPos.x, y_value, 0.0f });
        //Set Forwards
        y_value = closeForward.GetComponent<MyPlayer_PVE>().startPosition.y;
        if (Vector2.Distance(ballPos, closeForward.transform.position) < 1.0f) y_value = ballPos.y;
        closeForward.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { playerWithBall.transform.position.x, y_value, 0.0f });
        y_value = farForward.GetComponent<MyPlayer_PVE>().startPosition.y;
        if (Vector2.Distance(ballPos, farForward.transform.position) < 1.0f) y_value = ballPos.y;
        farForward.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { playerCloseToBall.transform.position.x, y_value, 0.0f });
    }

    void equilibratedAtacking(GameObject[] rivalPlayers, Vector3 ballPos)
    {
        if (transform.GetChild(3).GetComponent<MyPlayer_PVE>().ball == null)
        {
            GameObject forward_with_ball, forward_without_ball, cierre, goal;
            bool cierreWithBall = false;
            cierre = ia_players[0];
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
                    cierreWithBall = true;
                }
            }

            float forwardWithBall_Y, forwardWithoutBall_Y;
            forwardWithBall_Y = forwardWithoutBall_Y = goal.transform.position.y;

            if (cierreWithBall)
            {
                if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(cierre.transform.position, goal.transform.position) && Vector2.Distance(forward_with_ball.transform.position, cierre.transform.position) >= 2)
                    forwardWithBall_Y = cierre.transform.position.y;
                if (Vector2.Distance(forward_without_ball.transform.position, goal.transform.position) < Vector2.Distance(cierre.transform.position, goal.transform.position) && Vector2.Distance(forward_without_ball.transform.position, cierre.transform.position) >= 2)
                    forwardWithoutBall_Y = cierre.transform.position.y;

            }


            if (forward_with_ball.transform.position.x < 0.0f)
            {
                forward_with_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x - separationDist / 2.0f, forwardWithBall_Y, 0.0f });
                if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
                    forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x + separationDist / 2.0f, forwardWithoutBall_Y, 0.0f });
                else forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.x, forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
            }
            else
            {
                forward_with_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x + separationDist / 2.0f, forwardWithBall_Y, 0.0f });
                if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
                    forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x - separationDist / 2.0f, forwardWithoutBall_Y, 0.0f });
                else forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.x, forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
            }
            if (Vector2.Distance(cierre.transform.position, goal.transform.position) > Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) &&
                Vector2.Distance(cierre.transform.position, goal.transform.position) > Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
            {
                cierre.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x, goal.transform.position.y + 8.0f * (goal.transform.position.y / (Mathf.Abs(goal.transform.position.y))), 0.0f });
            }
            else
            {
                cierre.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { cierre.GetComponent<MyPlayer_PVE>().startPosition.x, cierre.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
            }
        }
        else
        {
            for(int i = 0; i < ia_players.Length; i++)
            {
                MyPlayer_PVE player = ia_players[i].GetComponent<MyPlayer_PVE>();
                player.MoveTo(new float[] { player.startPosition.x, player.startPosition.y, 0.0f });
            }
        }
    }

    void offensiveAtacking(GameObject[] rivalPlayers, Vector3 ballPos)
    {
        if (transform.GetChild(3).GetComponent<MyPlayer_PVE>().ball == null)
        {
            GameObject forward_with_ball, forward_without_ball, cierre, goal;
            bool cierreWithBall = false;
            cierre = ia_players[0];
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
                    cierreWithBall = true;
                }
            }

            float forwardWithBall_Y, forwardWithoutBall_Y;
            forwardWithBall_Y = forwardWithoutBall_Y = goal.transform.position.y;

            if (cierreWithBall)
            {
                if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(cierre.transform.position, goal.transform.position) && Vector2.Distance(forward_with_ball.transform.position, cierre.transform.position) >= 2)
                    forwardWithBall_Y = cierre.transform.position.y;
                if (Vector2.Distance(forward_without_ball.transform.position, goal.transform.position) < Vector2.Distance(cierre.transform.position, goal.transform.position) && Vector2.Distance(forward_without_ball.transform.position, cierre.transform.position) >= 2)
                    forwardWithoutBall_Y = cierre.transform.position.y;

            }


            if (forward_with_ball.transform.position.x < 0.0f)
            {
                forward_with_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x - separationDist / 2.0f, forwardWithBall_Y, 0.0f });
                if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
                    forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x + separationDist / 2.0f, forwardWithoutBall_Y, 0.0f });
                else forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.x, forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
            }
            else
            {
                forward_with_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x + separationDist / 2.0f, forwardWithBall_Y, 0.0f });
                if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
                    forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x - separationDist / 2.0f, forwardWithoutBall_Y, 0.0f });
                else forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.x, forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
            }
            if (Vector2.Distance(cierre.transform.position, goal.transform.position) > Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) &&
                Vector2.Distance(cierre.transform.position, goal.transform.position) > Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
            {
                cierre.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x, goal.transform.position.y + 4.0f * (goal.transform.position.y / (Mathf.Abs(goal.transform.position.y))), 0.0f });
            }
            else
            {
                cierre.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { cierre.GetComponent<MyPlayer_PVE>().startPosition.x, cierre.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
            }
        }
        else
        {
            for (int i = 0; i < ia_players.Length; i++)
            {
                MyPlayer_PVE player = ia_players[i].GetComponent<MyPlayer_PVE>();
                player.MoveTo(new float[] { player.startPosition.x, player.startPosition.y, 0.0f });
            }
        }
    }

    void defensiveAtacking(GameObject[] rivalPlayers, Vector3 ballPos)
    {
        if (transform.GetChild(3).GetComponent<MyPlayer_PVE>().ball == null)
        {
            GameObject forward_with_ball, forward_without_ball, cierre, goal;
            bool cierreWithBall = false;
            cierre = ia_players[0];
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
                    cierreWithBall = true;
                }
            }

            float forwardWithBall_Y, forwardWithoutBall_Y;
            forwardWithBall_Y = forwardWithoutBall_Y = goal.transform.position.y;

            if (cierreWithBall)
            {
                if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(cierre.transform.position, goal.transform.position) && Vector2.Distance(forward_with_ball.transform.position, cierre.transform.position) >= 2)
                    forwardWithBall_Y = cierre.transform.position.y;
                if (Vector2.Distance(forward_without_ball.transform.position, goal.transform.position) < Vector2.Distance(cierre.transform.position, goal.transform.position) && Vector2.Distance(forward_without_ball.transform.position, cierre.transform.position) >= 2)
                    forwardWithoutBall_Y = cierre.transform.position.y;

            }
            else if (forward_with_ball == ia_players[1]) forwardWithBall_Y = goal.transform.position.y / 2.0f;
            else if (forward_without_ball == ia_players[1]) forwardWithoutBall_Y = goal.transform.position.y / 2.0f;

            if (forward_with_ball.transform.position.x < 0.0f)
            {
                forward_with_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x - separationDist / 2.0f, forwardWithBall_Y, 0.0f });
                if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
                    forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x + separationDist / 2.0f, forwardWithoutBall_Y, 0.0f });
                else forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.x, forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
            }
            else
            {
                forward_with_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x + separationDist / 2.0f, forwardWithBall_Y, 0.0f });
                if (Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) < Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
                    forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x - separationDist / 2.0f, forwardWithoutBall_Y, 0.0f });
                else forward_without_ball.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.x, forward_without_ball.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
            }
            if (Vector2.Distance(cierre.transform.position, goal.transform.position) > Vector2.Distance(forward_with_ball.transform.position, goal.transform.position) &&
                Vector2.Distance(cierre.transform.position, goal.transform.position) > Vector2.Distance(forward_without_ball.transform.position, goal.transform.position))
            {
                cierre.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { goal.transform.position.x, ia_players[1].transform.position.y + 8.0f * (goal.transform.position.y / (Mathf.Abs(goal.transform.position.y))), 0.0f });
            }
            else
            {
                cierre.GetComponent<MyPlayer_PVE>().MoveTo(new float[] { cierre.GetComponent<MyPlayer_PVE>().startPosition.x, cierre.GetComponent<MyPlayer_PVE>().startPosition.y, 0.0f });
            }
        }
        else
        {
            for (int i = 0; i < ia_players.Length; i++)
            {
                MyPlayer_PVE player = ia_players[i].GetComponent<MyPlayer_PVE>();
                player.MoveTo(new float[] { player.startPosition.x, player.startPosition.y, 0.0f });
            }
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
            && mg.myPlayers[1].transform.position.y > playerWithBall.transform.position.y && mg.myPlayers[2].transform.position.y > playerWithBall.transform.position.y
            && playerWithBall.transform.position.y > -4))
        {
            playerWithBall.GetComponent<MyPlayer_PVE>().stablishNewShootCheck();
            return;
        }

        switch (playerWithBall.transform.GetComponent<MyPlayer_PVE>().formationPos)
        {
            case formationPositions.CIERRE:
                if (Random.Range(1, 101) > 90) playerWithBall.GetComponent<MyPlayer_PVE>().stablishNewShootCheck();
                else passToPlayer(playerWithBall);
                break;
            case formationPositions.ALA:
                if (Random.Range(1 + playerWithBall.transform.position.y * -22.5f, 101) > 90) passToPlayer(playerWithBall);
                else playerWithBall.GetComponent<MyPlayer_PVE>().stablishNewShootCheck();
                break;
            case formationPositions.PIVOT:
                float randVal = Random.Range(1 + playerWithBall.transform.position.y * -22.5f, 101);
                Debug.Log(randVal);
                if (playerWithBall.GetComponent<MyPlayer_PVE>().ball.GetComponent<Ball>().inArea) shootToGoal(playerWithBall);
                else if (randVal > 90) shootToGoal(playerWithBall);
                else passToPlayer(playerWithBall);
                break;
            case formationPositions.GOALKEEPER:
                passToPlayer(playerWithBall);
                break;
        }

       //passToPlayer(playerWithBall);
    }

    void passToPlayer(GameObject playerWithBall)
    {
        GameObject ball = playerWithBall.GetComponent<MyPlayer_PVE>().ball;
        Vector3 shootingTarget;
        List<Vector3> closePlayers = new List<Vector3>();
        playerWithBall.GetComponent<Collider2D>().enabled = false;

        for (int i = 0; i < mg.myIA_Players.Length; i++)
        {
            switch (playerWithBall.transform.GetComponent<MyPlayer_PVE>().formationPos)
            {
                case formationPositions.CIERRE:
                    switch (teamStrategy)
                    {
                        case strategy.EQUILIBRATED:
                            if (Vector2.Distance(playerWithBall.transform.position, mg.myIA_Players[i].transform.position) < 9 && mg.myIA_Players[i] != playerWithBall) closePlayers.Add(mg.myIA_Players[i].transform.position);
                            break;
                        case strategy.OFFENSIVE:
                            if (i == 1) closePlayers.Insert(0, mg.myIA_Players[i].transform.position);
                            else if (i == 2) closePlayers.Insert(1, mg.myIA_Players[i].transform.position);
                            break;
                        case strategy.DEFFENSIVE:
                            if (Vector2.Distance(playerWithBall.transform.position, mg.myIA_Players[i].transform.position) < 9 && mg.myIA_Players[i] != playerWithBall) closePlayers.Add(mg.myIA_Players[i].transform.position);
                            break;
                    }
                    break;
                case formationPositions.ALA:
                    switch (teamStrategy)
                    {
                        case strategy.EQUILIBRATED:
                            if (i == 2) closePlayers.Insert(0, mg.myIA_Players[i].transform.position);
                            else if (Random.Range(1 + playerWithBall.transform.position.y * -22.5f, 101) <= 90 && Vector2.Distance(playerWithBall.transform.position, mg.myIA_Players[i].transform.position) < 9 && mg.myIA_Players[i] != playerWithBall) closePlayers.Add(mg.myIA_Players[i].transform.position);
                            break;
                        case strategy.OFFENSIVE:
                            if (i == 2) closePlayers.Insert(0, mg.myIA_Players[i].transform.position);
                            break;
                        case strategy.DEFFENSIVE:
                            if (i == 2) closePlayers.Insert(0, mg.myIA_Players[i].transform.position);
                            else if (Vector2.Distance(playerWithBall.transform.position, mg.myIA_Players[i].transform.position) < 9 && mg.myIA_Players[i] != playerWithBall) closePlayers.Add(mg.myIA_Players[i].transform.position);
                            break;
                    }
                    break;
                case formationPositions.PIVOT:
                    switch (teamStrategy)
                    {
                        case strategy.EQUILIBRATED:
                            if (i == 1) closePlayers.Insert(0, mg.myIA_Players[i].transform.position);
                            else if (Random.Range(1 + playerWithBall.transform.position.y * -22.5f, 101) <= 90 && Vector2.Distance(playerWithBall.transform.position, mg.myIA_Players[i].transform.position) < 9 && mg.myIA_Players[i] != playerWithBall) closePlayers.Add(mg.myIA_Players[i].transform.position);
                            break;
                        case strategy.OFFENSIVE:
                            if (i == 1) closePlayers.Insert(0, mg.myIA_Players[i].transform.position);
                            break;
                        case strategy.DEFFENSIVE:
                            if (Random.Range(1 + playerWithBall.transform.position.y * -22.5f, 101) <= 90 && Vector2.Distance(playerWithBall.transform.position, mg.myIA_Players[i].transform.position) < 9 && mg.myIA_Players[i] != playerWithBall) closePlayers.Add(mg.myIA_Players[i].transform.position);
                            break;
                    }
                    break;
                case formationPositions.GOALKEEPER:
                    switch (teamStrategy)
                    {
                        case strategy.DEFFENSIVE:
                        case strategy.EQUILIBRATED:
                            if (Vector2.Distance(playerWithBall.transform.position, mg.myIA_Players[i].transform.position) < 9 && mg.myIA_Players[i] != playerWithBall) closePlayers.Add(mg.myIA_Players[i].transform.position);
                            break;
                        case strategy.OFFENSIVE:
                            if (i == 1) closePlayers.Insert(0, mg.myIA_Players[i].transform.position);
                            else if (i == 2) closePlayers.Insert(1, mg.myIA_Players[i].transform.position);
                            break;
                    }
                    break;
            }
        }
        while (closePlayers.Count != 0)
        {
            shootingTarget = closePlayers[0];
            float minDist = Vector2.Distance(shootingTarget, playerWithBall.transform.position);
            if (closePlayers.Count > 1)
            {
                for (int i = 0; i < closePlayers.Count; i++)
                {
                    Debug.Log("Player " + (i + 1).ToString() + "-> " + Vector2.Distance(playerWithBall.transform.position, closePlayers[i]).ToString());
                    if (Vector2.Distance(playerWithBall.transform.position, closePlayers[i]) < minDist)
                    {
                        Debug.Log("Target change to-> " + (i + 1).ToString());
                        shootingTarget = closePlayers[i];
                        minDist = Vector2.Distance(playerWithBall.transform.position, closePlayers[i]);
                    }
                }
            }
            RaycastHit2D hit;
            Vector3 dir = shootingTarget - ball.transform.position;
            Debug.Log("Final distance-> " + Vector2.Distance(playerWithBall.transform.position, shootingTarget).ToString());
            hit = Physics2D.Raycast(ball.transform.position, dir, 10);
            if (hit && hit.transform.parent == playerWithBall.transform.parent && hit.transform.name != playerWithBall.transform.name && !hit.transform.GetComponent<MyPlayer_PVE>().covered)
            {
                Debug.Log(hit.transform.name + "of team " + hit.transform.parent.name + "selected to pass");
                Debug.Log("Shoot distance-> " + Vector2.Distance(playerWithBall.transform.position, hit.transform.position).ToString());
                playerWithBall.GetComponent<MyPlayer_PVE>().ShootBall(new float[] { hit.transform.position.x, hit.transform.position.y - 0.25f, playerWithBall.transform.position.x, playerWithBall.transform.position.y });
                closePlayers.Clear();
            }
            else
            {
                if (hit)
                {
                    Debug.Log("Unable to pass to player with distance-> " + Vector2.Distance(playerWithBall.transform.position, hit.transform.position).ToString());
                    //if (hit.transform.parent != playerWithBall.transform.parent) 
                    //Debug.Log("Reason: Target intercepted with " + hit.transform.name + "of team " +
                    //hit.transform.parent.name);
                    //else if (!hit.transform.GetComponent<MyPlayer_PVE>().covered)
                    //    Debug.Log("Reason: Target covered");
                }
                else
                {
                    Debug.Log(shootingTarget.ToString() + " not Found");
                }
                closePlayers.Remove(shootingTarget);
            }
        }
        if(playerWithBall.transform.GetComponent<MyPlayer_PVE>().ball != null)
        {

            switch (playerWithBall.transform.GetComponent<MyPlayer_PVE>().formationPos)
            {
                case formationPositions.CIERRE:
                case formationPositions.PIVOT:
                    playerWithBall.GetComponent<MyPlayer_PVE>().stablishNewShootCheck();
                    break;
                case formationPositions.ALA:
                    if(playerWithBall.GetComponent<MyPlayer_PVE>().ball.GetComponent<Ball>().inArea) shootToGoal(playerWithBall);
                    break;
                case formationPositions.GOALKEEPER:
                    Vector2 randShoot = new Vector2(Random.Range(-5.0f, 5.0f), Random.Range(-1.0f, -2.5f));
                    playerWithBall.GetComponent<MyPlayer_PVE>().ShootBall(new float[] { randShoot.x, randShoot.y, playerWithBall.transform.position.x, playerWithBall.transform.position.y });
                    break;
            }
        }
        playerWithBall.GetComponent<Collider2D>().enabled = true;
    }

    void shootToGoal(GameObject playerWithBall)
    {
        if (!playerWithBall.GetComponent<MyPlayer_PVE>().ball.GetComponent<Ball>().inArea || 
            (playerWithBall.GetComponent<MyPlayer_PVE>().ball.transform.position.y / Mathf.Abs(playerWithBall.GetComponent<MyPlayer_PVE>().ball.transform.position.y))
            != mg.myPlayers[3].transform.position.y / Mathf.Abs(mg.myPlayers[3].transform.position.y) || !mg.GameOn) return;
        Debug.Log("Shooting to goal");

        int playerIdx = 3;
        int ia_Idx = 0;
        for (int i = 0; i < mg.myIA_Players.Length; i++)
        {

            if (playerWithBall == mg.myIA_Players[i])
            {
                ia_Idx = i;
                break;
            }
        }
        mg.ChooseShoot(playerIdx, ia_Idx);

        //if(mg.myIA_Players[3].transform.position.x < 0)
        //{
        //    Vector2 randShoot = new Vector2(Random.Range(mg.myIA_Players[3].transform.position.x +0.5f,1.5f), -7.5f);
        //    playerWithBall.GetComponent<MyPlayer_PVE>().ShootBall(new float[] { randShoot.x, randShoot.y, playerWithBall.transform.position.x, playerWithBall.transform.position.y });
        //}
        //else
        //{
        //    Vector2 randShoot = new Vector2(Random.Range(mg.myIA_Players[3].transform.position.x - 0.5f, -1.5f), -7.5f);
        //    playerWithBall.GetComponent<MyPlayer_PVE>().ShootBall(new float[] { randShoot.x, randShoot.y, playerWithBall.transform.position.x, playerWithBall.transform.position.y });
        //}

    }
}
