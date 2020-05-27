using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//using Firebase;
//using Firebase.Unity.Editor;
//using Firebase.Database;

public class RewardsManager : MonoBehaviour
{
    Animator anim;
    Vector2[] swipes;
    [SerializeField]
    Transform rewardsPanel;
    // Start is called before the first frame update
    void Start()
    {
        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://soccer-legends-db.firebaseio.com/");

        //DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        //for (int i = 0; i < StaticInfo.teamSelectedToPlay.Count; i++)
        //{
        //    rewardsPanel.GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite =
        //        StaticInfo.teamSelectedToPlay[i].basicInfo.artworkResult;
        //    string child = "000";
        //    if (i < 10) child = "00" + i.ToString();
        //    else if (i < 100) child = "0" + i.ToString();
        //    else child = i.ToString();

        //    if (StaticInfo.teamSelectedToPlay[i].levelMAX != StaticInfo.teamSelectedToPlay[i].info.level)
        //    {
        //        StaticInfo.teamSelectedToPlay[i].currentExp += 300; //CANTIDAD EXP HARDCODED
        //        FirebaseDatabase.DefaultInstance.GetReference("player/2/characters/" + child + "/exp").SetValueAsync(StaticInfo.teamSelectedToPlay[i].currentExp.ToString());
        //    }

        //}

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
        SceneManager.LoadScene("MainMenu_scene");
    }
}
