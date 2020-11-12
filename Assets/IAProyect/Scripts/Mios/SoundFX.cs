using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    AudioSource miaudio = default;
    [SerializeField] AudioClip[] audioClips = default;
    public static SoundFX miSFX = default;

    private void Awake()
    {
        miSFX = this;
        miaudio = GetComponent<AudioSource>();
    }
    public void PlaySFX(int nSound)
    {
        miaudio.PlayOneShot(audioClips[nSound]);
    }

    public void playWithVariation(int nSound)
    {
        miaudio.pitch = Random.Range(-1, 2);
        miaudio.PlayOneShot(audioClips[nSound]);
        miaudio.pitch = 1;
    }
}
