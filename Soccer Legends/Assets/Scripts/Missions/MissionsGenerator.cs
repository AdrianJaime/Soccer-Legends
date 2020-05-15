using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase;
//using Firebase.Unity.Editor;
//using Firebase.Database;

public class MissionsGenerator : MonoBehaviour
{

    /// <summary>
    /// Estos 2 structs se deberán sustituir por la infromacion que se lee de BD
    /// </summary>
    [System.Serializable]
    public struct BD_MISSION_DATA
    {
        public int actualProgress,maxProgress; //variable propia de la mision {ej. si es conseguir 100 goles en total: 100 es maxProgress y el progreso dependerá del jugador}
        public string id,title, description; // titulo como {MISION 1, MISION 2...} y descrpicion
        public bool claim; //si se ha conseguido o no la recompensa
        public List<MissionObject.OBJECT_DATA> rewards;
        private int rainbowBalls;
    }


    [SerializeField] List<BD_MISSION_DATA> missions;
    [SerializeField]GameObject missionPrefab;
    [SerializeField]Transform placeToSpawnMissions;
    [SerializeField]MissionDetail popUpMissionScript;

    public CompendiumItems objetosFlama;

    bool missionsSpawned = false;

    private void Start()
    {

        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://soccer-legends-db.firebaseio.com/");

        //DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        DownloadMissionsDB();
    }

    private void Update()
    {
        if(!missionsSpawned)
            SetUpMissions();
    }

    /// <summary>
    /// Descragamos desde BD la info de todas las misiones y las guardamos en la lista missions
    /// tener en cuenta que actualemnte la lista es de una clase determinada y que cuando se descargen los
    /// datos de BD, puede cambiar.
    /// </summary>
    void DownloadMissionsDB()
    {
        //FirebaseDatabase.DefaultInstance.GetReference("missions/normals").GetValueAsync().ContinueWith(task =>
        //{
        //    if (task.IsFaulted) Debug.Log("F in the chat");
        //    else if (task.IsCompleted)
        //    {
        //        DataSnapshot snapshot = task.Result;
        //        for (int i = 1; i <= snapshot.ChildrenCount; i++)
        //        {

        //            // Facilita lectura de base de datos.

        //            string child = "000";
        //            if (i < 10) child = "00" + i.ToString();
        //            else if (i < 100) child = "0" + i.ToString();
        //            else child = i.ToString();
                    

        //            BD_MISSION_DATA mission = new BD_MISSION_DATA();
        //            mission.title = snapshot.Child(child).Child("title").GetValue(true).ToString();
        //            mission.maxProgress = int.Parse(snapshot.Child(child).Child("maxProgress").GetValue(true).ToString());
        //            mission.description = snapshot.Child(child).Child("description").GetValue(true).ToString();
        //            mission.id = child;

        //            missions.Add(mission);
        //        }
        //    }
        //});
    }

    /// <summary>
    /// Una vez con la lista de missiones acualizada generamos las misiones 
    /// </summary>
    /// 
    void checkMissionProgress()
    {

    }
    void SetUpMissions()
    {
        foreach(BD_MISSION_DATA mission in missions)
        {
            GameObject spawnedMission = Instantiate(missionPrefab, placeToSpawnMissions);
            MissionObject missionLogic = spawnedMission.GetComponent<MissionObject>();
            //Hay que mirar los rewards
            missionLogic.SetUpVariables(mission.id,mission.claim,mission.title, mission.description, mission.actualProgress, mission.maxProgress,/* mission.rewards,*/popUpMissionScript);
        }

        if(missions.Count > 0)
            missionsSpawned = true;
    }

}
