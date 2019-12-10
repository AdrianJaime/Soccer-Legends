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
        Vector2 IA_ObjectivePos;
        switch (ia_State)
        {
            case IA_State.FREE_BALL:
                for(int i = 0; i < ia_players.Length; i++)
                {
                    if(ia_players[i].GetComponent<IA_Player_PVE>().ia_formationPos == formationPositions.PIVOT)
                    {
                        IA_ObjectivePos = new Vector2(ballPos.x + ia_players[i].GetComponent<IA_Player_PVE>().startPosition.x,
                            ballPos.y + ia_players[i].GetComponent<IA_Player_PVE>().startPosition.y) / 2.0f;
                    }
                    else
                    {
                        IA_ObjectivePos = ballPos;
                    }
                    ia_players[i].GetComponent<IA_Player_PVE>().MoveTo(new float[] { IA_ObjectivePos.x, IA_ObjectivePos.y, 0.0f });
                }
                break;
            case IA_State.PLAYER_HAS_BALL:
                break;
            case IA_State.IA_HAS_BALL:
                break;
            default:
                break;
        }
    }
}
