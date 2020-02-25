using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Waveform
{
    Sine,
    Square,
    Tri,
    SawDigital,
    SawAnalog,
    Invert,
    Noise
}

public class Oscillator : MonoBehaviour
{
    private double sampling_frequency = 48000.0;

    public double GetOscillator(Waveform wave, double phase) {
        double amp;

        switch (wave) {
            case Waveform.Sine:
                amp = Sine(phase);
                break;
            case Waveform.Square:
                amp = Square(phase);
                break;
            case Waveform.Tri:
                amp = Triangle(phase);
                break;
            case Waveform.SawDigital:
                amp = SawDigital(phase);
                break;
            case Waveform.SawAnalog:
                amp = SawAnalog(phase);
                break;
            case Waveform.Invert:
                amp = Invert(phase);
                break;
            case Waveform.Noise:
                amp = Noise(phase);
                break;
            default:
                amp = Sine(phase);
                break;
        }
        return amp;
    }

    private float Sine(double phase) {
        return Mathf.Sin((float)phase);
    }

    private float Square(double phase) {
        return Mathf.Sign(Mathf.Sin((float)phase));
    }

    private float Triangle(double phase) {
        return (Mathf.PingPong((float)phase, 1.0f) - 0.5f) * 2.0f;
    }

    private float SawDigital(double phase) {
        return (2.0f * ((float)phase - (float)Mathf.Floor((float)phase + 0.5f)));
    }

    private float SawAnalog(double phase) {
        float output = 0.0f;

        for (float n = 1.0f; n < 40.0; n++) {
            output += Mathf.Sin((float)phase * n) / n;
        }
        return output;
    }

    private float Invert(double phase) {
        return Mathf.Sin((float)phase + Mathf.Sin((float)phase));
    }

    private float Noise(double phase) {
        return (float)phase * Mathf.PerlinNoise(0.0f, 0.0f);
    }
}
