using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase;
//using Firebase.Unity.Editor;
//using Firebase.Database;

public class ObjectsInventory : MonoBehaviour
{
    //public Transform locationSpawn;
    //public GameObject inventoryPrefabUnit;

    //public List<GameObject> listOfInteractuable = new List<GameObject>();//list of gameobject-objects

    ////public List<> listRealDataConsum = new List<>(); // Donde guardaremos al principio de todo lo leido de la base de datos

    //public List<ConsumBaseInfo> listOfConsum = new List<ConsumBaseInfo>(); //La informacion de todos los objetos que existen

    //// Start is called before the first frame update
    //void Start()
    //{
    //    //1. Leemos la base de datos y guardamos los datos del los consumibles actuales del juegador en listRealDataConsum
    //    //
    //    FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://soccer-legends-db.firebaseio.com/");

    //    DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    //    //2. Creamos los slots con la info previa
    //    //
    //    CreateObjects();

    //    //3. Actualizamos la interfaz de usuario con los datos reales.
    //    //
    //    UpdateUI();
    //}


    //void CreateObjects()
    //{
    //    foreach(ConsumBaseInfo consum in listOfConsum)
    //    {
    //        if (consum != null)
    //        {
    //            //vamos a crear un objeto "SlotConsum" sin ningun tipo de información 
    //            GameObject actualObject = Instantiate(inventoryPrefabUnit, locationSpawn);
    //            //Le asignamos la info basica del objeto (baseConsumInfo)
    //            actualObject.GetComponent<SlotConsum>().consumBaseInfo = consum;
    //            //Con los datos guardados previamente sobre el jugador actualizamos segun el ID de consum la cantidad que dispone el jugador
    //            actualObject.GetComponent<SlotConsum>().quantity = 24;//linea de prueba

    //            //lo añadimos para tnerlos guardados en una variable
    //            listOfInteractuable.Add(actualObject);
    //        }
    //    }
    //}

    ////Update all slots in listOfInteractuable
    //void UpdateUI()
    //{
    //    foreach (GameObject slot in listOfInteractuable)
    //    {
    //        slot.GetComponent<SlotConsum>().UpdateUI();
    //    }
    //}

    //public void ConfirmSelection()
    //{
    //    foreach (GameObject slot in listOfInteractuable)
    //    {
    //        slot.GetComponent<SlotConsum>().Confirm();
    //    }
    //}
    //public void ResetSelection()
    //{
    //    foreach (GameObject slot in listOfInteractuable)
    //    {
    //        slot.GetComponent<SlotConsum>().ResetConsum();
    //    }
    //}
}
