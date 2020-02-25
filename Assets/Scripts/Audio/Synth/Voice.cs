using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voice : MonoBehaviour
{
    public Oscillator oscillator;
    public Waveform waveform;
    public Envelope env;
    private double wave;

    public float gain = 0;
    public float volume = 0.1f;

    public Waveform lfoShape;
    public double lfoFreq;
    public float lfoAmp = 1.0f;

    private double sampling_frequency;

    public bool singing;

    private void Start() {
        sampling_frequency = AudioSettings.outputSampleRate;
        env = gameObject.GetComponent<Envelope>();

        TriggerNote();
    }

    public void TriggerNote() {
        gain = volume;

        env.NoteOn(AudioSettings.dspTime);
        Invoke("ReleaseNote", env.noteHold);

        singing = true;
    }

    void ReleaseNote() {
        env.NoteOff(AudioSettings.dspTime);
    }

    public double GetVoice(double phase, double lfoPhase) {

        if (env) {
            // LFO/Frequency Modulation
            if (lfoFreq > 0) {
                lfoPhase += lfoFreq * 2.0 * Mathf.PI / sampling_frequency;

                phase = phase + lfoAmp * (float)oscillator.GetOscillator(lfoShape, lfoPhase);
            }

            // Oscillator
            wave = gain * (float)oscillator.GetOscillator(waveform, phase);

            // Envelope
            wave = wave * (float)env.Filter(AudioSettings.dspTime);

            if (!env.active)
                singing = false;
        } else {
            wave = 0;
        }

        return wave;
    }
}

