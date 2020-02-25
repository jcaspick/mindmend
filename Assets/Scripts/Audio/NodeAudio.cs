using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeAudio : MonoBehaviour
{
    private Synth synth;

    public Voice nodePluck;
    public Voice nodeSustain;

    void Awake()
    {
        synth = gameObject.GetComponent<Synth>();    
    }

    public void Pluck() {
        if (GameSettings.instance.AUDIO_SYNTH_MODE) {
            synth.enabled = true;
            synth.PlayVoices(new List<Voice> { nodePluck });
        }
    }

    public void Sustain() {
        if (GameSettings.instance.AUDIO_SYNTH_MODE) {
            synth.enabled = true;
            synth.PlayVoices(new List<Voice> { nodePluck, nodeSustain });

        }
    }

    public void Inactive() {
        if (GameSettings.instance.AUDIO_SYNTH_MODE) {
            synth.StopVoices();
            synth.enabled = false;
        }
    }
}
