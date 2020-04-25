using UnityEngine;
using UnityEditor;
using System.IO;
 
public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    //public static void CreateAsset<T>() where T : ScriptableObject
    //{
    //    T asset = ScriptableObject.CreateInstance<T>();
        

    //    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //    if (path == "")
    //    {
    //        path = "Assets";
    //    }
    //    else if (Path.GetExtension(path) != "")
    //    {
    //        path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    //    }

    //    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

    //    AssetDatabase.CreateAsset(asset, assetPathAndName);

    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh();
    //    EditorUtility.FocusProjectWindow();
    //    Selection.activeObject = asset;
    //}

    //public static string CreateCharacter(Generator_SO.datagenerated data)
    //{
    //    CharacterInfo character = ScriptableObject.CreateInstance<CharacterInfo>();
    //    //character.SetDirty();
    //    character.nameCharacter = data.nameCharacter;
    //    character.index = data.index;
    //    character.description = data.nameCharacter;
    //    character.rarity = data.rarity;
    //    character.type = data.type;
    //    character.animator_character = data.animator_character;
    //    character.artworkConforntation = data.artworkConforntation;
    //    character.artworkIcon = data.artworkIcon;
    //    character.artworkResult = data.artworkResult;
    //    character.artworkSelectorIcon = data.artworkSelectorIcon;
    //    character.completeArtwork = data.completeArtwork;
    //    character.specialAttackInfo = data.specialAttackInfo;
    //    //character.SetDirty();

    //    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //    if (path == "")
    //    {
    //        path = "Assets";
    //    }
    //    else if (Path.GetExtension(path) != "")
    //    {
    //        path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    //    }

    //    //string assetPathAndName = Application.persistentDataPath + typeof(CharacterInfo).ToString() + ".asset");

    //    //AssetDatabase.CreateAsset(character, assetPathAndName);

    //    //AssetDatabase.SaveAssets();
    //    //AssetDatabase.Refresh();
    //    //EditorUtility.FocusProjectWindow();
    //    //Selection.activeObject = character;

    //    //return assetPathAndName;
    //}
}
