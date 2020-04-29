using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RewardsManager : MonoBehaviour
{
    Animator anim;
    Vector2[] swipes;
    [SerializeField]
    Transform rewardsPanel;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < StaticInfo.teamSelectedToPlay.Count; i++)
            rewardsPanel.GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite =
                StaticInfo.teamSelectedToPlay[i].basicInfo.artworkResult;
        anim = GetComponent<Animator>();
        swipes = new Vector2[2];
    }

    // Update is called once per frame
    void Update()
    {
        Touch swipe;
        if (Input.touchCount > 0)
        {
            swipe = Input.GetTouch(0);
            if (swipe.phase == TouchPhase.Began)
            {
                swipes[0] = swipe.position;
            }
            else if (swipe.phase == TouchPhase.Ended)
            {
                swipes[1] = swipe.position;
                if (swipes[0].y < swipes[1].y)
                {
                    anim.SetTrigger("SlideUp");
                    Invoke("loadMainMenu", 1.0f);
                }
            }
        }
    }

    void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
