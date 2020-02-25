using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    private GameObject sourceObj;
    private AudioSource source;
    public AudioClip soundEffect;

    public bool loop;
    public float volume = 1.0f;

    public void PlaySoundFX() {
        sourceObj = Instantiate(Resources.Load("SoundFX")) as GameObject;
        sourceObj.transform.parent = gameObject.transform;

        source = sourceObj.GetComponent<AudioSource>();
        source.loop = loop;
        source.volume = volume;

        source.PlayOneShot(soundEffect);
    }

    // Update is called once per frame
    void Update()
    {
        if (source && !source.isPlaying) {
            source = null;
            Destroy(sourceObj);
        }
    }
}
