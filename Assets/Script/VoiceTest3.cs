using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceTest3 : MonoBehaviour
{
    private AudioClip _clip,c;
    private int head = 0;
    private const int samplingFrequency = 44100;
    private const int lengthSeconds = 1;
    private float[] processBuffer = new float[256];
    private float[] microphoneBuffer = new float[lengthSeconds * samplingFrequency];

    private int math = 0;

    private float mathTime = 0;
    
    private string[] noteNames = {"ド", "ド♯", "レ", "レ♯", "ミ", "ファ", "ファ♯", "ソ", "ソ♯", "ラ", "ラ♯", "シ"};
    
    [SerializeField] private Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _clip = Microphone.Start("マイク (Realtek High Definition Audio)", true, 1, samplingFrequency);
        while (Microphone.GetPosition(null) < 0)
        {
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mathTime >= 1f)
        {
            Recording();
            mathTime = 0;
        }
        else
        {

            mathTime += Time.deltaTime;
        }
    }

    private void Recording()
    {
        var position = Microphone.GetPosition("マイク (Realtek High Definition Audio)");
        if (position < 0 || head == position)
        {
            return;
        }

        _clip.GetData(microphoneBuffer, 0);
        c = _clip;

        int loop = 0;
        
        while (GetDataLength(microphoneBuffer.Length, head, position) > processBuffer.Length)
        {
            var remain = microphoneBuffer.Length - head;
            if (remain < processBuffer.Length)
            {
                Array.Copy(microphoneBuffer, head, processBuffer, 0, remain);
                Array.Copy(microphoneBuffer, 0, processBuffer, remain, processBuffer.Length - remain);
            }
            else
            {
                Array.Copy(microphoneBuffer, head, processBuffer, 0, processBuffer.Length);
            }

            // 擬似コード
            c.SetData(processBuffer, 0);
            SavWav.Save(math + "_" + loop, c);
            Debug.Log(processBuffer + "   " + loop);
            
            loop++;
            
            var maxIndex = 0;
            var maxValue = 0.0f;
            for (int i = 0; i < processBuffer.Length; i++)
            {
                var val = processBuffer[i];
                if (val > maxValue)
                {
                    maxValue = val;
                    maxIndex = i;
                }
            }
        
            var freq = maxIndex * AudioSettings.outputSampleRate / 2 / processBuffer.Length;

            _text.text = GetNoteName(freq);

            head += processBuffer.Length;
            if (head > microphoneBuffer.Length)
            {
                head -= microphoneBuffer.Length;
            }
        }
        math++;
    }

    static int GetDataLength(int bufferLength, int head, int tail)
    {
        if (head < tail)
        {
            return tail - head;
        }
        else
        {
            return bufferLength - head + tail;
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
        
        //Debug.Log(note);
        return noteNames[note];
    }

    // See https://en.wikipedia.org/wiki/MIDI_tuning_standard
    private int calculateNoteNumberFromFrequency(float freq)
    {
        return Mathf.FloorToInt(69 + 12 * Mathf.Log(freq / 440, 2));
    }
}
