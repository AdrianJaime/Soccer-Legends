using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveSpriteAtEndAnimationSelectorAnimator : MonoBehaviour
{

    void RemoveSpriteOfImageByIndexArray(int index)
    {
        gameObject.GetComponent<Image>().sprite = null;
    }
}
