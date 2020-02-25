using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAudio : MonoBehaviour
{
    private Synth synth;
    public List<Voice> goalReached;

    private SoundFX fx;

    void Awake() {
        synth = GetComponent<Synth>();
        fx = GetComponent<SoundFX>();
    }

    public void Achieve() {
        if (GameSettings.instance.AUDIO_SYNTH_MODE) {
            synth.enabled = true;
            synth.PlayVoices(goalReached);

        }

        fx.PlaySoundFX();
    }
}
