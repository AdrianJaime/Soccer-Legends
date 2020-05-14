using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMissionsGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct EVENT_MISSIONS
    {
        public List<MissionsGenerator.BD_MISSION_DATA> missions; //como con las misiones diarias o normales
    }

    [SerializeField] List<EVENT_MISSIONS> events;
    [SerializeField] GameObject missionPrefab, inventoryPrefab;
    [SerializeField] Transform placeToSpawnInventoryMission;
    [SerializeField] MissionDetail popUpMissionScript;
    [SerializeField] DoubleScrollSync panelSyncScript;


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
        foreach (EVENT_MISSIONS eventUnit in events)
        {
            //Aqui instanciamos un nuevo iinventario para las siguientes missiones
            GameObject inventory = Instantiate(inventoryPrefab, placeToSpawnInventoryMission);
            foreach (MissionsGenerator.BD_MISSION_DATA mission in eventUnit.missions)
            {
                GameObject spawnedMission = Instantiate(missionPrefab, inventory.transform.GetChild(0)); //Es el primer gameObject de la jerarquia, si se cambia no funcionará
                MissionObject missionLogic = spawnedMission.GetComponent<MissionObject>();
                missionLogic.SetUpVariables(mission.id, mission.claim, mission.title, mission.description, mission.actualProgress, mission.maxProgress,/* mission.rewards,*/ popUpMissionScript);
            }
        }


        missionsSpawned = true;
        panelSyncScript.SetUpPannels();
    }

}
