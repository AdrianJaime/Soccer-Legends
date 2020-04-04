using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }


    [SerializeField] List<BD_MISSION_DATA> missions;
    [SerializeField]GameObject missionPrefab;
    [SerializeField]Transform placeToSpawnMissions;


    bool missionsSpawned = false;

    private void Start()
    {
        SetUpMissions();
    }

    /// <summary>
    /// Descragamos desde BD la info de todas las misiones y las guardamos en la lista missions
    /// tener en cuenta que actualemnte la lista es de una clase determinada y que cuando se descargen los
    /// datos de BD, puede cambiar.
    /// </summary>
    void DownloadMissionsBD()
    {

    }

    /// <summary>
    /// Una vez con la lista de missiones acualizada generamos las misiones 
    /// </summary>
    void SetUpMissions()
    {
        foreach(BD_MISSION_DATA mission in missions)
        {
            GameObject spawnedMission = Instantiate(missionPrefab, placeToSpawnMissions);
            MissionObject missionLogic=spawnedMission.GetComponent<MissionObject>();
            missionLogic.SetUpVariables(mission.id,mission.claim,mission.title, mission.description, mission.actualProgress, mission.maxProgress, mission.rewards);
        }

        missionsSpawned = true;
    }

}
