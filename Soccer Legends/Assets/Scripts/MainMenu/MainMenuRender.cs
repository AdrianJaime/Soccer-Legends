using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuRender : MonoBehaviour
{
    [SerializeField] Image artworkLocation;
    [SerializeField] Image iconoArtwork;
    [SerializeField] Transform placeToSpawnAnimation;
    [SerializeField] CharacterInfo character;

    private void Awake()
    {

        character = StaticInfo.firstCharcter.basicInfo;
        iconoArtwork.sprite = StaticInfo.firstCharcter.basicInfo.artworkIcon;

        if (character.animation2DObject == null)
        {
            artworkLocation.sprite = character.completeArtwork;
            artworkLocation.gameObject.SetActive(true);
        }
        else
        {
            artworkLocation.gameObject.SetActive(false);
            Instantiate(character.animation2DObject, placeToSpawnAnimation);
        }
        
    }

}
