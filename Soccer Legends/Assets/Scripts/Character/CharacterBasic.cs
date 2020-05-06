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
        currentExpAwakening = _copy.currentExpAwakening;
        power = _copy.power;
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

        FirebaseDatabase.DefaultInstance.GetReference("player/2/characters/" + idstr).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted) Debug.Log("F in the chat");
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int exp = int.Parse(snapshot.Child("exp").GetValue(true).ToString());
                info.level = Mathf.RoundToInt(Mathf.Pow(exp * 5 / 4, 0.33333f));

                //Guarrada ajustar exp
                    //if (info.level > levelMAX) info.level = levelMAX;
                    //FirebaseDatabase.DefaultInstance.GetReference("player/2/characters/" + idstr + "/exp").SetValueAsync((Mathf.Pow(levelMAX, 3) * 4 / 5).ToString());
                //////////
                
                info.owned = bool.Parse(snapshot.Child("owned").GetValue(true).ToString());
            }
        });

        FirebaseDatabase.DefaultInstance.GetReference("card/" + idstr).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted) Debug.Log("F in the chat");
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string maxStatsSTR = snapshot.Child("maxSTATS").GetValue(true).ToString();
                string baseStatsSTR = snapshot.Child("baseSTATS").GetValue(true).ToString();
                string[] maxStats = maxStatsSTR.Split('-');
                string[] baseStats = baseStatsSTR.Split('-');

                info.atk = Mathf.RoundToInt(int.Parse(baseStats[0]) + (int.Parse(maxStats[0]) - int.Parse(baseStats[0])) * ((float)info.level / 100));
                info.teq = Mathf.RoundToInt(int.Parse(baseStats[1]) + (int.Parse(maxStats[1]) - int.Parse(baseStats[1])) * ((float)info.level / 100));
                info.def = Mathf.RoundToInt(int.Parse(baseStats[2]) + (int.Parse(maxStats[2]) - int.Parse(baseStats[2])) * ((float)info.level / 100));

                power = info.atk + info.teq + info.def;

                switch (snapshot.Child("rarity").GetValue(true).ToString())
                {
                    case "bronze":
                        levelMAX = 60;
                        break;
                    case "silver":
                        levelMAX = 80;
                        break;
                    case "gold":
                        levelMAX = 100;
                        break;
                }
            }
        });
        ///ALFA CHANGE
        //info.owned = true;


    }
    

}

