using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steps : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] stoneClips;
    [SerializeField]
    private AudioClip[] mudClips;
    [SerializeField]
    private AudioClip[] grassClips;

    private AudioSource audioSource;

    public int terrainIndex;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        AudioClip clip = GetRandomClip();

        audioSource.pitch = Random.Range(1f, 2f);
        audioSource.volume = Random.Range(0.7f, 1f);
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        switch (terrainIndex)
        {
            case 0:
                return stoneClips[UnityEngine.Random.Range(0, stoneClips.Length)];
            case 1:
                return mudClips[UnityEngine.Random.Range(0, mudClips.Length)];
            case 2:
            default:
                return grassClips[UnityEngine.Random.Range(0, grassClips.Length)];
        }

    }
}
