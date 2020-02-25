using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoteUtility
{
	public static double[] notes;
	private static int columns;

    public static int minFreq = 65;
    public static int maxFreq = 900;

	public static void Setup(int width, int height) {
		if (notes == null) {
			notes = new double[width * height];
			columns = width;
		}

		BuildFreqArray();
    }

	private static void BuildFreqArray() {
		string[] scale = A_pent;
		int octaves = noteFreq["C"].Length;

		List<double> freqs = new List<double>();

		for (var oct = 0; oct < octaves; oct++) {
			for (var i = 0; i < scale.Length; i++) {

				double freq = noteFreq[scale[i]][oct];

				if (freq > minFreq && freq < maxFreq) { // listenable range
					freqs.Add(freq);
				}
			}
		}

		freqs.Sort((f1, f2) => f1.CompareTo(f2)); // low to high

		for (var i = 0; i < notes.Length; i++) {
			notes[i] = freqs[i % freqs.Count];
		}
	}

	public static double GetNoteForPosition(float x, float y) {
		return notes[(int)(x + (columns * y))];
	}

	// Notes reference
	private static string[] noteRef = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };
	private static int[] spacing = new int[] { 2, 2, 1, 2, 2, 2, 1 }; // Major chord

	// Notes frequencies
	public static float[] A_freq = { 440, 494, 554, 587, 659, 740, 831 };
	public static float[] A_pentFreq = { 440, 494, 554, 659, 740 };
	public static string[] A_major = { "A", "B", "Db", "D", "E", "Gb", "Ab" };
	public static string[] A_pent = { "A", "B", "Db", "E", "Gb" };

	private static Dictionary<string, double[]> noteFreq = new Dictionary<string, double[]>() {
        { "C", new double[] { 32.70, 65.41, 130.81, 261.63, 523.25, 1046.50, 2093.00 } },
        { "Db", new double[] { 34.65, 69.30, 138.59, 277.18, 554.37, 1108.73, 2217.46 } },
        { "D", new double[] { 36.71, 73.42, 146.83, 293.66, 587.33, 1174.66, 2349.32 } },
        { "Eb", new double[] { 38.89, 77.78, 155.56, 311.13, 622.25, 1244.51, 2489.02 } },
        { "E", new double[] { 41.20, 82.41, 164.81, 329.63, 659.26, 1318.51, 2637.02 } },
        { "F", new double[] { 43.65, 87.31, 174.61, 349.23, 698.46, 1396.91, 2793.83 } },
        { "Gb", new double[] { 46.25, 92.50, 185.00, 369.99, 739.99, 1479.98, 2959.96 } },
        { "G", new double[] { 49.00, 98.00, 196.00, 392.00, 783.99, 1567.98, 3135.96 } },
        { "Ab", new double[] { 51.91, 103.83, 207.65, 415.30, 830.61, 1661.22, 3322.44 } },
        { "A", new double[] { 55.00, 110.00, 220.00, 440.00, 880.00, 1760.00, 3520.00 } },
        { "Bb", new double[] { 58.27, 116.54, 233.08, 466.16, 932.33, 1864.66, 3729.31 } },
        { "B", new double[] { 61.74, 123.47, 246.94, 493.88, 987.77, 1975.53, 3951.07 } }
     };

    // Scales and their note ratios
    private static Dictionary<string, string> scales = new Dictionary<string, string>() {
		{ "major", "1 2 3 4 5 6 7" },
		{ "lydian", "1 2 3 4# 5 6 7" },
		{ "dominant", "1 2 3 4 5 6 7b" },
		{ "dorian", "1 2 3b 4 5 6 7b" },
		{ "minor", "1 2 3b 4 5 6b 7b" }, 
		{ "phrygian", "1 2b 3b 4 5 6b 7b" },
		{ "locrian", "1 2b 3b 4 5b 6b 7b" },
		{ "melodic minor", "1 2 3b 4 5 6 7" },
		{ "melodic minor second mode", "1 2b 3b 4 5 6 7b" },
		{ "lydian augmented", "1 2 3 4# 5A 6 7" },
		{ "lydian dominant", "1 2 3 4# 5 6 7b" },
		{ "hindu", "1 2 3 4 5 6b 7b" },
		{ "locrian #2", "1 2 3b 4 5b 6b 7b" },
		{ "arabian", "1 2 3 4 5b 6b 7b" },
		{ "diminished whole tone", "1 2b 3b 3 5b 6b 7b" },
		{ "major pentatonic", "1 2 3 5 6" },
		{ "chinese", "1 3 4# 5 7" },
		{ "indian", "1 3 4 5 7b" },
		{ "minor seven flat five pentatonic", "1 3b 4 5b 7b" },
		{ "pentatonic", "1 3b 4 5 7b" },
		{ "minor six pentatonic", "1 3b 4 5 6" },
		{ "minor hexatonic", "1 2 3b 4 5 7" },
		{ "flat three pentatonic", "1 2 3b 5 6" },
		{ "flat six pentatonic", "1 2 3 5 6b" },
		{ "major flat two pentatonic", "1 2b 3 5 6" },
		{ "whole tone pentatonic", "1 3 5b 6b 7b" },
		{ "ionian pentatonic", "1 3 4 5 7" },
		{ "lydian #5 pentatonic", "1 3 4# 5A 7" },
		{ "lydian dominant pentatonic", "1 3 4# 5 7b" },
		{ "minor #7 pentatonic", "1 3b 4 5 7" },
		{ "super locrian pentatonic", "1 3b 4d 5b 7b" },
		{ "in-sen", "1 2b 4 5 7b" },
		{ "iwato", "1 2b 4 5b 7b" },
		{ "hirajoshi", "1 2 3b 5 6b" },
		{ "kumoijoshi", "1 2b 4 5 6b" },
		{ "pelog", "1 2b 3b 5 6b" },
		{ "vietnamese 1", "1 3b 4 5 6b" },
		{ "vietnamese 2", "1 3b 4 5 7b" },
		{ "prometheus", "1 2 3 4# 6 7b" },
		{ "prometheus neopolitan", "1 2b 3 4# 6 7b" },
		{ "ritusen", "1 2 4 5 6" },
		{ "scriabin", "1 2b 3 5 6" },
		{ "piongio", "1 2 4 5 6 7b" },
		{ "major blues", "1 2 3b 3 5 6" },
		{ "blues", "1 3b 4 5b 5 7b" }, 
		{ "composite blues", "1 2 3b 3 4 5b 5 6 7b" },
		{ "augmented", "1 2A 3 5 5A 7" },
		{ "augmented heptatonic", "1 2A 3 4 5 5A 7" },
		{ "dorian #4", "1 2 3b 4# 5 6 7b" },
		{ "lydian diminished", "1 2 3b 4# 5 6 7" },
		{ "whole tone", "1 2 3 4# 5A 7b" },
		{ "leading whole tone", "1 2 3 4# 5A 7b 7" },
		{ "harmonic minor", "1 2 3b 4 5 6b 7" },
		{ "lydian minor", "1 2 3 4# 5 6b 7b" },
		{ "neopolitan", "1 2b 3b 4 5 6b 7" },
		{ "neopolitan minor", "1 2b 3b 4 5 6b 7b" },
		{ "dorian b2", "1 2b 3b 4 5 6 7" },
		{ "neopolitan major pentatonic", "1 3 4 5b 7b" },
		{ "romanian minor", "1 2 3b 5b 5 6 7b" },
		{ "double harmonic lydian", "1 2b 3 4# 5 6b 7" },
		{ "diminished", "1 2 3b 4 5b 6b 6 7" },
		{ "harmonic major", "1 2 3 4 5 6b 7" },
		{ "gypsy", "1 2b 3 4 5 6b 7" },
		{ "egyptian", "1 2 4 5 7b" },
		{ "hungarian minor", "1 2 3b 4# 5 6b 7" },
		{ "hungarian major", "1 2A 3 4# 5 6 7b" },
		{ "oriental", "1 2b 3 4 5b 6 7b" },
		{ "spanish", "1 2b 3 4 5 6b 7b" },
		{ "spanish heptatonic", "1 2b 3b 3 4 5 6b 7b" },
		{ "flamenco", "1 2b 3b 3 4# 5 7b" },
		{ "balinese", "1 2b 3b 4 5 6b 7" },
		{ "todi raga", "1 2b 3b 4# 5 6b 7" },
		{ "malkos raga", "1 3b 4 6b 7b" },
		{ "kafi raga", "1 3b 3 4 5 6 7b 7" },
		{ "purvi raga", "1 2b 3 4 4# 5 6b 7" },
		{ "persian", "1 2b 3 4 5b 6b 7" },
		{ "bebop", "1 2 3 4 5 6 7b 7" },
		{ "bebop dominant", "1 2 3 4 5 6 7b 7" },
		{ "bebop minor", "1 2 3b 3 4 5 6 7b" },
		{ "bebop major", "1 2 3 4 5 5A 6 7" },
		{ "bebop locrian", "1 2b 3b 4 5b 5 6b 7b" },
		{ "minor bebop", "1 2 3b 4 5 6b 7b 7" },
		{ "mystery #1", "1 2b 3 5b 6b 7b" },
		{ "enigmatic", "1 2b 3 5b 6b 7b 7" },
		{ "minor six diminished", "1 2 3b 4 5 6b 6 7" },
		{ "ionian augmented", "1 2 3 4 5A 6 7" },
		{ "lydian #9", "1 2b 3 4# 5 6 7" },
		{ "ichikosucho", "1 2 3 4 5b 5 6 7" },
		{ "six tone symmetric", "1 2b 3 4 5A 6" }
	};
}
