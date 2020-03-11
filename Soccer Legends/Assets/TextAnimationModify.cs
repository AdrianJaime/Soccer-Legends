using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextAnimationModify : MonoBehaviour
{
    [SerializeField] bool doubleChange;

    [SerializeField] TextModify[] texts;



    [System.Serializable]
    struct TextModify
    {
        public Text textsToModify;
        public string textsChanges;
        public Color colors;

    }

    public void ChangeText(int index)
    {
        texts[index].textsToModify.text = texts[index].textsChanges;
        texts[index].textsToModify.color = texts[index].colors;
    }


}
