using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using System;
using UnityEngine.UI;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    // Start is called before the first frame update

    public int stamina, maxSta;
    public bool info = false;
    DateTime lastRestore;
    string nameStr, coinStr, ballStr, rankStr;
    public TextMeshProUGUI timeLeft, progress, playerName, coin, ball, rank;
    public Slider staSlider;

    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://soccer-legends-db.firebaseio.com/");

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {

        if (info)
        {
            TimeSpan aux = DateTime.Now.Subtract(lastRestore);
            while (aux.TotalMinutes > 5 && stamina < maxSta)
            {
                stamina++;
                lastRestore = lastRestore.AddMinutes(5);
                FirebaseDatabase.DefaultInstance.GetReference("player/2/lastRestore").SetValueAsync(lastRestore.ToString());
                FirebaseDatabase.DefaultInstance.GetReference("player/2/sta").SetValueAsync(stamina.ToString());
                aux = DateTime.Now.Subtract(lastRestore);
            }
            timeLeft.text = (Math.Truncate(aux.TotalMilliseconds / 60000 * 100) / 100).ToString();
            TimeSpan five = new TimeSpan(0, 5, 0);
            String timeleftString = "";
            if (five.Subtract(aux).Minutes < maxSta)
            {
                if (five.Subtract(aux).Seconds < 10) timeleftString = "0" + five.Subtract(aux).Minutes.ToString() + ":" + "0" + five.Subtract(aux).Seconds.ToString();
                else timeleftString = "0" + five.Subtract(aux).Minutes.ToString() + ":" + five.Subtract(aux).Seconds.ToString();
            }
            else if (five.Subtract(aux).Seconds < 10) timeleftString = five.Subtract(aux).Minutes.ToString() + ":" + "0" + five.Subtract(aux).Seconds.ToString();

            if (stamina == 10) timeLeft.text = "--:--";
            else timeLeft.text = timeleftString;
            progress.text = stamina.ToString() + "/" + maxSta.ToString();
            staSlider.value = stamina;

            if (playerName.text == "") playerName.text = nameStr;
            if (coin.text == "") coin.text = coinStr;
            if (ball.text == "") ball.text = ballStr;
            if (rank.text == "") rank.text = rankStr;

        }
        else
        {
            FirebaseDatabase.DefaultInstance.GetReference("player/2").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted) Debug.Log("F in the chat");
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    stamina = int.Parse(snapshot.Child("sta").GetValue(true).ToString());
                    lastRestore = Convert.ToDateTime(snapshot.Child("lastRestore").GetValue(true).ToString());
                    staSlider.maxValue = maxSta;

                    nameStr = snapshot.Child("name").GetValue(true).ToString();

                    coinStr = snapshot.Child("coins").GetValue(true).ToString();

                    ballStr = snapshot.Child("balls").GetValue(true).ToString();

                    rankStr = snapshot.Child("rank").GetValue(true).ToString();

                    FirebaseDatabase.DefaultInstance.GetReference("rankConditions/" + rankStr).GetValueAsync().ContinueWith(task2 =>
                    {
                        if (task2.IsFaulted) Debug.Log("F in the chat");
                        else if (task2.IsCompleted)
                        {
                            DataSnapshot snapshot2 = task.Result;
                            maxSta = int.Parse(snapshot2.Child("maxSta").GetValue(true).ToString());
                        }
                    });

                    info = true;
                }
            });

        }
    }
}
