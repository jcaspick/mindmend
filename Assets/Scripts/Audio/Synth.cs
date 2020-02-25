using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synth : MonoBehaviour
{
    private List<Voice> voices = new List<Voice>();
    public List<Voice> defaultVoices = new List<Voice>();
    private bool playing;

    private AudioSource source;

    public double frequency = 440.0;
    private double increment;
    private double phase;
    private double lfo;
    private double sampling_frequency = 48000.0;

    public bool debug;

    public void Start() {
        sampling_frequency = AudioSettings.outputSampleRate;

        PlayVoices();
    }

    public void Update() {
        if (!IsSinging())
            StopVoices();
    }

    public void PlayVoices(List<Voice> voicesToPlay = null) {
        if (playing)
            return;

        if (!source) {
            source = gameObject.AddComponent<AudioSource>();
            source.Play();
            playing = true;
        }

        if (voicesToPlay == null)
            voicesToPlay = defaultVoices;

        foreach (Voice voiceToPlay in voicesToPlay) {
            Voice voice = Instantiate(voiceToPlay);
            voice.transform.parent = gameObject.transform;
            voice.transform.localPosition = new Vector3(0, 0, 0);

            voice.gain = 0.0f;
            double note = NoteUtility.GetNoteForPosition(gameObject.transform.position.x, gameObject.transform.position.y);
            frequency = note;

            voices.Add(voice);

            if (debug) { Debug.Log("Synth at: " + voice.transform.position.x + ", " + voice.transform.position.y + " is playing at " + frequency); }
        }
    }

    public void StopVoices() {
        if (!playing)
            return;

        foreach (Voice voice in voices) {
            voice.gain = 0.0f;

            if (debug) { Debug.Log("Stopping voices at: " + gameObject.transform.position.x + " " + gameObject.transform.position.y); }
        }
        playing = false;
        GetComponent<AudioSource>().Stop();
        Destroy(source);
        enabled = false;
    }
    // Unity has a 32 AudioSource limit, so the above code creates and destroys Sources on the fly.
    // And there's a Uniy bug where the script that calls OnAudioFilterRead must be "reset" (toggle enabled)
    // between dynamic add/remove AudioSource component, to feed filter again.


    void OnAudioFilterRead(float[] data, int channels) {
        increment = (frequency * 2.0 * Mathf.PI) / sampling_frequency;

        for (int i = 0; i < data.Length; i += channels) {
            phase += increment;

            foreach(Voice voice in voices) {
                lfo += (voice.lfoFreq * 2.0 * Mathf.PI) / sampling_frequency;

                data[i] += (float)voice.GetVoice((float)phase, (float)lfo);
            }

            if (channels == 2) {
                data[i + 1] = data[i];
            }

            if (phase > Mathf.PI * 2) {
                phase = 0.0;
            }
        }
    }

    public bool IsSinging() {
        bool singing = false;

        foreach(Voice voice in voices) {
            if(voice.singing) { singing = true; }
        }
        return singing;
    }
}
