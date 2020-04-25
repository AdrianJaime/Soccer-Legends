using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class Generator_SO :MonoBehaviour
{

    [SerializeField] Image art;

    [System.Serializable]
    public class datagenerated
    {
        public string ID;
        public int index;
        public string nameCharacter;
        public string description;

        public Rarity rarity;
        public Type type;

        //info drawable
        public Sprite artworkIcon,
                      completeArtwork,
                      artworkConforntation,
                      artworkSelectorIcon,
                      artworkResult;

        //info special attack
        public SpecialAttackInfo specialAttackInfo;

        public RuntimeAnimatorController animator_character;
    }

     [SerializeField] datagenerated data;

    public bool cretae = false;
    //public  void CreateAsset()
    //{
    //    CharacterInfo aux = AssetDatabase.LoadAssetAtPath < CharacterInfo > (ScriptableObjectUtility.CreateCharacter(data));
    //    if (aux == null)
    //    {
    //        Debug.Log("No he encontrado el asset");
    //    }
    //    else
    //    {
    //        Debug.Log("He encontrado el asset");
    //        art.sprite = aux.completeArtwork;
    //    }
    //}

    //private void Start()
    //{

    //    CharacterInfo aux=AssetDatabase.LoadAssetAtPath<CharacterInfo > ("Assets/New CharacterInfo.asset");

    //    if(aux==null)
    //    {
    //        Debug.Log("No he encontrado el asset");
    //    }
    //    else
    //    {
    //        Debug.Log("He encontrado el asset");
    //        art.sprite = aux.completeArtwork;
    //    }
    //}


}


