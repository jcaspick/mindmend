using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Envelope : MonoBehaviour
{
    public float attack;
    public float decay;
    public float sustain;
    public float release;
    public float noteHold;

    public float startAmp;
    public bool noteOn;
    public double triggerOnTime;
    public double triggerOffTime;

    public bool active;

    public void NoteOn(double timeOn) {
        triggerOnTime = timeOn;
        noteOn = true;
        active = true;
    }

    public void NoteOff(double timeOff) {
        triggerOffTime = timeOff;
        noteOn = false;
    }

    /*public double Filter(double time) {
        double amp = 0.00;
        double lifeTime = time - triggerOnTime;

        if(noteOn) {
            if (lifeTime <= attack) {
                // Attack
                amp = (lifeTime / attack) * startAmp;
            }
            if(lifeTime > attack && lifeTime <= (attack + decay)) {
                // Decay
                amp = ((lifeTime - attack) / decay) * (sustain - startAmp) + startAmp;
            }
            if(lifeTime > attack + decay) {
                // Sustain
                amp = sustain;
            }
        } else {
            // Release
            amp = ((time - triggerOffTime) / release) * (0.0 - sustain) + sustain;
        }

        if (amp <= 0.0001)
            amp = 0.0;

        return amp;
    }*/

    public double Filter(double time) {
        double amp = 0.00;
        double releaseAmp = 0.00;

        if(triggerOnTime > triggerOffTime) {
            double lifeTime = time - triggerOnTime;

            if (lifeTime <= attack) {
                amp = Attack(lifeTime);
            }
            if(lifeTime > attack && lifeTime <= (attack + decay)) {
                amp = Decay(lifeTime);
            }
            if(lifeTime > attack + decay) {
                amp = Sustain();
            }
        } else {
            double lifeTime = triggerOffTime - triggerOnTime;

            if (lifeTime <= attack) {
                releaseAmp = Attack(lifeTime);
            }
            if (lifeTime > attack && lifeTime <= (attack + decay)) {
                releaseAmp = Decay(lifeTime);
            }
            if (lifeTime > attack + decay) {
                releaseAmp = Sustain();
            }

            amp = Release(time) * (0.0 - releaseAmp) + releaseAmp;
        }

        if (time - triggerOnTime > noteHold + decay + release) {
            active = false;
        }

        if (amp <= 0.0001)
            amp = 0.0;

        return amp;
    }

    private double Attack(double lifeTime) {
        return (lifeTime / attack) * startAmp;
    }

    private double Decay(double lifeTime) {
        return ((lifeTime - attack) / decay) * (sustain - startAmp) + startAmp;
    }

    private double Sustain() {
        return sustain;
    }

    private double Release(double time) {
        return (time - triggerOffTime) / release;
    }

}
