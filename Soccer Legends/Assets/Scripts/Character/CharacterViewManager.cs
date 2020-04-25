using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class CharacterViewManager : MonoBehaviour
{
    private CharacterBasic actualCharacter;

    [SerializeField] Text pivotStat, defenseStat, technicalStat, nameCharacter, levelCharacter, descriptionCharacter, decriptionSpecialAttack;
    [SerializeField] Image imageCharacter, imageFillNameRarity, starsImage, colorTypeElement;

    [SerializeField] ColorsScriptableObject colorsRarity,colorsType;
    [SerializeField] SpriteConpendiumSO starsRarity;

    //[SerializeField] DragonBones.UnityArmatureComponent dragonBonesPlace;
    [SerializeField] Transform placeToSpawnAnimation;

    // Start is called before the first frame update
    void Start()
    {
        actualCharacter = StaticInfo.characterToAcces;
        UpdateInterface();
    }


    public void UpdateInterface()
    {
        //Stats
        pivotStat.text = actualCharacter.info.atk.ToString();
        defenseStat.text = actualCharacter.info.def.ToString();
        technicalStat.text = actualCharacter.info.teq.ToString();

        //Image
        ///OLD 
        ///FRAG ABOUT DRAGONBONES APROACH
        //if (actualCharacter.basicInfo.dragonBonesData == null)
        //{
        //    imageCharacter.sprite = actualCharacter.basicInfo.completeArtwork;
        //    imageCharacter.gameObject.SetActive(true);
        //    dragonBonesPlace.gameObject.SetActive(false);
        //}
        //else
        //{
        //    ReloadDBDataByAlejandro();
        //    imageCharacter.gameObject.SetActive(false);
        //    dragonBonesPlace.gameObject.SetActive(true);
        //}
        ///NEW
        if (actualCharacter.basicInfo.animation2DObject == null)
        {
            imageCharacter.sprite = actualCharacter.basicInfo.completeArtwork;
            imageCharacter.gameObject.SetActive(true);
        }
        else
        {
            //ReloadDBDataByAlejandro();
            imageCharacter.gameObject.SetActive(false);
            Instantiate(actualCharacter.basicInfo.animation2DObject, placeToSpawnAnimation);
        }



        //Descriptions
        descriptionCharacter.text = actualCharacter.basicInfo.description;
        //decriptionSpecialAttack.text = actualCharacter.basicInfo.specialAttackInfo.name;

        //Name and display
        nameCharacter.text = actualCharacter.basicInfo.nameCharacter;
        levelCharacter.text = "Lvl " + actualCharacter.info.level.ToString();
        starsImage.sprite = starsRarity.sprites[(int)actualCharacter.basicInfo.rarity];
        imageFillNameRarity.color = colorsRarity.colors[(int)actualCharacter.basicInfo.rarity];
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(StaticInfo.previousScene);
    }

    //public static void ChangeArmatureData(UnityArmatureComponent _armatureComponent, string armatureName, string dragonBonesName)
    //{
    //    bool isUGUI = _armatureComponent.isUGUI;
    //    UnityDragonBonesData unityData = null;
    //    Slot slot = null;
    //    if (_armatureComponent.armature != null)
    //    {
    //        unityData = _armatureComponent.unityData;
    //        slot = _armatureComponent.armature.parent;
    //        _armatureComponent.Dispose(false);

    //        UnityFactory.factory._dragonBones.AdvanceTime(0.0f);

    //        _armatureComponent.unityData = unityData;
    //    }

    //    _armatureComponent.armatureName = armatureName;
    //    _armatureComponent.isUGUI = isUGUI;

    //    _armatureComponent = UnityFactory.factory.BuildArmatureComponent(_armatureComponent.armatureName, dragonBonesName, null, _armatureComponent.unityData.dataName, _armatureComponent.gameObject, _armatureComponent.isUGUI);
    //    if (slot != null)
    //    {
    //        slot.childArmature = _armatureComponent.armature;
    //    }

    //    _armatureComponent.sortingLayerName = _armatureComponent.sortingLayerName;
    //    _armatureComponent.sortingOrder = _armatureComponent.sortingOrder;
    //}

    //public  bool ChangeDragonBonesData(UnityArmatureComponent _armatureComponent, TextAsset dragonBoneJSON)
    //{
    //    if (dragonBoneJSON != null)
    //    {
    //        var textureAtlasJSONs = new List<string>();
    //       // GetTextureAtlasConfigs(textureAtlasJSONs, AssetDatabase.GetAssetPath(dragonBoneJSON.GetInstanceID()));

    //        //UnityDragonBonesData.TextureAtlas[] textureAtlas = GetTextureAtlasByJSONs(textureAtlasJSONs);

    //        //UnityDragonBonesData data = CreateUnityDragonBonesData(dragonBoneJSON, textureAtlas);
    //        //UnityDragonBonesData data =  dragonBonesPlace.unityData;
    //        _armatureComponent.unityData = dragonBonesPlace.unityData;

    //        var dragonBonesData = UnityFactory.factory.LoadData(dragonBonesPlace.unityData, true);
    //        if (dragonBonesData != null)
    //        {
    //            _armatureComponent.unityData = dragonBonesPlace.unityData;

    //           // var armatureName = dragonBonesData.armatureNames[0];
    //            var armatureName = _armatureComponent.armatureName;
    //            ChangeArmatureData(_armatureComponent, armatureName, _armatureComponent.unityData.dataName);

    //            _armatureComponent.gameObject.name = armatureName;


    //            return true;
    //        }

    //    }
    //    else if (_armatureComponent.unityData != null)
    //    {
    //        _armatureComponent.unityData = null;

    //        if (_armatureComponent.armature != null)
    //        {
    //            _armatureComponent.Dispose(false);
    //        }
    //        return true;
    //    }

    //    return false;
    //}
    //public void ReloadDBDataByAlejandro()
    //{
    //    //clear cache
    //    UnityFactory.factory.Clear(true);
    //    dragonBonesPlace.unityData = actualCharacter.basicInfo.dragonBonesData;

    //    if (ChangeDragonBonesData(dragonBonesPlace, dragonBonesPlace.unityData.dragonBonesJSON))
    //    {
    //        // dragonBonesPlace.CollectBones();
    //    }
    //    dragonBonesPlace.animation.Play();

    //}

}
