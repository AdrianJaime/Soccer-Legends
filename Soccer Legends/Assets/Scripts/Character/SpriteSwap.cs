using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwap : MonoBehaviour
{
    public string SpriteSheetName;
    private string LoadedSpriteSheetName;
    private SpriteRenderer spriteRenderer;

    // The dictionary containing all the sliced up sprites in the sprite sheet
    private Dictionary<string, Sprite> spriteSheet=new Dictionary<string, Sprite>();


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SpriteSheetName = spriteRenderer.sprite.name;

        LoadSpriteSheet();
    }

    // Runs after the animation has done its work
    private void LateUpdate()
    {
        //// Check if the sprite sheet name has changed (possibly manually in the inspector)
        if (LoadedSpriteSheetName != SpriteSheetName)
        {
            LoadSpriteSheet();
        }

        // Swap out the sprite to be rendered by its name
        // Important: The name of the sprite must be the same!
        spriteRenderer.sprite = spriteSheet[spriteRenderer.sprite.name];
    }

    // Loads the sprites from a sprite sheet
    private void LoadSpriteSheet()
    {
        // Load the sprites from a sprite sheet file (png). 
        // Note: The file specified must exist in a folder named Resources
        Sprite[] sprites = Resources.LoadAll<Sprite>("Assets/Resources");
        foreach(Sprite sprite in sprites)
        {
            spriteSheet.Add(sprite.name, sprite);
        }
        // Remember the name of the sprite sheet in case it is changed later
        LoadedSpriteSheetName = SpriteSheetName;


    }
}