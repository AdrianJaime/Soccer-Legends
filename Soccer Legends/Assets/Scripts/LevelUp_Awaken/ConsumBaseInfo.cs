
using UnityEngine;

public enum TypeConsum { TRAINING, AWAKEN,CONSUM }


[System.Serializable]
[CreateAssetMenu(fileName = "New_ConsumInfo", menuName = "Consum/NewConsum")]

public class ConsumBaseInfo : ScriptableObject
{

    //info Consum
    public string ID;
    public new string name; //new porque ya existe la variable name en un objeto de base
    public string description;
    public int expReward;

    public TypeConsum type;

    //info drawable
    public Sprite image;



}