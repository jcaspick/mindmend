using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MixType
{
    Additive,
    Subtractive
}

public class Oscillator : MonoBehaviour
{
    public double frequency = 440.0;
    private double increment;
    private double phase;
    private double sampling_frequency = 48000.0;

    public bool sine;
    public bool square;
    public bool triangle;
    public bool saw;
    public bool saw2;
    private float[] wavetable = new float[5];

    public float gain = 0;
    public float volume = 0.1f;

    public double[] frequencies;
    public int freqIndex;

    public MixType mixType;

    private void Awake() {
        frequencies = NoteUtility.notes;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.E)) {
            gain = volume;
            frequency = frequencies[freqIndex];

            freqIndex++;
            freqIndex = freqIndex % frequencies.Length;
        }
        if (Input.GetKeyUp(KeyCode.E)) {
            gain = 0;
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0 * (Mathf.PI / sampling_frequency);

        for (int i = 0; i < data.Length; i += channels) {
            phase += increment;

            if (sine) {
                wavetable[0] = Sine(phase);
            }
            if (square) {
                wavetable[1] = Square(phase);
            }
            if (triangle) {
                wavetable[2] = Triangle(phase);
            }
            if (saw) {
                wavetable[3] = Saw(phase);
            }
            if (saw2) {
                wavetable[4] = Saw2(phase);
            }

            foreach(float wave in wavetable) {
                switch (mixType) {
                    case MixType.Additive:
                        data[i] += wave;
                        break;
                    case MixType.Subtractive:
                        data[i] -= wave;
                        break;
                    default:
                        break;
                }
            }

            if (channels == 2) {
                data[i + 1] = data[i];
            }

            if (phase > Mathf.PI * 2) {
                phase = 0.0;
            }
        }
    }

    private float Sine(double phase) {
        return gain * Mathf.Sin((float)phase);
    }

    private float Square(double phase) {
        return gain * Mathf.Sign(Mathf.Sin((float)phase));
    }

    private float Triangle(double phase) {
        return gain * (Mathf.PingPong((float)phase, 1.0f) - 0.5f) * 2.0f;
    }

    private float Saw(double phase) {
        return gain * (2.0f * ((float)phase - (float)Mathf.Floor((float)phase + 0.5f)));
    }

    private float Saw2(double phase) {
        // has a pop at initialization
        return gain * ((float)phase % 2.0f) - 1.0f;
    }
}

