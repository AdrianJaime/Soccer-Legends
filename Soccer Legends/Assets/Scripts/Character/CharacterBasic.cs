using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

[System.Serializable]
public class CharacterBasic : MonoBehaviour
{
    [System.Serializable]
    public struct data
    {
        public int atk;
        public int teq;
        public int def;
        public int level;
        public bool owned;

        public data(int _atq, int _teq, int _def, int _level = 1, bool _owned = false)
        {
            atk = _atq;
            teq = _teq;
            def = _def;
            level = _level;
            owned = _owned;
        }
    }

    public CharacterInfo basicInfo;

    //info user
    public data info;
    public int levelMAX=1;
    public int currentExpAwakening = 0;
    public int currentExp=0;
    public int power=1;

    private void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://soccer-legends-db.firebaseio.com/");

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public CharacterBasic()
    {

    }

    public CharacterBasic(CharacterInfo _basicInfo, data _info, int _levelMAX = 1, int _currentExpAwakening = 0, 
        int _currentExp = 0, int _power = 1)
    {
        basicInfo = _basicInfo;
        info = _info;
        levelMAX = _levelMAX;
        currentExpAwakening = _currentExpAwakening;
        currentExp = _currentExp;
        power = _power;
    }
    public CharacterBasic(CharacterBasic _copy)
    {
        info = _copy.info;
        currentExp = _copy.currentExp;
        power = _copy.power;
       // owned = _copy.owned;
        basicInfo = _copy.basicInfo;
        levelMAX = _copy.levelMAX;
    }

    public void SaveCharacter()
    {
        //Guarda el pj en base de datos


    }
    public void LoadCharacterStats(string id)
    {
        //Recupera la info de base de datos a traves del basicInfo->ID
        string idstr;

        if (int.Parse(id) < 10) idstr = "00" + id;
        else if (int.Parse(id) < 100) idstr = "0" + id;
        else idstr = id;
        FirebaseDatabase.DefaultInstance.GetReference("card").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted) Debug.Log("F in the chat");
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string statsSTR = snapshot.Child(idstr).Child("maxSTATS").GetValue(true).ToString();
                string[] stats = statsSTR.Split('-');
                info.atk = int.Parse(stats[0]);
                info.teq = int.Parse(stats[1]);
                info.def = int.Parse(stats[2]);
                power = info.atk + info.teq + info.def;
            }
        });

                ///ALFA CHANGE
                info.owned = true;


    }
    

}

