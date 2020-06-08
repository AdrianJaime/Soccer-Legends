using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OST_MUSIC : MonoBehaviour
{
    [SerializeField] AudioClip menu, intro, normalMATCH, animationMusic;

    [SerializeField] AudioSource src;

    private void Update()
    {
        if (src.clip == intro && !src.isPlaying) { src.clip = normalMATCH; src.loop = true; src.Play(); }
    }

    private void OnLevelWasLoaded(int level)
    {
        switch(SceneManager.GetSceneByBuildIndex(level).name)
        {
            case "PlayersSelectorScene":
                if (src.clip == menu) return;
                src.Stop();
                src.clip = menu;
                src.loop = true;
                src.volume = 0.6f;
                src.Play();
                break;
            case "Match-PVE_test":
                src.Stop();
                src.clip = intro;
                src.loop = false;
                src.volume = 1.0f;
                src.Play();
                break;
            default:
                break;
        }
    }

    public void playAnimationMusic()
    {
        if (src.clip == animationMusic && src.isPlaying) return;
        src.Stop();
        src.clip = animationMusic;
        src.loop = true;
        src.Play();
    }
}
