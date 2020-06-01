using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceTest2 : MonoBehaviour
{
    private string[] noteNames = {"ド", "ド♯", "レ", "レ♯", "ミ", "ファ", "ファ♯", "ソ", "ソ♯", "ラ", "ラ♯", "シ"};

    [SerializeField] private Text _text;

    private AudioSource aud;
    
    private float mathTime = 0;
    // Start is called before the first frame update
    void Start()
    {

        aud = GetComponent<AudioSource>();
        // マイク名、ループするかどうか、AudioClipの秒数、サンプリングレート を指定する
        aud.clip = Microphone.Start("マイク (Realtek High Definition Audio)", true, 1, 44100);
        aud.Play();
    }

    // Update is called once per frame
    void Update()
    {
        float[] spectrum = new float[256];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        
        var maxIndex = 0;
        var maxValue = 0.0f;
        for (int i = 0; i < spectrum.Length; i++)
        {
            var val = spectrum[i];
            if (val > maxValue)
            {
                maxValue = val;
                maxIndex = i;
            }
        }
        
        var freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrum.Length;

        _text.text = GetNoteName(freq);

        if (!aud.isPlaying)
        {
            
            aud.Play();
        }
    }
    
    public string GetNoteName(float freq)
    {
        // 周波数からMIDIノートナンバーを計算
        var noteNumber = calculateNoteNumberFromFrequency(freq);
        // 0:C - 11:B に収める
        var note = noteNumber % 12;
        // 0:C～11:Bに該当する音名を返す
        if (note < 0) return null;
        
        Debug.Log(note);
        return noteNames[note];
    }

    // See https://en.wikipedia.org/wiki/MIDI_tuning_standard
    private int calculateNoteNumberFromFrequency(float freq)
    {
        return Mathf.FloorToInt(69 + 12 * Mathf.Log(freq / 440, 2));
    }
}

