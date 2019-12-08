using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;

public class DBManager : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GetDB()
    {
        RestClient.Get<User>("https://soccer-legends-d86c7.firebaseio.com/-LvaWZe8QhmQSlQJ8Njp.json").Then(response =>
        {
            text.text = response.userName;
        });
    }
    public void PostDB()
    {
        User user = new User("Player1", 10);
        RestClient.Put("https://soccer-legends-d86c7.firebaseio.com/"+user.userName+".json", user);
    }
}
