using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeAudio : MonoBehaviour
{
    public Oscillator oscillator;
    public bool debug;

    // Start is called before the first frame update
    void Start()
    {
        oscillator = Instantiate(oscillator);
        oscillator.gameObject.transform.parent = gameObject.transform;
        oscillator.gameObject.transform.position = gameObject.transform.position;

        oscillator.gain = 0;
        oscillator.frequency = NoteUtility.GetNoteForPosition(gameObject.transform.position.x, gameObject.transform.position.y);
    }

    public void Active() {
        if (GameSettings.instance.AUDIO_SYNTH_MODE) {
            oscillator.GetComponent<AudioSource>().Play();
            oscillator.gain = oscillator.volume;
        }

        if(debug) {
            Debug.Log("Oscillator at: " + oscillator.transform.position.x + ", " + oscillator.transform.position.y + " is playing at " + oscillator.frequency);
        }
    }

    public void Inactive() {
        if (GameSettings.instance.AUDIO_SYNTH_MODE) {
            oscillator.GetComponent<AudioSource>().Stop();
            oscillator.gain = 0.0f;
        }
    }
}
